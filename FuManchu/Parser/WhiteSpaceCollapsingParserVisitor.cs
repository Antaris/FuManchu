namespace FuManchu.Parser
{
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Collapses whitespace in a syntax tree.
	/// </summary>
	public class WhiteSpaceCollapsingParserVisitor : ParserVisitor
	{
		/// <summary>
		/// Collapses the next whitespace instance.
		/// </summary>
		/// <param name="span">The tilde span.</param>
		private void CollapseNextWhiteSpace(Span span)
		{
			
		}

		/// <summary>
		/// Collapses the previous whitespace instance.
		/// </summary>
		/// <param name="span">The tilde span.</param>
		private void CollapsePreviousWhiteSpace(Span span)
		{
			// Walk to parent block. (TagElement)
			var element = span.Parent;
			// Walk to the parent block. (Tag)
			var tag = element.Parent;
			// Walk to the parent block. (Tag or Document)
			var root = tag.Parent;

			var items = root.Children.ToList();
			int index = items.IndexOf(tag);
			if (index > 0)
			{
				var potential = items[index - 1] as Span;
				if (potential != null && potential.Kind == SpanKind.WhiteSpace)
				{
					// Remove this item.
					items.Remove(potential);
					// Override the items.
					root.ReplaceChildren(items);
				}
			}
		}

		/// <inheritdoc />
		public override void VisitSpan(Span span)
		{
			if (span.Kind == SpanKind.MetaCode)
			{
				var symbol = span.Symbols.FirstOrDefault() as HandlebarsSymbol;
				if (symbol != null && symbol.Type == HandlebarsSymbolType.Tilde)
				{
					VisitTilde(span);
				}
			}

			base.VisitSpan(span);
		}

		/// <summary>
		/// Visits a ~ character which allows collapsing of whitespace.
		/// </summary>
		/// <param name="span">The tilde span.</param>
		public void VisitTilde(Span span)
		{
			if (span.Previous != null && span.Previous.Kind == SpanKind.MetaCode && span.Previous.Content.EndsWith("{{"))
			{
				CollapsePreviousWhiteSpace(span);
			} 
			else if (span.Next != null && span.Next.Kind == SpanKind.MetaCode && span.Next.Content.StartsWith("}}"))
			{
				CollapseNextWhiteSpace(span);
			}
		}
	}
}