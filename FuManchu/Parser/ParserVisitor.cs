namespace FuManchu.Parser
{
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Represents a visitor for walking a parsed syntax tree.
	/// </summary>
	public abstract class ParserVisitor
	{
		/// <summary>
		/// Called when the visitor has finished walking the syntax tree.
		/// </summary>
		public virtual void OnComplete() { }

		/// <summary>
		/// Visits the block.
		/// </summary>
		/// <param name="block">The block.</param>
		public virtual void VisitBlock(Block block)
		{
			VisitStartBlock(block);
			foreach (var node in block.Children)
			{
				node.Accept(this);
			}
			VisitEndBlock(block);
		}

		/// <summary>
		/// Processed after a all child nodes of a block are visited.
		/// </summary>
		/// <param name="block">The block.</param>
		public virtual void VisitEndBlock(Block block) { }

		/// <summary>
		/// Processed after an error has been encountered.
		/// </summary>
		/// <param name="error">The error.</param>
		public virtual void VisitError(Error error) { }

		/// <summary>
		/// Visits the span.
		/// </summary>
		/// <param name="span">The span.</param>
		public virtual void VisitSpan(Span span)
		{
			
		}

		/// <summary>
		/// Processed before any child nodes of a block are visited.
		/// </summary>
		/// <param name="block">The block.</param>
		public virtual void VisitStartBlock(Block block) { }
	}
}