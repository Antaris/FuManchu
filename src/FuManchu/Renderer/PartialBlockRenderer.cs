﻿namespace FuManchu.Renderer
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

			var span = block.Children.FirstOrDefault(c => !c.IsBlock && ((Span)c).Kind == SpanKind.Expression) as Span;
			if (span == null)
			{
				// Malformed tag?
				return;
			}

			string name = span.Content;
			object model = arguments.FirstOrDefault();

			if (model != null)
			{
				using (var scope = context.BeginScope(model))
				{
					Write(scope.ScopeContext, writer, new SafeString(context.Service.RunPartial(name, scope.ScopeContext)));
				}
			}
			else
			{
				Write(context, writer, new SafeString(context.Service.RunPartial(name, context)));
			}
		}
	}
}
