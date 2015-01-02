namespace FuManchu.Text
{
	/// <summary>
	/// Defines the required contract for implementing an encoded string.
	/// </summary>
	public interface IEncodedString
	{
		/// <summary>
		/// Returns the encoded string.
		/// </summary>
		/// <returns>The encoded string.</returns>
		string ToEncodedString();
	}
}