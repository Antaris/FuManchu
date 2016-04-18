﻿namespace FuManchu
{
	using FuManchu.Renderer;

	/// <summary>
	/// Represents a compiled Handlebars partial template.
	/// </summary>
	/// <param name="model">The parent render context.</param>
	/// <returns>The template result.</returns>
	public delegate string HandlebarPartialTemplate(RenderContext context);
}