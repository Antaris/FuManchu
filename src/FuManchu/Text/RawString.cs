namespace FuManchu.Text
{
	/// <summary>
	/// Represents a raw string that doesn't support encoding.
	/// </summary>
	public class RawString : IEncodedString
	{
		private readonly object _value;

		/// <summary>
		/// Initialises a new instance of <see cref="RawString"/>
		/// </summary>
		/// <param name="value">The value.</param>
		public RawString(object value)
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