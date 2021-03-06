﻿using System;
using System.Linq;
using System.Threading;

namespace ATTi.Core
{
	/// <summary>
	/// Utility structure for performing and tracking threadsafe state transitions.
	/// </summary>
	/// <typeparam name="E">State type E (should be an enum)</typeparam>
	public struct Status<E>
	{
		#region Fields

		int _status;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="initialialState">Initial state</param>
		public Status(E initialialState)
		{
			_status = Convert.ToInt32(initialialState);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Accesses the current state.
		/// </summary>
		public E CurrentState
		{
			get { return (E)Enum.ToObject(typeof(E), Thread.VolatileRead(ref _status)); }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Determines if the current state is greater than the comparand.
		/// </summary>
		/// <param name="comparand">comparand</param>
		/// <returns><em>true</em> if the current state is greater than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool IsGreaterThan(E comparand)
		{
			return Thread.VolatileRead(ref _status) > Convert.ToInt32(comparand);
		}

		/// <summary>
		/// Determines if the current state is less than the comparand.
		/// </summary>
		/// <param name="comparand">comparand</param>
		/// <returns><em>true</em> if the current state is less than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool IsLessThan(E comparand)
		{
			return Thread.VolatileRead(ref _status) < Convert.ToInt32(comparand);
		}

		/// <summary>
		/// Transitions to the given state.
		/// </summary>
		/// <param name="value">the target state</param>
		public void SetState(E value)
		{
			Thread.VolatileWrite(ref _status, Convert.ToInt32(value));
		}

		/// <summary>
		/// Performs a state transition if the current state compares greater than the <paramref name="comparand"/>
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state</param>
		/// <returns><em>true</em> if the current state compares greater than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool SetStateIfGreaterThan(E value, E comparand)
		{
			int c = Convert.ToInt32(comparand);
			int v = Convert.ToInt32(value);

			int init, fin = Thread.VolatileRead(ref _status);
			while (true)
			{
				if (fin < c) return false;

				init = fin;
				fin = Interlocked.CompareExchange(ref _status, v, init);
				if (fin == init) return true;
			}
		}

		/// <summary>
		/// Performs a state transition if the current state compares less than the <paramref name="comparand"/>
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state</param>
		/// <returns><em>true</em> if the current state compares less than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool SetStateIfLessThan(E value, E comparand)
		{
			int c = Convert.ToInt32(comparand);
			int v = Convert.ToInt32(value);

			int init, fin = Thread.VolatileRead(ref _status);
			while (true)
			{
				if (fin > c) return false;

				init = fin;
				fin = Interlocked.CompareExchange(ref _status, v, init);
				if (fin == init) return true;
			}
		}

		/// <summary>
		/// Performs a state transition if the current state compares less than the <paramref name="comparand"/>
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state</param>
		/// <param name="actionOnSuccess">An action to be performed if the state transition succeeds</param>
		/// <returns><em>true</em> if the current state compares less than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool SetStateIfLessThan(E value, E comparand, Action actionOnSuccess)
		{
			if (actionOnSuccess == null) throw new ArgumentNullException("actionOnSuccess");
			if (SetStateIfLessThan(value, comparand))
			{
				actionOnSuccess();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Toggles between the toggle state and the desired state - with
		/// a spin-wait if necessary.
		/// </summary>
		/// <param name="desired">desired state</param>
		/// <param name="toggle">state from which the desired state can toggle</param>
		/// <returns><em>true</em> if the state transitions to the desired state from the toggle state; otherwise <em>false</em></returns>
		public bool SpinToggleState(E desired, E toggle)
		{
			int d = Convert.ToInt32(desired);
			int t = Convert.ToInt32(toggle);

		spin:
			int r = Interlocked.CompareExchange(ref _status, d, t);
			// If the state was the toggle state then we're successful and done...
			if (r == t) return true;
			// otherwise if the result is anything but the desired state we're
			// unsuccessful and done...
			if (r != d) return false;
			// otherwise we spin
			goto spin;
		}

		/// <summary>
		/// Perfroms a spinwait until the current state equals the target state.
		/// </summary>
		/// <param name="targetState">the target state</param>
		/// <param name="loopAction">An action to perform inside the spin cycle</param>
		public void SpinWaitForState(E targetState, Action loopAction)
		{
			int state = Convert.ToInt32(targetState);
			while (Thread.VolatileRead(ref _status) != state)
			{
				loopAction();
			}
		}

		/// <summary>
		/// Performs a spinwait until the current state equals the target state.
		/// </summary>
		/// <param name="targetState">the target state</param>
		/// <param name="loopAction">An action to perform inside the spin cycle; 
		/// waiting continues until either the target state is reached or the loop
		/// action returns false.</param>
		/// <returns><em>true</em> if the target state was reached; otherwise <em>false</em>.</returns>
		public bool TrySpinWaitForState(E targetState, Func<E, bool> loopAction)
		{
			int target = Convert.ToInt32(targetState);
			int current;
			while ((current = Thread.VolatileRead(ref _status)) != target)
			{
				if (!loopAction((E)Enum.ToObject(typeof(E), current)))
				{ 
					// loop signaled to stop waiting...
					return false;
				}
			}
			// wait completed, state reached...
			return true;
		}

		/// <summary>
		/// Tries to transition the state
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state must match current state</param>
		/// <returns><em>true</em> if the current state matches <paramref name="comparand"/> and the state is transitioned to <paramref name="value"/>; otherwise <em>false</em></returns>
		public bool TryTransition(E value, E comparand)
		{
			int c = Convert.ToInt32(comparand);
			return Interlocked.CompareExchange(ref _status, Convert.ToInt32(value), c) == c;
		}

		public bool TryTransition(E value, params E[] comparand)
		{
			var comparands = (from c in comparand
											 select Convert.ToInt32(c)).ToArray();
			var v = Convert.ToInt32(value);

			var state = Thread.VolatileRead(ref _status);
			while (comparands.Contains(state))
			{
				if (Interlocked.CompareExchange(ref _status, v, state) == state)
					return true;
				state = Thread.VolatileRead(ref _status);
			}

			return false;
		}

		/// <summary>
		/// Tries to transition the state. Upon success executes the action given.
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state must match current state</param>
		/// <param name="actionOnSuccess">action to perform if the state transition is successful</param>
		/// <returns><em>true</em> if the current state matches <paramref name="comparand"/> and the state is transitioned to <paramref name="value"/>; otherwise <em>false</em></returns>
		public bool TryTransition(E value, E comparand, Action actionOnSuccess)
		{
			if (actionOnSuccess == null) throw new ArgumentNullException("actionOnSuccess");
			if (TryTransition(value, comparand))
			{
				actionOnSuccess();
				return true;
			}
			return false;
		}

		public E CompareExchange(E value, E comparand)
		{
			return
				(E)Enum.ToObject(typeof(E), Interlocked.CompareExchange(ref _status, Convert.ToInt32(value), Convert.ToInt32(comparand)));
			;
		}

		#endregion Methods

		public bool HasState(E value)
		{
			int v = Convert.ToInt32(value);
			return (_status & v) == v;
		}
	}
}
