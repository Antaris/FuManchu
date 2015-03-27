namespace FuManchu
{
	using System;

	/// <summary>
	/// Represents a scope around a context.
	/// </summary>
	/// <typeparam name="T">The context type.</typeparam>
	public class ContextScope<T> : DisposableAction
	{
		/// <summary>
		/// Initialises a new instance of <see cref="ContextScope{T}" />.
		/// </summary>
		/// <param name="context">The scoped context.</param>
		/// <param name="action">The action to execute to release the context.</param>
		public ContextScope(T context, Action action) : base(action)
		{
			Context = context;
		}

		/// <summary>
		/// Gets or sets the scoped context.
		/// </summary>
		public T Context { get; private set; }
	}
}