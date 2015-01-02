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
		/// <param name="template">The handlebars template.</param>
		/// <returns>The compiled template as an executable delegate.</returns>
		Func<object, string> Compile(string template);

		void RegisterPartial(string name, string template);

		string RunPartial(string name, RenderContext context);
	}
}