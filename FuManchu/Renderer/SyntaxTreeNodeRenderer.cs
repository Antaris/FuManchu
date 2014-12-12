namespace FuManchu.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Provides a base implementation of a syntax tree renderer.
	/// </summary>
	/// <typeparam name="T">The node type.</typeparam>
	public abstract class SyntaxTreeNodeRenderer<T> : ISyntaxTreeNodeRenderer<T> where T : SyntaxTreeNode
	{
		/// <inheritdoc />
		public abstract void Render(T target, RenderContext context, TextWriter writer);

		/// <summary>
		/// Gets the arguments and mapped parameters from the given node.
		/// </summary>
		/// <param name="block">The target block.</param>
		/// <param name="context">The render context.</param>
		/// <returns>The set of arguments and mapped parameters.</returns>
		protected virtual Tuple<object[], Dictionary<string, object>> GetArgumentsAndMappedParameters(Block block, RenderContext context)
		{
			var arguments = new List<object>();
			var maps = new Dictionary<string, object>();

			var items = block.Children.OfType<Span>().Where(c => c.Kind == SpanKind.Parameter || c.Kind == SpanKind.Map);

			foreach (var span in items)
			{
				if (span.Kind == SpanKind.Parameter)
				{
					arguments.Add(ResolveValue(span.Symbols, context));
				}
				else
				{
					var symbols = span.Symbols.Cast<HandlebarsSymbol>().ToList();
					string key = symbols[0].Content;

					object value = ResolveValue(symbols.Skip(2), context);

					if (maps.ContainsKey(key))
					{
						maps[key] = value;
					}
					else
					{
						maps.Add(key, value);
					}
				}
			}

			return Tuple.Create(arguments.ToArray(), maps);
		}

		/// <summary>
		/// Resolves the value for the given
		/// </summary>
		/// <param name="symbols">The symbols.</param>
		/// <param name="context">The render context.</param>
		/// <returns>The resolved value.</returns>
		protected object ResolveValue(IEnumerable<ISymbol> symbols, RenderContext context)
		{
			// Do work here.
			return null;
		}
	}
}