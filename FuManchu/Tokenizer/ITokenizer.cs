namespace FuManchu.Tokenizer
{
	/// <summary>
	/// Defines the required contract for implementing a tokenizer.
	/// </summary>
	public interface ITokenizer
	{
		/// <summary>
		/// Gets the next symbol.
		/// </summary>
		/// <returns>The symbol.</returns>
		ISymbol NextSymbol();
	}
}