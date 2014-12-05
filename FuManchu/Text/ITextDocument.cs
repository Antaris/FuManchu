namespace FuManchu.Text
{
	/// <summary>
	/// Represents a text document.
	/// </summary>
	public interface ITextDocument : ITextBuffer
	{
		/// <summary>
		/// Gets the current location in the document.
		/// </summary>
		SourceLocation Location { get; }
	}
}