namespace FuManchu
{
	/// <summary>
	/// Handles the resolution of unknown values.
	/// </summary>
	/// <param name="expression">The expression.</param>
	/// <returns>The value result.</returns>
	public delegate object UnknownValueResolver(string expression);
}
