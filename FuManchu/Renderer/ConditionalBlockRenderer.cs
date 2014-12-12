namespace FuManchu.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides rendering of {{#if ...}} {{/if}} blocks, with support for {{#elseif}} and {{else}}
	/// </summary>
	public class ConditionalBlockRenderer : BlockRenderer
	{
		/// <inheritdoc />
		public override void Render(Block target, RenderContext context, TextWriter writer)
		{
			// 1. Figure out branching blocks.
			// 2. For each branching block (first to last), resolve conditional argument.
			var blocks = ParseConditionalBlocks(target);

			foreach (var block in blocks)
			{
				var arguments = GetArgumentsAndMappedParameters(block.Item1, context);
				if (block.Item1.Name == "if" || block.Item1.Name == "elseif")
				{
					
				}
			}
		}

		/// <inheritdoc />
		protected override void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer)
		{

		}

		/// <summary>
		/// Parses the conditional blocks.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		private List<Tuple<Block, List<SyntaxTreeNode>>> ParseConditionalBlocks(Block target)
		{
			var result = new List<Tuple<Block, List<SyntaxTreeNode>>>();

			Block current = null;
			var nodes = new List<SyntaxTreeNode>();

			foreach (var node in target.Children)
			{
				bool isNewElement = false;
				if (node.IsBlock)
				{
					var block = (Block)node;
					if (block.Type == BlockType.TagElement && (block.Name == "if" || block.Name == "elseif" || block.Name == "else"))
					{
						if (current != null)
						{
							result.Add(Tuple.Create(current, nodes));
							result = new List<Tuple<Block, List<SyntaxTreeNode>>>();
						}
						current = block;
						isNewElement = true;
					}
				}

				if (!isNewElement)
				{
					nodes.Add(node);
				}
			}

			if (current != null)
			{
				result.Add(Tuple.Create(current, nodes));
			}

			return result;
		}
	}
}