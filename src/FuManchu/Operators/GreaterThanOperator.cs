namespace FuManchu
{
	using System;

	/// <summary>
	/// Performs a greater-than operation.
	/// </summary>
	public class GreaterThanOperator : OperatorBase
	{
		private readonly Func<int, int, bool> _operation;

		/// <summary>
		/// Initialises a new instance of <see cref="GreaterThanOperator"/>
		/// </summary>
		public GreaterThanOperator(bool greaterThanEqualTo = false)
			: base(greaterThanEqualTo ? ">=" : ">")
		{
			_operation = greaterThanEqualTo
				? (Func<int, int, bool>)((x, y) => x >= y)
				: (x, y) => x > y;
		}

		/// <inheritdoc />
		public override bool Result(object x, object y)
		{
			if (x == null || y == null)
			{
				return false;
			}

			var xType = x.GetType();
			var yType = y.GetType();

			if (xType.IsValueType && yType.IsValueType)
			{
				return _operation((int)x, (int)y);
			}

			if (xType.IsValueType)
			{
				try
				{
					return _operation((int)x, (int)Convert.ChangeType(y, typeof(int)));
				}
				catch
				{
					return false;
				}
			}

			try
			{
				var xNumeric = (int)Convert.ChangeType(x, typeof(int));
				var yNumeric = (int)Convert.ChangeType(y, typeof(int));
				return _operation(xNumeric, yNumeric);
			}
			catch
			{
				return false;
			}

		}
	}
}