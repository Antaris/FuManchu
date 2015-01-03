namespace FuManchu
{
	/// <summary>
	/// Represents a state machine.
	/// </summary>
	/// <typeparam name="T">The state machine.</typeparam>
	public abstract partial class StateMachine<T>
	{
		protected delegate StateResult State();

		/// <summary>
		/// Gets the start state.
		/// </summary>
		protected abstract State StartState { get; }

		/// <summary>
		/// Gets or sets the current state.
		/// </summary>
		protected State CurrentState { get; set; }

		/// <summary>
		/// Stays this at the current state.
		/// </summary>
		/// <returns>The state result.</returns>
		protected StateResult Stay()
		{
			return new StateResult(CurrentState);
		}

		/// <summary>
		/// Stays this at the current state.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <returns>The state result.</returns>
		protected StateResult Stay(T output)
		{
			return new StateResult(output, CurrentState);
		}

		/// <summary>
		/// Stops this state machine.
		/// </summary>
		/// <returns>The state result.</returns>
		protected StateResult Stop()
		{
			return null;
		}

		/// <summary>
		/// Transitions the specified new state.
		/// </summary>
		/// <param name="newState">The new state.</param>
		/// <returns>The state result.</returns>
		protected StateResult Transition(State newState)
		{
			return new StateResult(newState);
		}

		/// <summary>
		/// Transitions the specified new state.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="newState">The new state.</param>
		/// <returns>The state result.</returns>
		protected StateResult Transition(T output, State newState)
		{
			return new StateResult(output, newState);
		}

		/// <summary>
		/// Advances to the next state.
		/// </summary>
		/// <returns>The output.</returns>
		protected virtual T Turn()
		{
			if (CurrentState != null)
			{
				StateResult result;
				do
				{
					result = CurrentState();
					if (result == null)
					{
						break;
					}
					CurrentState = result.Next;
				}
				while (!result.HasOutput);

				if (result == null)
				{
					return default(T);
				}

				return result.Output;
			}

			return default(T);
		}
	}
}