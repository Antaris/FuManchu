namespace FuManchu.Text
{
	using System;
	using System.IO;

	/// <summary>
	/// Provides a base implementation of a text reader that supports lookahead operations.
	/// </summary>
	public abstract class LookaheadTextReader : TextReader
	{
		/// <summary>
		/// Gets the current location.
		/// </summary>
		public abstract SourceLocation CurrentLocation { get; }

		/// <summary>
		/// Begins a lookahead operation.
		/// </summary>
		/// <returns>The disposable tracking object used to accept or reject the lookahead operation.</returns>
		public abstract IDisposable BeginLookahead();

		/// <summary>
		/// Cancels the backtrack operation.
		/// </summary>
		public abstract void CancelBacktrack();
	}
}