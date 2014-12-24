namespace FuManchu.Renderer
{
	using System.IO;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides rendering of expressions.
	/// </summary>
	public class ExpressionSpanRenderer : SpanRenderer
	{
		/// <inheritdoc />
		public override void Render(Span target, RenderContext context, TextWriter writer)
		{
			object value = context.ResolveValue(target);

			if (value != null)
			{
				writer.Write(value.ToString());
			}
		}
	}
}