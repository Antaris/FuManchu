namespace FuManchu
{
	/// <summary>
	/// Represents a compiled Handlebars helper.
	/// </summary>
	/// <param name="model">The helper options.</param>
	/// <returns>The helper result.</returns>
	public delegate string HandlebarHelper(HelperOptions options);
}
