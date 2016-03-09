//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

using UnityEngine;

using Common.Utils;

namespace Common.Signal {
	/**
	 * A simple class that can dispatch actions to listeners
	 */
	public class Signal {

		private readonly string name;

		private ConcreteSignalParameters parameters;

		public delegate void SignalListener(ISignalParameters parameters);

		private SimpleList<SignalListener> listenerList;

		/**
		 * Constructor
		 */
		public Signal(string name) {
			this.name = name;
			this.listenerList = new SimpleList<SignalListener>();
		}

		/**
		 * Clears the parameters of the signal
		 */
		public void ClearParameters() {
			if(this.parameters == null) {
				// not yet initialized
				// note that we lazy initialize parameters to save memory
				return;
			}

			this.parameters.Clear();
		}

		/**
		 * Adds a parameter to the signal
		 */
		public void AddParameter(string key, object value) {
			if(this.parameters == null) {
				this.parameters = new ConcreteSignalParameters();
			}

			this.parameters.AddParameter(key, value);
		}

		/**
		 * Adds a listener
		 */
		public void AddListener(SignalListener listener) {
			this.listenerList.Add(listener);
		}

		/**
		 * Removes the specified listener
		 */
		public void RemoveListener(SignalListener listener) {
			this.listenerList.Remove(listener);
		}

		/**
		 * Checks if the signals has a listener registered
		 */
		public bool HasListener() {
			return this.listenerList.Count > 0;
		}

		/**
		 * Checks if the signals has the specified listener registered
		 */
		public bool HasListener(SignalListener listener) {
			return this.listenerList.Contains(listener);
		}

		/**
		 * Dispatches the signal thus invoking its listeners
		 */
		public void Dispatch() {
			if(this.listenerList.Count == 0) {
				Debug.LogWarning("There are no listeners to the signal: " + this.name);
			}

			for(int i = 0; i < this.listenerList.Count; ++i) {
				// invoke the listener
				this.listenerList[i](this.parameters); // note that the parameters passed may be null if there was none specified
			}
		}

		public string Name {
			get {
				return name;
			}
		}

	}
}