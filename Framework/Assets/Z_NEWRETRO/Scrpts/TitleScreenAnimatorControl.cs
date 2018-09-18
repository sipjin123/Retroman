using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UniRx;

using Common;
using Common.Query;

using Framework;
using Common.Utils;
using System.Collections;
using Sandbox.ButtonSandbox;

namespace  Retroman
{
	

	public class TitleScreenAnimatorControl : MonoBehaviour 
	{
		
		[SerializeField]
		Animator TitleScreenAnimator;

		void Awake()
		{
			TitleScreenAnimator = GetComponent<Animator>();
			InitializeSignals();
		}

		void InitializeSignals()
		{
			Factory.Get<DataManagerService>().MessageBroker.Receive<LaunchGamePlay>().Subscribe(_ =>
        		{	
					TitleScreenAnimator.SetTrigger("Outro");

        		})
				.AddTo(this);
		

		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}


}