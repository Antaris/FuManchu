namespace FuManchu
{
	using System;
	using System.Collections.Concurrent;
	using System.IO;
	using System.Linq;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Renderer;
	using FuManchu.Tags;
	using FuManchu.Text;

	/// <summary>
	/// Provides services for running Handlebars templates.
	/// </summary>
	public class HandlebarsService : IHandlebarsService
	{
		private readonly ConcurrentDictionary<string, Func<HelperOptions, string>> _helpers = new ConcurrentDictionary<string, Func<HelperOptions, string>>(); 
		private readonly ConcurrentDictionary<string, Func<RenderContext, string>> _partials = new ConcurrentDictionary<string, Func<RenderContext, string>>();
		private readonly ConcurrentDictionary<string, Func<object, string>> _templates = new ConcurrentDictionary<string, Func<object, string>>(); 

		/// <summary>
		/// Initialises a new instance of <see cref="HandlebarsService"/>
		/// </summary>
		public HandlebarsService()
		{
			TagProviders = new TagProvidersCollection(TagProvidersCollection.Default);
			ModelMetadataProvider = new DefaultModelMetadataProvider();
		}

		/// <summary>
		/// The collection of tag providers.
		/// </summary>
		public TagProvidersCollection TagProviders { get; private set; }

		/// <summary>
		/// The model metadata provider.
		/// </summary>
		public IModelMetadataProvider ModelMetadataProvider { get; set; }

		/// <inheritdoc />
		public Func<object, string> Compile(string template)
		{
			var document = CreateDocument(template);

			// Collapse any whitespace.
			var whitespace = new WhiteSpaceCollapsingParserVisitor();
			document.Accept(whitespace);

			return (model) =>
			       {
				       using (var writer = new StringWriter())
				       {
					       var render = new RenderingParserVisitor(writer, model, ModelMetadataProvider ?? new DefaultModelMetadataProvider())
					                    {
						                    Service = this
					                    };

					       // Render the document.
					       document.Accept(render);

					       return writer.GetStringBuilder().ToString();
				       }
			       };
		}

		/// <inheritdoc />
		public Func<object, string> Compile(string name, string template)
		{
			Func<object, string> func;
			if (_templates.TryGetValue(name, out func))
			{
				return func;
			}

			func = Compile(template);
			_templates.TryAdd(name, func);

			return func;
		}

		/// <inheritdoc />
		public string CompileAndRun(string name, string template, object model = null)
		{
			Func<object, string> func = (string.IsNullOrEmpty(name)) ? Compile(template) : Compile(name, template);

			return func(model);
		}

		/// <inheritdoc />
		public Func<RenderContext, string> CompilePartial(string template)
		{
			var document = CreateDocument(template);

			// Collapse any whitespace.
			var whitespace = new WhiteSpaceCollapsingParserVisitor();
			document.Accept(whitespace);

			return (context) =>
			{
				using (var writer = new StringWriter())
				{
					var render = new RenderingParserVisitor(writer, context, ModelMetadataProvider ?? new DefaultModelMetadataProvider())
					             {
						             Service = this
					             };

					// Render the document.
					document.Accept(render);

					return writer.GetStringBuilder().ToString();
				}
			};
		}

		/// <inheritdoc />
		public bool HasRegisteredHelper(string name)
		{
			return _helpers.ContainsKey(name);
		}

		/// <inheritdoc />
		public void RegisterHelper(string name, Func<HelperOptions, string> helper)
		{
			Func<HelperOptions, string> temp;
			if (!_helpers.TryGetValue(name, out temp))
			{
				_helpers.TryAdd(name, helper);
			}
		}

		/// <summary>
		/// Registers a partial template with the given name.
		/// </summary>
		/// <param name="name">The name of the partial template.</param>
		/// <param name="func">The partial delegate.</param>
		public void RegisterPartial(string name, Func<RenderContext, string> func)
		{
			Func<RenderContext, string> temp;
			if (!_partials.TryGetValue(name, out temp))
			{
				_partials.TryAdd(name, func);
			}
		}

		/// <inheritdoc />
		public void RegisterPartial(string name, string template)
		{
			Func<RenderContext, string> func;
			if (!_partials.TryGetValue(name, out func))
			{
				func = CompilePartial(template);
				_partials.TryAdd(name, func);
			}
		}

		/// <summary>
		/// Removes a compiled template.
		/// </summary>
		/// <param name="name">The name of the compiled template.</param>
		public void RemoveCompiledTemplate(string name)
		{
			Func<object, string> func;
			_templates.TryRemove(name, out func);
		}

		/// <summary>
		/// Removes a compiled partial template.
		/// </summary>
		/// <param name="name">The name of the compiled partial template.</param>
		public void RemoveCompiledPartial(string name)
		{
			Func<RenderContext, string> func;
			_partials.TryRemove(name, out func);
		}

		/// <inheritdoc />
		public string Run(string name, object model = null)
		{
			Func<object, string> func;
			if (_templates.TryGetValue(name, out func))
			{
				return func(model);
			}

			throw new ArgumentException("No template called '" + name + "' has been compiled.");
		}

		/// <inheritdoc />
		public string RunPartial(string name, RenderContext context)
		{
			Func<RenderContext, string> func;
			if (_partials.TryGetValue(name, out func))
			{
				return func(context);
			}

			throw new ArgumentException("No partial template called '" + name + "' has been compiled.");
		}

		/// <inheritdoc />
		public string RunHelper(string name, HelperOptions options)
		{
			Func<HelperOptions, string> func;
			if (_helpers.TryGetValue(name, out func))
			{
				return func(options);
			}

			throw new ArgumentException("No helper called '" + name + "' has been registered.");
		}

		/// <summary>
		/// Creates a document <see cref="Block"/> from the given template.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <returns>The document <see cref="Block"/></returns>
		private Block CreateDocument(string template)
		{
			using (var reader = new StringReader(template))
			{
				using (var source = new SeekableTextReader(reader))
				{
					var errors = new ParserErrorSink();
					var parser = new HandlebarsParser();

					var context = new ParserContext(source, parser, errors, TagProviders);
					parser.Context = context;

					parser.ParseDocument();
					var results = context.CompleteParse();

					if (results.Success)
					{
						return results.Document;
					}

					throw new InvalidOperationException(
						string.Join("\n", results.Errors.Select(e => string.Format("{0}: {1}", e.Location, e.Message))));
				}
			}
		}
	}
}