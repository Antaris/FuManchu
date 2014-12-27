namespace FuManchu.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides rendering of a Handlebars document.
	/// </summary>
	public class RenderingParserVisitor : ParserVisitor<RenderContext>
	{
		private readonly TextWriter _textWriter;
		private readonly IDictionary<SpanKind, ISpanRenderer> _spanRenderers = new Dictionary<SpanKind, ISpanRenderer>()
		{
			{ SpanKind.Text, new TextSpanRenderer() },
			{ SpanKind.MetaCode, new MetaCodeSpanRenderer() },
			{ SpanKind.Expression, new ExpressionSpanRenderer() }
		};

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

			var context = new RenderContext(this)
			{
				TemplateData = new TemplateData()
				{
					Model = model,
					ModelMetadata = (model == null) ? null : modelMetadataProvider.GetMetadataForType(() => model, model.GetType())
				},
				ModelMetadataProvider = ModelMetadataProvider
			};

			SetScope(context);
		}

		/// <summary>
		/// Gets the model metadata provider.
		/// </summary>
		public IModelMetadataProvider ModelMetadataProvider { get; private set; }

		/// <inheritdoc />
		public override void VisitBlock(Block block)
		{
			if (block.Descriptor != null)
			{
				block.Descriptor.Renderer.Render(block, Scope, _textWriter);
			}
			else
			{
				base.VisitBlock(block);
			}
		}

		/// <inheritdoc />
		public override void VisitSpan(Span span)
		{
			ISpanRenderer renderer = null;
			if (_spanRenderers.TryGetValue(span.Kind, out renderer))
			{
				renderer.Render(span, Scope, _textWriter);
			}
		}
	}
}