namespace FuManchu.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Represents a context for rendering a syntax tree node.
	/// </summary>
	public class RenderContext : ContextBase<RenderContext>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderContext"/> class.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		/// <param name="parentContext">The parent render context.</param>
		public RenderContext(ParserVisitor<RenderContext> visitor, RenderContext parentContext = null)
			: base(visitor, parentContext)
		{

		}

		/// <summary>
		/// Gets or sets whether we are escaping text encoding.
		/// </summary>
		public bool EscapeEncoding { get; internal set; }

		/// <inheritdoc />
		protected override RenderContext CreateChildContext(object model)
		{
			return RenderContextFactory.CreateRenderContext(this, model);
		}
	}
}