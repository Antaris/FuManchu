namespace FuManchu.Renderer
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Renders a partial include.
	/// </summary>
	public class PartialBlockRenderer : BlockRenderer
	{
		/// <inheritdoc />
		protected override void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer)
		{
			if (context.Service == null)
			{
				// No service, can't do anything.
				return;
			}

			string name = block.Name;
			object model = arguments.FirstOrDefault();

			if (model != null)
			{
				using (var scope = context.BeginScope(model))
				{
					Write(scope.Context, writer, new SafeString(context.Service.RunPartial(name, scope.Context)));
				}
			}
			else
			{
				Write(context, writer, new SafeString(context.Service.RunPartial(name, context)));
			}
		}
	}
}
