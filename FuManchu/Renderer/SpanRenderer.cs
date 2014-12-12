namespace FuManchu.Renderer
{
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides a base implementation of a span renderer.
	/// </summary>
	public abstract class SpanRenderer : SyntaxTreeNodeRenderer<Span>, ISpanRenderer { }
}