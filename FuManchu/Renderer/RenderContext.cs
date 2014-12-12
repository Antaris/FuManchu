namespace FuManchu.Renderer
{
	using FuManchu.Parser;

	/// <summary>
	/// Represents a context for rendering a syntax tree node.
	/// </summary>
	public class RenderContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderContext"/> class.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		/// <param name="parentRenderContext">The parent render context.</param>
		public RenderContext(ParserVisitor visitor, RenderContext parentRenderContext = null)
		{
			ParentRenderContext = parentRenderContext;
			Visitor = visitor;
		}

		/// <summary>
		/// Gets or sets the model.
		/// </summary>
		public object Model { get; set; }

		/// <summary>
		/// Gets the parent render context.
		/// </summary>
		public RenderContext ParentRenderContext { get; private set; }

		/// <summary>
		/// Gets the parser visitor.
		/// </summary>
		public ParserVisitor Visitor { get; private set; }
	}
}