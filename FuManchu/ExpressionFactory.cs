namespace FuManchu
{
	using System;
	using System.Linq.Expressions;

	/// <summary>
	/// Provides factory metods for creating executable expressions for strings.
	/// </summary>
	public static class ExpressionFactory
	{
		/// <summary>
		/// Generates a <see cref="LambdaExpression"/> for the given string expression.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>The lambda expression.</returns>
		public static LambdaExpression ForStringExpression(Type modelType, string expression)
		{
			if (modelType == null)
			{
				throw new ArgumentNullException("modelType");
			}

			var param = Expression.Parameter(modelType);
			Expression root = param;

			if (!string.IsNullOrWhiteSpace(expression))
			{
				foreach (var member in expression.Split('.'))
				{
					root = Expression.PropertyOrField(root, member);
				}
			}

			return Expression.Lambda(root, param);
		}
	}
}