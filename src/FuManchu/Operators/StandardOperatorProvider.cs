namespace FuManchu
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents the standard set of operators.
	/// </summary>
	public class StandardOperatorProvider : IOperatorProvider
	{
		/// <inheritdoc />
		public IEnumerable<IOperator> GetOperators()
		{
			yield return new EqualityOperator();				// ==
			yield return new EqualityOperator(true);			// ===
			yield return new NegatedEqualityOperator();			// !=
			yield return new NegatedEqualityOperator("not");	// not
			yield return new NegatedEqualityOperator(true);		// !==
			yield return new GreaterThanOperator();				// > 
			yield return new GreaterThanOperator(true);			// >=
		}
	}
}