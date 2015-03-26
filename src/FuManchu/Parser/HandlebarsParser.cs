﻿namespace FuManchu.Parser
{
	using System;
	using System.Linq;
	using System.Runtime.Remoting.Services;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tags;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Provides parsing services for the Handlebars language.
	/// </summary>
	public class HandlebarsParser : TokenizerBackedParser<HandlebarsTokenizer, HandlebarsSymbol, HandlebarsSymbolType>
	{
		/// <inheritdoc />
		protected override LanguageCharacteristics<HandlebarsTokenizer, HandlebarsSymbol, HandlebarsSymbolType> Language
		{
			get { return HandlebarsLanguageCharacteristics.Instance; }
		}

		/// <summary>
		/// Parses a block tag.
		/// </summary>
		public void AtBlockTag(HandlebarsSymbolType expectedPrefixSymbol = HandlebarsSymbolType.Hash)
		{
			var parent = Context.CurrentBlock;

			string tagName = null;
			TagDescriptor descriptor = null;
			// Start a new block.
			Context.StartBlock(BlockType.Tag);

			using (Context.StartBlock(BlockType.TagElement))
			{
				// Accept the open tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				if (Required(expectedPrefixSymbol, true))
				{
					// Accept the prefix symbol type. tag.
					AcceptAndMoveNext();
					// Output that tag as metacode.
					Output(SpanKind.MetaCode);
				}

				// Parse the content of the element as an expression body.
				AtExpressionBody();

				if (Context.CurrentBlock.Name != null)
				{
					tagName = Context.CurrentBlock.Name;
					descriptor = Context.TagProviders.GetDescriptor(tagName, expectedPrefixSymbol == HandlebarsSymbolType.Negate);
					Context.CurrentBlock.Descriptor = descriptor;
				}

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the closing tag.
				AcceptAndMoveNext();
				// Output this as metacode.
				Output(SpanKind.MetaCode);
			}

			// Special case, as we need to handle branching, so let's merge the last block into the parent block.
			if (tagName == "elseif" && parent != null && parent.Name == "if")
			{
				// Let's merge the current block with the parent and re-instate it.
				Context.MergeCurrentWithParent();
			}
			else
			{
				Context.CurrentBlock.Name = tagName;
				Context.CurrentBlock.Descriptor = descriptor;
			}

			// Switch back to parsing the content of the block.
			ParseBlock();
		}

		/// <summary>
		/// Parses the end of a tag block.
		/// </summary>
		public void AtBlockEndTag()
		{
			string tagName = Context.CurrentBlock.Name;

			using (Context.StartBlock(BlockType.TagElement))
			{
				// Accept the open tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the slash tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);

				// Accept everything until either the close of the tag.
				AcceptUntil(HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
				// Output the first part as an expression.
				Output(SpanKind.Expression);

				// Get the name of the tag.
				string name = LastSpanContent();
				if (!string.Equals(name, tagName))
				{
					Context.OnError(CurrentLocation, "Unbalanced tags - expected a closing tag for '" + Context.CurrentBlock.Name + "' but instead found '" + name + "'");
				}

				Context.CurrentBlock.Name = name;

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the closing tag.
				AcceptAndMoveNext();
				// Output this as metacode.
				Output(SpanKind.MetaCode);
			}

			// End the current block;
			Context.EndBlock();
		}

		/// <summary>
		/// Parses a comment tag.
		/// </summary>
		public void AtCommentTag()
		{
			using (var block = Context.StartBlock(BlockType.Comment))
			{
				// Accept the open tag.
				AcceptAndMoveNext();
				// Output the tag as metacode.
				Output(SpanKind.MetaCode);

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the ! symbol.
				AcceptAndMoveNext();
				// Output the symbol as metacode.
				Output(SpanKind.MetaCode);

				// Accept the comment.
				AcceptAndMoveNext();
				Output(SpanKind.Comment, collapsed: true);

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the closing tag.
				AcceptAndMoveNext();
				Output(SpanKind.MetaCode);
			}
		}

		/// <summary>
		/// Parses an expression.
		/// </summary>
		public void AtExpressionTag(HandlebarsSymbolType? expectedPrefixSymbol = null, SpanKind expectedPrefixSpanKind = SpanKind.MetaCode)
		{
			string tagName = Context.CurrentBlock.Name;

			using (var block = Context.StartBlock(BlockType.Expression))
			{
				// Accept the open tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				if (expectedPrefixSymbol != null && Required(expectedPrefixSymbol.Value, true))
				{
					//Accept the prefix symbol and move next.
					AcceptAndMoveNext();
					// Output the prefix symbol.
					Output(expectedPrefixSpanKind);
				}

				// Parse the expression body.
				AtExpressionBody();

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the closing tag.
				AcceptAndMoveNext();
				// Output this as metacode.
				Output(SpanKind.MetaCode);
			}
		}

		/// <summary>
		/// Handles parsing an expression body.
		/// </summary>
		public void AtExpressionBody()
		{
			string name;
			bool updateTagElementType = false;
			bool isNegated = LastSpanContent() == "^";

			using (Context.StartBlock(BlockType.ExpressionBody))
			{
				// Accept everything until either the close of the tag, or the first element of whitespace.
				AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde, HandlebarsSymbolType.OpenParenthesis);
				// Output the first part as an expression.
				Output(SpanKind.Expression);

				name = LastSpanContent();

				if (isNegated && string.IsNullOrWhiteSpace(name))
				{
					name = "^";
				}
				Context.CurrentBlock.Name = name;

				// Special case - else expressions become tag elements themselves.
				if (name == "else" || isNegated)
				{
					updateTagElementType = true;
				}

				while (CurrentSymbol.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Type != HandlebarsSymbolType.Tilde && CurrentSymbol.Type != HandlebarsSymbolType.CloseParenthesis)
				{
					if (CurrentSymbol.Type == HandlebarsSymbolType.WhiteSpace)
					{
						// Accept all the whitespace.
						AcceptAll(HandlebarsSymbolType.WhiteSpace);
						// Take all the whitespace, and output that.
						Output(SpanKind.WhiteSpace);
					}

					if (CurrentSymbol.Type == HandlebarsSymbolType.OpenParenthesis)
					{
						// Start parsing a sub-expression.
						AtSubExpression();
					}

					if (CurrentSymbol.Type == HandlebarsSymbolType.Assign)
					{
						// We're in a parameterised argument (e.g. one=two
						AcceptAndMoveNext();
						// Accept everything until the next whitespace or closing tag.
						AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde, HandlebarsSymbolType.CloseParenthesis);
						// Output this as a map.
						Output(SpanKind.Map);
					}
					else
					{
						// Accept everything until the next whitespace or closing tag.
						AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde, HandlebarsSymbolType.CloseParenthesis);
						if (CurrentSymbol.Type == HandlebarsSymbolType.Assign)
						{
							continue;
						}
						// Output this as a map.
						Output(SpanKind.Parameter);
					}
				}

				if (Context.CurrentBlock.Children.Count == 0)
				{
					// The block has no children, so ignore it.
					Context.CurrentBlock.Ignore = true;
				}
			}

			if (updateTagElementType)
			{
				Context.CurrentBlock.Type = BlockType.TagElement;
			}
			Context.CurrentBlock.Name = name;
		}

		/// <summary>
		/// Parses an expression.
		/// </summary>
		public void AtPartialTag()
		{
			string tagName = Context.CurrentBlock.Name;

			using (var block = Context.StartBlock(BlockType.Partial))
			{
				// Accept the open tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the right arrow >.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);

				// Parse the inner as an expression body.
				AtExpressionBody();

				if (Optional(HandlebarsSymbolType.Tilde))
				{
					// Output the tilde.
					Output(SpanKind.MetaCode);
				}

				// Accept the closing tag.
				AcceptAndMoveNext();
				// Output this as metacode.
				Output(SpanKind.MetaCode);
			}
		}

		/// <summary>
		/// Parses a sub-expression.
		/// </summary>
		public void AtSubExpression()
		{
			using (Context.StartBlock(BlockType.SubExpression))
			{
				if (Required(HandlebarsSymbolType.OpenParenthesis, true))
				{
					AcceptAndMoveNext();
					// Output the ( symbol.
					Output(SpanKind.MetaCode);
				}

				AtExpressionBody();

				if (Required(HandlebarsSymbolType.CloseParenthesis, true))
				{
					AcceptAndMoveNext();
					// Output the ) symbol.
					Output(SpanKind.MetaCode);;
				}
			}
		}

		/// <summary>
		/// Parses a tag.
		/// </summary>
		public void AtTag()
		{
			var current = CurrentSymbol;
			NextToken();
			HandlebarsSymbol tilde = null;

			if (CurrentSymbol.Type == HandlebarsSymbolType.Tilde)
			{
				tilde = CurrentSymbol;
				NextToken();
			}

			if (CurrentSymbol.Type == HandlebarsSymbolType.Hash)
			{
				// Put the opening tag back.
				PutBack(CurrentSymbol);
				if (tilde != null)
				{
					PutBack(tilde);
				}
				PutBack(current);
				NextToken();
				// We're at a block tag {{#hello}} etc.
				AtBlockTag();
			}
			else if (CurrentSymbol.Type == HandlebarsSymbolType.Bang)
			{
				// Put the opening tag back.
				PutBack(CurrentSymbol);
				if (tilde != null)
				{
					PutBack(tilde);
				}
				PutBack(current);
				NextToken();
				// We're at a comment {{!....}}
				AtCommentTag();
			}
			else if (CurrentSymbol.Type == HandlebarsSymbolType.Slash)
			{
				// Put the opening tag back.
				PutBack(CurrentSymbol);
				if (tilde != null)
				{
					PutBack(tilde);
				}
				PutBack(current);
				NextToken();
				// We're at a closing block tag {{/each}}
				AtBlockEndTag();
			}
			else if (CurrentSymbol.Type == HandlebarsSymbolType.RightArrow)
			{
				// Put the opening tag back.
				PutBack(CurrentSymbol);
				if (tilde != null)
				{
					PutBack(tilde);
				}
				PutBack(current);
				NextToken();
				// We're at a partial include tag {{>body}}
				AtPartialTag();
			}
			else if (CurrentSymbol.Type == HandlebarsSymbolType.Negate)
			{
				var current2 = CurrentSymbol;
				// Step foward and see if this is a block or expression.
				NextToken();
				if (CurrentSymbol.Type == HandlebarsSymbolType.CloseTag)
				{
					// This is an expression.	
					PutBack(CurrentSymbol);

					// Put the opening tag back.
					PutBack(current2);
					if (tilde != null)
					{
						PutBack(tilde);
					}
					PutBack(current);
					NextToken();
					// We're at a negated block tag {{^hello}} etc.
					AtExpressionTag(HandlebarsSymbolType.Negate, SpanKind.Expression);
				}
				else
				{
					PutBack(CurrentSymbol);

					// Put the opening tag back.
					PutBack(current2);
					if (tilde != null)
					{
						PutBack(tilde);
					}
					PutBack(current);
					NextToken();
					// We're at a negated block tag {{^hello}} etc.
					AtBlockTag(HandlebarsSymbolType.Negate);
				}
			}
			else if (CurrentSymbol.Type == HandlebarsSymbolType.Ampersand)
			{
				// Put the opening tag back.
				PutBack(CurrentSymbol);
				if (tilde != null)
				{
					PutBack(tilde);
				}
				PutBack(current);
				NextToken();
				// Handle an expression tag.
				AtExpressionTag(HandlebarsSymbolType.Ampersand);
			}
			else
			{
				// Put the opening tag back.
				PutBack(CurrentSymbol);
				if (tilde != null)
				{
					PutBack(tilde);
				}
				PutBack(current);
				NextToken();
				// Handle an expression tag.
				AtExpressionTag();
			}
		}

		/// <summary>
		/// Gets the content of the last span.
		/// </summary>
		/// <returns>The span content.</returns>
		private string LastSpanContent()
		{
			var span = Context.CurrentBlock.Children.LastOrDefault() as Span;
			if (span != null)
			{
				return span.Content;
			}
			return null;
		}

		/// <inheritdoc />
		public override void ParseBlock()
		{
			// Accept any leading whitespace.
			AcceptWhile(HandlebarsSymbolType.WhiteSpace);
			// Output the whitespace.
			Output(SpanKind.WhiteSpace);

			// Accept everything until we meet a tag (either {{ or {{{).
			AcceptUntil(HandlebarsSymbolType.OpenTag, HandlebarsSymbolType.RawOpenTag, HandlebarsSymbolType.WhiteSpace);

			// Output everything we have so far as text.
			Output(SpanKind.Text);

			if (EndOfFile || CurrentSymbol == null)
			{
				return;
			}

			if (CurrentSymbol.Type == HandlebarsSymbolType.OpenTag || CurrentSymbol.Type == HandlebarsSymbolType.RawOpenTag)
			{
				// Now we're at a tag.
				AtTag();
			}
		}

		/// <inheritdoc />
		public override void ParseDocument()
		{
			using (PushSpanConfig())
			{
				if (Context == null)
				{
					throw new InvalidOperationException("Context has not been set.");
				}

				using (Context.StartBlock(BlockType.Document))
				{
					if (!NextToken())
					{
						return;
					}

					while (!EndOfFile)
					{
						ParseBlock();
					}
				}
			}
		}
	}
}