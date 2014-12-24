namespace FuManchu.Renderer
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides rendering of implicit block tags, e.g. {{#people}}{{/people}}.
	/// </summary>
	public class ImplicitBlockRenderer : EnumerableBlockRenderer
	{
		/// <inheritdoc />
		protected override void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer)
		{
			string name = block.Name;

			var children = block.Children.ToList();
			children.RemoveAt(0);
			children.RemoveAt(children.Count - 1);

			if (string.IsNullOrEmpty(name))
			{
				// Nothing we can do.
				return;
			}

			object value = context.ResolveValue(name, false);
			if (value == null)
			{
				// No value, nothing we can do :-(
				return;
			}

			if ((value is IEnumerable) && !(value is string))
			{
				RenderEnumerable((IEnumerable)value, context, children, null);
			}
			else
			{
				// Treat this as a conditional block.
				if (IsTruthy(value))
				{
					// Create a scope around the value.
					using (var scope = context.BeginScope(value))
					{
						RenderChildren(children, context);
					}
				}
			}
		}
	}
}