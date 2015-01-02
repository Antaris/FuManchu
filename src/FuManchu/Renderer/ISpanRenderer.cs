namespace FuManchu.Renderer
{
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Defines the required contract for implementing a span renderer.
	/// </summary>
	public interface ISpanRenderer : ISyntaxTreeNodeRenderer<Span>
	{
	}
}