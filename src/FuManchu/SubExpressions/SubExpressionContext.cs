namespace FuManchu.SubExpressions
{
	using FuManchu.Parser;
	using FuManchu.Renderer;

	/// <summary>
	/// Represents a context for tracking the result of a sub-expression.
	/// </summary>
	public class SubExpressionContext
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="SubExpressionContext"/> class.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public SubExpressionContext(ParserVisitor<SubExpressionContext> visitor)
		{
			Visitor = visitor;
		}

		/// <summary>
		/// Gets the parser visitor.
		/// </summary>
		public ParserVisitor<SubExpressionContext> Visitor { get; private set; }

		/// <summary>
		/// Gets the result of a sub-expression.
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// Gets or sets the Handlebars service.
		/// </summary>
		public IHandlebarsService Service { get; set; }
	}
}