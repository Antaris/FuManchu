namespace FuManchu 
{
	/// <summary>
	/// Initialises a new instance of <see cref="IOperator"/>
	/// </summary>
	public interface IOperator
	{
		/// <summary>
		/// Gets the name of the operator.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the result of the operator
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>The boolean result of the operator.</returns>
		bool Result(object x, object y);
	}
}