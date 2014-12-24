namespace FuManchu.Binding
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Threading;

	public static class TryGetValueProvider
	{
		private static readonly Dictionary<Type, TryGetValueDelegate> _tryGetValueDelegateCache = new Dictionary<Type, TryGetValueDelegate>();
		private static readonly ReaderWriterLockSlim _tryGetValueDelegateCacheLock = new ReaderWriterLockSlim();

		private static readonly MethodInfo _strongTryGetValueImplInfo = typeof(TryGetValueProvider).GetTypeInfo().GetDeclaredMethod("StrongTryGetValueImpl");

		public static TryGetValueDelegate CreateInstance(Type targetType)
		{
			TryGetValueDelegate result;

			_tryGetValueDelegateCacheLock.EnterReadLock();
			try
			{
				if (_tryGetValueDelegateCache.TryGetValue(targetType, out result))
				{
					return result;
				}
			}
			finally
			{
				_tryGetValueDelegateCacheLock.ExitReadLock();
			}

			var dictionaryType = targetType.ExtractGenericInterface(typeof(IDictionary<,>));

			if (dictionaryType != null)
			{
				var typeArguments = dictionaryType.GetGenericArguments();
				var keyType = typeArguments[0];
				var returnType = typeArguments[1];

				if (keyType.IsAssignableFrom(typeof(string)))
				{
					var implemenationMethod = _strongTryGetValueImplInfo.MakeGenericMethod(keyType, returnType);
					result = (TryGetValueDelegate)implemenationMethod.CreateDelegate(typeof(TryGetValueDelegate));
				}
			}

			if (result == null && typeof(IDictionary).IsAssignableFrom(targetType))
			{
				result = TryGetValueFromNonGenericDictionary;
			}

			_tryGetValueDelegateCacheLock.EnterWriteLock();
			try
			{
				_tryGetValueDelegateCache[targetType] = result;
			}
			finally
			{
				_tryGetValueDelegateCacheLock.ExitWriteLock();
			}

			return result;
		}

		private static bool StrongTryGetValueImpl<TKey, TValue>(object dictionary, string key, out object value)
		{
			var strongDict = (IDictionary<TKey, TValue>)dictionary;

			TValue strongValue;
			var success = strongDict.TryGetValue((TKey)(object)key, out strongValue);
			value = strongValue;
			return success;
		}

		private static bool TryGetValueFromNonGenericDictionary(object dictionary, string key, out object value)
		{
			var weakDict = (IDictionary)dictionary;

			var success = weakDict.Contains(key);
			value = success ? weakDict[key] : null;
			return success;
		}
	}

	public delegate bool TryGetValueDelegate(object dictionary, string key, out object value);
}