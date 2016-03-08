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

namespace Common {
	/**
	 * A common interface to process a certain type.
	 * Used to avoid using closures which uses memory
	 */
	public interface Processor<T> {

		/**
		 * Processes the instance. Returns true if processor is already done with the process. Otherwise it should return false.
		 */
		bool Process(T instance);

	}
}

