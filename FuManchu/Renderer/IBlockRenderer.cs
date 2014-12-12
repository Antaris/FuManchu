﻿namespace FuManchu.Renderer
{
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Defines the required contract for implementing a block renderer.
	/// </summary>
	public interface IBlockRenderer : ISyntaxTreeNodeRenderer<Block>
	{
		/// <summary>
		/// Renders the child node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="context">The context.</param>
		void RenderChild(SyntaxTreeNode node, RenderContext context);
	}
}