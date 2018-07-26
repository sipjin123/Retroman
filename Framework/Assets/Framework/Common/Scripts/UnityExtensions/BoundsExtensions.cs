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

namespace Common {
	/**
	 * Extensions to Unity's Bounds class
	 */
	public static class BoundsExtensions {

		/**
		 * Returns whether or not the bounds intersects with another bounds but only with x and y axis.
		 */
		public static bool Intersects2D(this Bounds self, Bounds other) {
			// disjoint on x axis
			if(self.max.x < other.min.x || other.max.x < self.min.x) {
				return false;
			}

			// disjoint on y axis
			if(self.max.y < other.min.y || other.max.y < self.min.y) {
				return false;
			}

			return true;
		}

	}
}

