namespace FuManchu
{
	using System;
	using FuManchu.Renderer;

	/// <summary>
	/// Defines the required contract for implementing a Handlebars service.
	/// </summary>
	public interface IHandlebarsService
	{
		/// <summary>
		/// Compiles a Handlebars template
		/// </summary>
		/// <param name="name">The name of the template.</param>
		/// <param name="template">The handlebars template.</param>
		/// <returns>The compiled template as an executable delegate.</returns>
		Func<object, string> Compile(string name, string template);

		/// <summary>
		/// Compiles and runs the given Handlebars template.
		/// </summary>
		/// <param name="name">The name of the template.</param>
		/// <param name="template">The handlebars template.</param>
		/// <param name="model">The model for the template.</param>
		/// <returns>The template result.</returns>
		string CompileAndRun(string name, string template, object model = null);

		/// <summary>
		/// Registers a partial template with the given name.
		/// </summary>
		/// <param name="name">The name of the partial template.</param>
		/// <param name="template">The partial template content.</param>
		void RegisterPartial(string name, string template);

		/// <summary>
		/// Runs a pre-compiled template.
		/// </summary>
		/// <param name="name">The name of the template.</param>
		/// <param name="model">The model for the template.</param>
		/// <returns>The template result.</returns>
		string Run(string name, object model = null);

		/// <summary>
		/// Runs a pre-compiled partial template.
		/// </summary>
		/// <param name="name">The name of the partial template.</param>
		/// <param name="context">The render context.</param>
		/// <returns>The template result.</returns>
		string RunPartial(string name, RenderContext context);
	}
}