namespace FuManchu
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FuManchu.Renderer;

	/// <summary>
	/// Represents options passed to Handlebars helpers.
	/// </summary>
	public class HelperOptions
	{
		/// <summary>
		/// Gets the set of input arguments.
		/// </summary>
		public object[] Arguments { get; set; }

		/// <summary>
		/// Gets the input argument. This property exists for API compatability with HandlebarsJS
		/// </summary>
		public dynamic Data { get { return Arguments.FirstOrDefault(); } }

		/// <summary>
		/// Gets the parameters collection. This property exists for API compatability with HandlebarsJS
		/// </summary>
		public IDictionary<string, object> Hash { get { return Parameters; } }

		/// <summary>
		/// Gets the render function. This property exists for API compatability with HandlebarsJS
		/// </summary>
		public Func<object, string> Fn { get { return Render; } } 

		/// <summary>
		/// Gets the parameters collection.
		/// </summary>
		public IDictionary<string, object> Parameters { get; set; }

		/// <summary>
		/// Gets or sets the render function.
		/// </summary>
		public Func<object, string> Render { get; set; } 

		/// <summary>
		/// Gets or sets the render context.
		/// </summary>
		public RenderContext RenderContext { get; set; }
	}
}