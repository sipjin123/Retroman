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

using UnityEngine;

namespace Common.Utils {
	/**
	 * A serialized key-value holder
	 */
	[Serializable]
	public class SerializedKeyValue<K, V> {

		[SerializeField]
		private K key;

		[SerializeField]
		private V value;

		public SerializedKeyValue() {
		}

		public K Key {
			get {
				return key;
			}
		}

		public V Value {
			get {
				return value;
			}
		}

	}
}

