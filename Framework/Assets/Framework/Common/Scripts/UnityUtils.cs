//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

using UnityEngine;

namespace Common {
	/**
	 * Contains Unity related utility methods
	 */
	public static class UnityUtils {

		/**
		 * Returns a certain component of an object with the specified name
		 */
		public static T GetRequiredComponent<T>(string objectName) where T : Component {
			GameObject go = GameObject.Find(objectName);
			Assertion.AssertNotNull(go);

			T component = go.GetComponent<T>();
			Assertion.AssertNotNull(component);

			return component;
		}

	}
}

