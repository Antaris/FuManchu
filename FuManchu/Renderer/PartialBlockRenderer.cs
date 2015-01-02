namespace FuManchu.Renderer
{
	using System.Collections.Generic;
	using System.IO;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Renders a partial include.
	/// </summary>
	public class PartialBlockRenderer : BlockRenderer
	{
		/// <inheritdoc />
		protected override void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer)
		{

		}
	}
}
