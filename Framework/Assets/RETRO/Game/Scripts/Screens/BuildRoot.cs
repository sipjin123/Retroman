using UnityEngine;
using System.Collections;

namespace Synergy88 {
		
	public class BuildRoot : MonoBehaviour {

		private void Start() {
			S88Signals.ON_LOAD_SPLASH.Dispatch();
		}

	}

}