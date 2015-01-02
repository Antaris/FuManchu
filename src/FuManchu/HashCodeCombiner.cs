namespace FuManchu
{
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Provides services for combining hashcodes.
	/// </summary>
	internal class HashCodeCombiner
	{
		private long _combinedHash64 = 0x1505L;

		/// <summary>
		/// Gets the combined hash.
		/// </summary>
		public int CombinedHash
		{
			get { return _combinedHash64.GetHashCode(); }
		}

		/// <summary>
		/// Creates a new instance of <see cref="HashCodeCombiner"/>
		/// </summary>
		/// <returns>The combiner instance.</returns>
		public static HashCodeCombiner Start()
		{
			return new HashCodeCombiner();
		}

		/// <summary>
		/// Adds the hashcode of items in the given set.
		/// </summary>
		/// <param name="e">The enumerable set.</param>
		/// <returns>The current combiner.</returns>
		public HashCodeCombiner Add(IEnumerable e)
		{
			if (e == null)
			{
				Add(0);
			}
			else
			{
				int count = 0;
				foreach (object o in e)
				{
					Add(0);
					count++;
				}
				Add(count);
			}
			return this;
		}

		/// <summary>
		/// Adds the hashcode of the given integer.
		/// </summary>
		/// <param name="i">The integer.</param>
		/// <returns>The current combiner.</returns>
		public HashCodeCombiner Add(int i)
		{
			_combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;

			return this;
		}

		/// <summary>
		/// Adds the hashcode of the given object.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <returns>The current combiner.</returns>
		public HashCodeCombiner Add(object o)
		{
			var hashcode = (o != null) ? o.GetHashCode() : 0;
			Add(hashcode);
			return this;
		}

		/// <summary>
		/// Adds the hashcode of the specified value.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="comparer">The comparer.</param>
		/// <returns>The current combiner.</returns>
		public HashCodeCombiner Add<T>(T value, IEqualityComparer<T> comparer)
		{
			return Add(comparer.GetHashCode(value));
		}
	}
}