namespace FuManchu
{
	using System.Collections.Generic;
	using System.Linq;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tokenizer;

	/// <summary>
	/// Provides a base implementation of a context.
	/// </summary>
	/// <typeparam name="T">The context type.</typeparam>
	public abstract class ContextBase<T> where T : ContextBase<T>
	{
		private readonly IDictionary<string, object> _variables = new Dictionary<string, object>(); 

		/// <summary>
		/// Initialises a new instance of <see cref="ContextBase{T}"/>
		/// </summary>
		/// <param name="visitor">The parser visitor.</param>
		/// <param name="parentContext">The parent context.</param>
		protected ContextBase(ParserVisitor<T> visitor, T parentContext = null)
		{
			Visitor = visitor;
			ParentContext = parentContext;
		}

		/// <summary>
		/// Gets or sets the model metadata provider.
		/// </summary>
		public IModelMetadataProvider ModelMetadataProvider { get; set; }

		/// <summary>
		/// Gets or sets the Handlebars service.
		/// </summary>
		public IHandlebarsService Service { get; set; }

		/// <summary>
		/// Gets or sets the template data.
		/// </summary>
		public TemplateData TemplateData { get; set; }

		/// <summary>
		/// Gets the parent render context.
		/// </summary>
		public T ParentContext { get; private set; }

		/// <summary>
		/// Gets the root render context.
		/// </summary>
		public T RootContext { get; internal set; }

		/// <summary>
		/// Gets the parser visitor.
		/// </summary>
		public ParserVisitor<T> Visitor { get; private set; }

		/// <summary>
		/// Creates a child context.
		/// </summary>
		/// <param name="model">The input model for the scope.</param>
		/// <returns>The child context.</returns>
		protected abstract T CreateChildContext(object model);

		/// <summary>
		/// Begins a child scope given the specified model.
		/// </summary>
		/// <returns>The disposable used to revert the scope.</returns>
		public ContextScope<T> BeginScope(object model)
		{
			var context = CreateChildContext(model);
			Visitor.SetScope(context);

			return new ContextScope<T>(context, () => Visitor.RevertScope());
		}

		/// <summary>
		/// Gets the variable with the given name.
		/// </summary>
		/// <param name="name">The variable name.</param>
		/// <returns>The variable value.</returns>
		public object GetVariable(string name, object @default = null)
		{
			object value;
			if (_variables.TryGetValue(name, out value))
			{
				return value;
			}

			return @default;
		}

		/// <summary>
		/// Resolves the value represented by the given span.
		/// </summary>
		/// <param name="span">The span representing the expression.</param>
		/// <returns>The resolved value.</returns>
		public object ResolveValue(Span span)
		{
			IEnumerable<ISymbol> symbols;

			switch (span.Kind)
			{
				case SpanKind.Expression:
				case SpanKind.Parameter:
					{
						symbols = span.Symbols;
						break;
					}
				case SpanKind.Map:
					{
						symbols = span.Symbols.Skip(2);
						break;
					}
				default:
					{
						return TemplateData.Model;
					}
			}

			return ResolveValueFromSymbols(symbols.ToArray());
		}

		/// <summary>
		/// Resolves the value represented by the given set of symbols.
		/// </summary>
		/// <param name="symbols">The set of symbols.</param>
		/// <returns>The resolved value.</returns>
		private object ResolveValueFromSymbols(ISymbol[] symbols)
		{
			if (symbols.Length == 0)
			{
				return TemplateData.Model;
			}

			var context = this;
			var templateData = TemplateData;

			string expression = string.Empty;
			bool isVariableLookup = false;

			foreach (HandlebarsSymbol symbol in symbols)
			{
				switch (symbol.Type)
				{
					case HandlebarsSymbolType.At:
						{
							isVariableLookup = true;

							break;
						}

					case HandlebarsSymbolType.CurrentContext:
						{
							// Ensure the model property is set.
							templateData = TemplateData;

							if (templateData == null)
							{
								return null;
							}

							break;
						}

					case HandlebarsSymbolType.ParentContext:
						{
							context = context.ParentContext;
							if (context == null)
							{
								// We couldn't reach the parent context.
								return null;
							}

							// Set the model to be that of the parent context.
							templateData = context.TemplateData;

							break;
						}

					case HandlebarsSymbolType.IntegerLiteral:
					case HandlebarsSymbolType.RealLiteral:
						{
							return symbol.Content;
						}

					case HandlebarsSymbolType.StringLiteral:
						{
							return symbol.Content.Substring(1, symbol.Content.Length - 2);
						}

					case HandlebarsSymbolType.Identifier:
					case HandlebarsSymbolType.Keyword:
					case HandlebarsSymbolType.Dot:
					case HandlebarsSymbolType.LeftBracket:
					case HandlebarsSymbolType.RightBracket:
						{
							if (symbol.Content == "this" && expression.Length == 0)
							{
								// Ensure the model property is set.
								templateData = TemplateData;

								if (templateData == null)
								{
									return null;
								}

								break;
							}

							expression += symbol.Content;
							break;
						}
					case HandlebarsSymbolType.Slash:
						{
							expression += ".";
							break;
						}
				}
			}

			if (expression.StartsWith("."))
			{
				// Skip the leading '.'
				expression = expression.Substring(1);
			}

			return ResolveValue((T)context, templateData, expression, isVariableLookup);
		}

		/// <summary>
		/// Resolves the value for the given expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="isVariableLookup">True if this is a variable lookup, otherwise false.</param>
		/// <returns>The resolved value.</returns>
		public object ResolveValue(string expression, bool isVariableLookup)
		{
			return ResolveValue((T)this, TemplateData, expression, isVariableLookup);
		}

		/// <summary>
		/// Resolves the value for the given expression.
		/// </summary>
		/// <param name="context">The render context.</param>
		/// <param name="templateData">The template data.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="isVariableLookup">True if this is a variable lookup, otherwise false.</param>
		/// <returns>The resolved value.</returns>
		public static object ResolveValue(T context, TemplateData templateData, string expression, bool isVariableLookup)
		{
			if (isVariableLookup && !string.IsNullOrEmpty(expression) && !expression.StartsWith("root."))
			{
				return context.GetVariable(expression);
			}

			if (isVariableLookup && !string.IsNullOrEmpty(expression) && expression.StartsWith("root."))
			{
				context = context.RootContext ?? context;
				templateData = context.TemplateData;
				expression = expression.Substring(5);
			}

			var modelMetadata = ExpressionMetadataProvider.FromStringExpression(expression, templateData, context.ModelMetadataProvider);
			if (modelMetadata == null)
			{
				return null;
			}
			return modelMetadata.Model;
		}

		/// <summary>
		/// Sets the variable with the given name.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The variable value.</param>
		public void SetVariable(string name, object value)
		{
			_variables[name] = value;
		}
	}
}