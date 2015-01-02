namespace FuManchu.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Text;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Provides rendering of a Handlebars document.
	/// </summary>
	public class RenderingParserVisitor : ParserVisitor<RenderContext>
	{
		private readonly TextWriter _textWriter;
		private readonly IDictionary<SpanKind, ISpanRenderer> _spanRenderers = new Dictionary<SpanKind, ISpanRenderer>()
		{
			{ SpanKind.Text, new TextSpanRenderer() },
			{ SpanKind.WhiteSpace, new WhiteSpaceSpanRenderer() },
			{ SpanKind.Expression, new ExpressionSpanRenderer() }
		};
		private IHandlebarsService _handlebarsService;

		/// <summary>
		/// Initialises a new instance of <see cref="RenderingParserVisitor"/>
		/// </summary>
		/// <param name="writer">The text writer</param>
		/// <param name="model">The document model.</param>
		/// <param name="modelMetadataProvider">The model metadata provider.</param>
		public RenderingParserVisitor(TextWriter writer, object model, IModelMetadataProvider modelMetadataProvider)
		{
			_textWriter = writer;
			ModelMetadataProvider = modelMetadataProvider;

			var context = RenderContextFactory.CreateRenderContext(this, model);
			SetScope(context);
		}

		/// <summary>
		/// Initialises a new instance of <see cref="RenderingParserVisitor"/>
		/// </summary>
		/// <param name="writer">The text writer</param>
		/// <param name="context">The render context.</param>
		/// <param name="modelMetadataProvider">The model metadata provider.</param>
		public RenderingParserVisitor(TextWriter writer, RenderContext context, IModelMetadataProvider modelMetadataProvider)
		{
			_textWriter = writer;
			ModelMetadataProvider = modelMetadataProvider;

			SetScope(context);
		}

		/// <summary>
		/// Gets the model metadata provider.
		/// </summary>
		public IModelMetadataProvider ModelMetadataProvider { get; private set; }

		/// <summary>
		/// Gets the Handlebars service.
		/// </summary>
		public IHandlebarsService Service
		{
			get { return _handlebarsService; }
			set
			{
				_handlebarsService = value;
				Scope.Service = _handlebarsService;
			}
		}

		/// <inheritdoc />
		public override void VisitBlock(Block block)
		{
			if (block.Descriptor != null)
			{
				block.Descriptor.Renderer.Render(block, Scope, _textWriter);
			}
			else if (block.Type == BlockType.Partial && Service != null)
			{
				VisitPartial(block);
			}
			else
			{
				base.VisitBlock(block);
			}
		}

		/// <summary>
		/// Visits a metacode span.
		/// </summary>
		/// <param name="span">The metacode span.</param>
		public void VisitMetaCodeSpan(Span span)
		{
			var symbol = span.Symbols.FirstOrDefault() as HandlebarsSymbol;
			if (symbol == null)
			{
				VisitError(new Error("Expected span to have at least 1 symbol", span.Start, span.Content.Length));
			}

			switch (symbol.Type)
			{
				case HandlebarsSymbolType.RawOpenTag:
				{
					// Tell the render context that it is rendering in escaped mode.
					Scope.EscapeEncoding = true;
					break;
				}
				case HandlebarsSymbolType.RawCloseTag:
				{
					// Tell the render context that it is no longer in escaped mode.
					Scope.EscapeEncoding = false;
					break;
				}
				case HandlebarsSymbolType.Tilde:
				{

					break;
				}
			}
		}

		/// <summary>
		/// Visits a partial reference block.
		/// </summary>
		/// <param name="block">The block.</param>
		public void VisitPartial(Block block)
		{
			new PartialBlockRenderer().Render(block, Scope, _textWriter);
		}

		/// <inheritdoc />
		public override void VisitSpan(Span span)
		{
			if (span.Kind == SpanKind.MetaCode)
			{
				VisitMetaCodeSpan(span);
			}
			else
			{
				ISpanRenderer renderer = null;
				if (_spanRenderers.TryGetValue(span.Kind, out renderer))
				{
					renderer.Render(span, Scope, _textWriter);
				}
			}
		}
	}
}