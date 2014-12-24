namespace FuManchu.Renderer
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Handles rendering {{#unless }} tags.
	/// </summary>
	public class UnlessBlockRenderer : BlockRenderer
	{
		/// <inheritdoc />
		protected override void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer)
		{
			var children = block.Children.ToList();
			children.RemoveAt(0);
			children.RemoveAt(children.Count - 1);

			var elseChildren = new List<SyntaxTreeNode>();

			// Determine if there is an alternate {{else}} block which denotes content to display when predicate is false.
			var elseNode = children.Find(n => n.IsBlock && ((Block)n).Name == "else");
			if (elseNode != null)
			{
				int elseIndex = children.IndexOf(elseNode);
				elseChildren = children.Skip(elseIndex + 1).ToList();
				children = children.Take(elseIndex).ToList();
			}

			if (!IsTruthy(arguments[0]))
			{
				RenderChildren(children, context);
			}
			else if (elseChildren.Count > 0)
			{
				RenderChildren(elseChildren, context);
			}
		}
	}
}