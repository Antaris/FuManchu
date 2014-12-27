namespace FuManchu.Parser
{
	using System.Collections.Generic;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Represents a visitor for walking a parsed syntax tree.
	/// </summary>
	public abstract class ParserVisitor<TScope> : IParserVisitor
	{
		private readonly Stack<TScope> _scopes = new Stack<TScope>();

		/// <summary>
		/// Gets the scope.
		/// </summary>
		public TScope Scope
		{
			get { return _scopes.Peek(); }
		}

		/// <inheritdoc />
		public virtual void OnComplete() { }

		/// <summary>
		/// Reverts the current scope.
		/// </summary>
		public virtual void RevertScope()
		{
			_scopes.Pop();
		}

		/// <summary>
		/// Sets the current scope.
		/// </summary>
		/// <param name="scope">The scope instance.</param>
		public virtual void SetScope(TScope scope)
		{
			_scopes.Push(scope);
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		public virtual void VisitError(Error error) { }

		/// <inheritdoc />
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