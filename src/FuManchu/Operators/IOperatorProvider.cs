namespace FuManchu
{
	using System.Collections.Generic;

	/// <summary>
	/// Defines the required contract for implementing an operator provider.
	/// </summary>
	public interface IOperatorProvider
	{
		/// <summary>
		/// Gets the available operators.
		/// </summary>
		/// <returns>The set of operators.</returns>
		IEnumerable<IOperator> GetOperators();
	}
}