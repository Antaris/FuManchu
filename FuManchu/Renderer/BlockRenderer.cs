namespace FuManchu.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides a base implementation of a block renderer.
	/// </summary>
	public abstract class BlockRenderer : SyntaxTreeNodeRenderer<Block>, IBlockRenderer
	{
		/// <inheritdoc />
		public override void Render(Block target, RenderContext context, TextWriter writer)
		{
			var parameters = GetArgumentsAndMappedParameters(target, context);

			Render(target, parameters.Item1, parameters.Item2, context, writer);
		}

		/// <summary>
		/// Renders the specified block.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <param name="arguments">The arguments.</param>
		/// <param name="maps">The maps.</param>
		/// <param name="context">The context.</param>
		/// <param name="writer">The writer.</param>
		protected abstract void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer);

		/// <inheritdoc />
		public void RenderChild(SyntaxTreeNode node, RenderContext context)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			node.Accept(context.Visitor);
		}
	}
}