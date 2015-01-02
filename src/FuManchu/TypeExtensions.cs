namespace FuManchu
{
	using System;
	using System.Linq;

	/// <summary>
	/// Provides extension methods for common type operations.
	/// </summary>
	internal static class TypeExtensions
	{
		/// <summary>
		/// Extracts the generic interface matching the interface type.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="interfaceType">The interface type.</param>
		/// <returns>The closed generic interface type.</returns>
		public static Type ExtractGenericInterface(this Type queryType, Type interfaceType)
		{
			Func<Type, bool> matchesInterface =
				t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType;
			return (matchesInterface(queryType)) ?
				queryType :
				queryType.GetInterfaces().FirstOrDefault(matchesInterface);
		}

		/// <summary>
		/// Gets whether the given type is a nullable value type.
		/// </summary>
		/// <param name="type">The type instance.</param>
		/// <returns>True if the type is a nullable value type, otherwise false.</returns>
		public static bool IsNullableValueType(this Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}
	}
}