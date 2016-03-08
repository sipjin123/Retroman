using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Common.Signal;

namespace Synergy88 {

	public class SystemVersion : MonoBehaviour {

		[SerializeField]
		private string version;

		[SerializeField]
		private Text labelVersion;

		private void Awake() {
			Assertion.AssertNotNull(this.labelVersion);
		}

		private void Start() {
			this.labelVersion.text = this.version;
		}

	}

}