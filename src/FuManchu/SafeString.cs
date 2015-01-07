namespace FuManchu
{
	using FuManchu.Text;

	/// <summary>
	/// Represents a safe string that doesn't support encoding, it is already considered to be encoded.
	/// </summary>
	public class SafeString : IEncodedString
	{
		private readonly object _value;

		/// <summary>
		/// Initialises a new instance of <see cref="SafeString"/>
		/// </summary>
		/// <param name="value">The value.</param>
		public SafeString(object value)
		{
			_value = value;
		}

		/// <inheritdoc />
		public string ToEncodedString()
		{
			if (_value == null)
			{
				return string.Empty;
			}

			return _value.ToString();
		}
	}
}