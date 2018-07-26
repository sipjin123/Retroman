using UnityEngine;

using System;
using System.Collections;

using Common;
using Common.Query;
using Common.Signal;
using Framework;

namespace Synergy88 {

	public class LoginRoot : Scene {

		public GameObject _gameRootObject;
		public GameObject _gameRootObject2;
		protected override void Awake() {
			base.Awake();
		}

		protected override void Start() {
			base.Start();

            //

            Debug.LogError("LOGIN BUTTON");
            /*
			this.AddButtonHandler(EButtonType.Login, (ISignalParameters parameters) => {
				this.Login();
				SoundControls.Instance._buttonClick.Play();
			});

			this.AddButtonHandler(EButtonType.LoginFacebook, (ISignalParameters parameters) => {
				S88Signals.FACEBOOK_LOGIN_REQUESTED.Dispatch();
			});*/
		}

		protected override void OnEnable() {
			base.OnEnable();

			S88Signals.ON_FACEBOOK_LOGIN_SUCCESSFUL.AddListener(this.OnFacebookLoginSuccessful);
			S88Signals.ON_FACEBOOK_LOGIN_FAILED.AddListener(this.OnFacebookLoginFailed);
		}

		protected override void OnDisable() {
			base.OnDisable();

			S88Signals.ON_FACEBOOK_LOGIN_SUCCESSFUL.RemoveListener(this.OnFacebookLoginSuccessful);
			S88Signals.ON_FACEBOOK_LOGIN_FAILED.RemoveListener(this.OnFacebookLoginFailed);
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}

		private void Login() {
			// load preloader here
			S88Signals.ON_LOGIN_DONE.Dispatch();
			_gameRootObject.SetActive(true);
			_gameRootObject2.SetActive(true);
		}

		#region Signals

		private void OnFacebookLoginSuccessful(ISignalParameters parameters) {
			//string fbId = (string)QuerySystem.Query<string>(QueryIds.UserFacebookId);
			//string fbEmail = (string)QuerySystem.Query<string>(QueryIds.UserEmail);
			//string fbFullname = (string)QuerySystem.Query<string>(QueryIds.UserFullName);
			//string fbProfilePhoto = (string)QuerySystem.Query<string>(QueryIds.UserProfilePhoto);
			this.Login();
		}

		private void OnFacebookLoginFailed(ISignalParameters parameters) {
			
		}

		#endregion
	}

}