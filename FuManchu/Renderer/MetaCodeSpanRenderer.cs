namespace FuManchu.Renderer
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Provides rendering of a ~ character which expands the whitespace of previous/next metacode spans.
	/// </summary>
	public class MetaCodeSpanRenderer : SpanRenderer
	{
		/// <inheritdoc />
		public override void Render(Span target, RenderContext context, TextWriter writer)
		{
			switch (target.Content)
			{
				case "~":
				{
					RenderAdjacentWhiteSpace(target, writer);
					break;
				}
			}
		}

		/// <summary>
		/// Renders any adjacent whitespace characters.
		/// </summary>
		/// <param name="target">The target span.</param>
		/// <param name="writer">The text writer.</param>
		public void RenderAdjacentWhiteSpace(Span target, TextWriter writer)
		{
			if (target.Previous != null && target.Previous.Kind == SpanKind.MetaCode)
			{
				WriteWhiteSpace(target.Previous.Symbols, writer);
			}

			if (target.Next != null && target.Next.Kind == SpanKind.MetaCode)
			{
				WriteWhiteSpace(target.Next.Symbols, writer);
			}
		}

		/// <summary>
		/// Writes the whitespace for the given set of symbols.
		/// </summary>
		/// <param name="symbols">The set of symbols.</param>
		/// <param name="writer">The text writer.</param>
		private static void WriteWhiteSpace(IEnumerable<ISymbol> symbols, TextWriter writer)
		{
			int length = symbols
				.Cast<HandlebarsSymbol>()
				.Where(hs => hs.Type == HandlebarsSymbolType.OpenTag
				             || hs.Type == HandlebarsSymbolType.RawOpenTag
				             || hs.Type == HandlebarsSymbolType.CloseTag
				             || hs.Type == HandlebarsSymbolType.RawCloseTag)
				.Sum(hs => hs.Content.Length);

			for (int i = 0; i < length; i++)
			{
				writer.Write(" ");
			}
		}
	}
}