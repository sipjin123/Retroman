using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using Facebook.Unity;
using MiniJSON;

using Common.Fsm;
using Common.Query;
using Common.Signal;

namespace Synergy88 {
	
	public class FacebookLogin : MonoBehaviour {
		/*
		// Facebook Keys
		private static readonly string FB_ID_KEY = "id";
		private static readonly string FB_NAME_KEY = "name";
		private static readonly string FB_EMAIL = "email";
		private static readonly string FB_PROFILE_PHOTO = "picture";
		private static readonly string FB_PROFILE_PHOTO_DATA = "data";
		private static readonly string FB_PROFILE_PHOTO_URL = "url";

		// Prefs Keys
		private static readonly string FACEBOOK_EMAIL_KEY = "facebookEmailKey";
		private static readonly string FACEBOOK_ID_KEY = "facebookIdKey";
		private static readonly string FACEBOOK_FULL_NAME = "facebookFullname";
		private static readonly string FACEBOOK_PROFILE_PHOTO = "facebookProfilePhoto";

		private Fsm fsm;

		[SerializeField]
		//private string fbId = "10207031375695482";
		private string fbId = string.Empty;

		[SerializeField]
		//private string fullName = "Aries Sanchez Sulit";
		private string fullName = string.Empty;

		[SerializeField]
		private string profilePhoto = string.Empty;

		[SerializeField]
		//private string email = "aries.sulit@email.com";
		private string email = string.Empty;
		
		[SerializeField]
		private List<string> permissions;
		[SerializeField]
		private List<Permission> grantedPermissions;
		private Dictionary<string, bool> grantedPermissionMap;

		// permissions
		[SerializeField]
		private readonly List<string> PERMISSIONS = new List<string>(){
			"public_profile", 
			"email", 
			"user_friends"
		};
		
		private void Awake() {
			// init permissions
			this.permissions = new List<string>();
			this.grantedPermissions = new List<Permission>();
			this.grantedPermissionMap = new Dictionary<string, bool>();

			Func<string, string> GetStoredData = (string key) => {
				string value = PlayerPrefs.GetString(key, string.Empty);
				return value;
			};

			// load from defaults
			this.fbId = GetStoredData(FACEBOOK_ID_KEY);
			this.email = GetStoredData(FACEBOOK_EMAIL_KEY);
			this.fullName = GetStoredData(FACEBOOK_FULL_NAME);
			this.profilePhoto = GetStoredData(FACEBOOK_PROFILE_PHOTO);

			this.PrepareFsm();
			
			// login signals
			S88Signals.FACEBOOK_LOGIN_REQUESTED.AddListener(this.Login);

			QuerySystem.RegisterResolver(QueryIds.HasLoggedInUser, (IQueryRequest request, IMutableQueryResult result) => {
				result.Set(this.HasLoggedInUser());
			});

			QuerySystem.RegisterResolver(QueryIds.UserEmail, (IQueryRequest request, IMutableQueryResult result) => {
				result.Set(this.email);
			});

			QuerySystem.RegisterResolver(QueryIds.UserFacebookId, (IQueryRequest request, IMutableQueryResult result) => {
				result.Set(this.fbId);
			});

			QuerySystem.RegisterResolver(QueryIds.UserFullName, (IQueryRequest request, IMutableQueryResult result) => {
				result.Set(this.fullName);
			});

			QuerySystem.RegisterResolver(QueryIds.UserProfilePhoto, (IQueryRequest request, IMutableQueryResult result) => {
				result.Set(this.profilePhoto);
			});
		}

		private void Update() {
			this.fsm.Update();
		}
		
		private void OnDestroy() {
			S88Signals.FACEBOOK_LOGIN_REQUESTED.RemoveListener(this.Login);
			QuerySystem.RemoveResolver(QueryIds.HasLoggedInUser);
			QuerySystem.RemoveResolver(QueryIds.UserEmail);
			QuerySystem.RemoveResolver(QueryIds.UserFacebookId);
			QuerySystem.RemoveResolver(QueryIds.UserFullName);
			QuerySystem.RemoveResolver(QueryIds.UserProfilePhoto);
		}
		
		// initial state
		private const string IDLE = "Idle";
		
		// events
		private const string INIT_FB = "InitFb";
		private const string FINISHED = "Finished";
		private const string LOGOUT_REQUESTED = "LogoutRequested";
		private const string LOGIN_SUCCESS = "LoginSuccess";
		private const string LOGIN_FAILED = "LoginFailed";
		private const string SUCCESS = "Success";
		private const string FAILED = "Failed";
		
		private void PrepareFsm() {
			this.fsm = new Fsm("FacebookLogin");
			
			// states
			FsmState idle = fsm.AddState(IDLE);
			FsmState init = fsm.AddState("Init");
			FsmState login = fsm.AddState("Login");
			FsmState requestPlayerData = fsm.AddState("RequestPlayerData");
			FsmState loginSuccessful = fsm.AddState("LoginSuccessful");
			FsmState loginFailed = fsm.AddState("loginFailed");
			FsmState logout = fsm.AddState("Logout");
			
			idle.AddAction(new FsmDelegateAction(idle, delegate(FsmState owner) {
				Debug.Log("FacebookLogin::Idle State\n");
			}));
			
			// actions
			init.AddAction(new FsmDelegateAction(init, delegate(FsmState owner) {
				Debug.Log("FacebookLogin::Init State\n");
				if(FB.IsInitialized) {
					// already initialized
					owner.SendEvent(FINISHED);
					return;
				}
				
				FB.Init(OnInitComplete, OnHideUnity);
			}, delegate(FsmState owner) {
				// OnUpdate()
				// check if FB is initialized and send Finished event
				if(FB.IsInitialized) {
					owner.SendEvent(FINISHED);
				}
			}));
			
			login.AddAction(new FsmDelegateAction(login, delegate(FsmState owner) {
				Debug.Log("FacebookLogin::Login State\n");
				if(FB.IsLoggedIn) {
					// already logged in
					owner.SendEvent(FINISHED);
					return;
				}
				
				DoFbLogin();
			}));
			
			requestPlayerData.AddAction(new FsmDelegateAction(requestPlayerData, delegate(FsmState owner) {
				Debug.Log("FacebookLogin::RequestPlayerData State\n");
				//FB.API("/me", HttpMethod.GET, this.HandlePlayerDataResult);
				//FB.API("/me?fields=email,name,picture", HttpMethod.GET, this.HandlePlayerDataResult);
				FB.API("me?fields=email,name,picture.height(713).width(713)", HttpMethod.GET, this.HandlePlayerDataResult);
			}));
			
			loginSuccessful.AddAction(new FsmDelegateAction(loginSuccessful, delegate(FsmState owner) {
				Debug.Log("FacebookLogin::LoginSuccessful State\n");
				S88Signals.ON_FACEBOOK_LOGIN_SUCCESSFUL.Dispatch();
			}));

			loginFailed.AddAction(new FsmDelegateAction(loginFailed, delegate(FsmState owner) {
				Debug.Log("FacebookLogin::LoginFailed State\n");
				S88Signals.ON_FACEBOOK_LOGIN_FAILED.Dispatch();
				owner.SendEvent(FINISHED);
			}));
			
			// transitions
			idle.AddTransition(INIT_FB, init);
			
			init.AddTransition(FINISHED, login);
			
			login.AddTransition(LOGIN_SUCCESS, requestPlayerData);
			login.AddTransition(FINISHED, requestPlayerData);
			login.AddTransition(LOGIN_FAILED, loginFailed);

			loginFailed.AddTransition(FINISHED, idle);

			requestPlayerData.AddTransition(FAILED, requestPlayerData); // request again if it failed
			requestPlayerData.AddTransition(SUCCESS, loginSuccessful);
			
			loginSuccessful.AddTransition(LOGOUT_REQUESTED, logout);
			loginSuccessful.AddTransition(LOGIN_SUCCESS, requestPlayerData);

			// auto start fsm
			this.fsm.Start(IDLE);
		}

		private void Login(ISignalParameters parameters) {
			this.email = string.Empty;
			this.fullName = string.Empty;

			// start the login process by starting the FSM
			if(!FB.IsLoggedIn) {
				this.fsm.SendEvent(INIT_FB);
			}
			else {
				this.fsm.SendEvent(LOGIN_SUCCESS);
			}
		}

		private void DoFbLogin() {
			FB.LogInWithReadPermissions(PERMISSIONS, this.HandleLoginResult);
		}
		
		private void HandleLoginResult(IResult result) {
			if (result == null) {
				Debug.Log("FacebookLogin::HandleLoginResult Null Result\n");
				this.fsm.SendEvent(LOGIN_FAILED);
				return;
			}
			
			// Some platforms return the empty string instead of null.
			if (!string.IsNullOrEmpty(result.Error)) {
				Debug.LogFormat("FacebookLogin::HandleLoginResult Login Error: {0}\n", result.Error);
				this.fsm.SendEvent(LOGIN_FAILED);
			} else if (result.Cancelled) {
				Debug.LogFormat("FacebookLogin::HandleLoginResult Login Cancelled: {0}\n", result.RawResult);
				this.fsm.SendEvent(LOGIN_FAILED);
			} else if (!string.IsNullOrEmpty(result.RawResult)) {
				Debug.LogFormat("FacebookLogin::HandleLoginResult Login Success: {0}\n", result.RawResult);
				this.ProcessPermissions(result.RawResult);
				this.fsm.SendEvent(LOGIN_SUCCESS);
			} else {
				Debug.Log("FacebookLogin::HandleLoginResult Login Empty Response\n");
				this.fsm.SendEvent(LOGIN_FAILED);
			}
		}
		
		private void HandlePlayerDataResult(IResult result) {
			if (result == null) {
				Debug.Log("FacebookLogin::HandlePlayerDataResult Null Result");
				this.fsm.SendEvent(FAILED);
				return;
			}
			
			// Some platforms return the empty string instead of null.
			if (!string.IsNullOrEmpty(result.Error)) {
				Debug.LogFormat("FacebookLogin::HandlePlayerDataResult Error: {0}\n", result.Error);
				this.fsm.SendEvent(FAILED);
			} else if (result.Cancelled) {
				Debug.LogFormat("FacebookLogin::HandlePlayerDataResult  Cancelled: {0}", result.RawResult);
				this.fsm.SendEvent(FAILED);
			} else if (!string.IsNullOrEmpty(result.RawResult)) {
				Debug.LogFormat("FacebookLogin::HandlePlayerDataResult  Success: {0}\n", result.RawResult);
				SavePlayerData(result.RawResult);
				this.fsm.SendEvent(SUCCESS);
			} else {
				Debug.Log("FacebookLogin::HandlePlayerDataResult  Empty Response\n");
				this.fsm.SendEvent(FAILED);
			}
		}

	//	{
	//		"email": "aries.sulit@gmail.com",
	//		"name": "Aries Sanchez Sulit",
	//		"picture": {
	//			"data": {
	//				"height": 713,
	//				"is_silhouette": false,
	//				"url": "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-xlp1/v/t1.0-1/10410893_10204493882219731_2253049558551334393_n.jpg?oh=740d9b8d9d205b26ed702eaee791b20c&oe=57417837&__gda__=1463505422_daa58978b4952292a425b7a10c9025d1",
	//				"width": 713
	//			}
	//		},
	//		"id": "10204344991737562"
	//	}

		private void SavePlayerData(string rawResult) {
			Debug.LogFormat("FacebookLogin::SavePlayerData RawResult:{0}\n", rawResult);

			Dictionary<string, object> root = (Dictionary<string, object>)Json.Deserialize(rawResult);
			root.TryGetValue(FB_ID_KEY, out this.fbId);
			root.TryGetValue(FB_NAME_KEY, out this.fullName);
			root.TryGetValue(FB_EMAIL, out this.email);
			root.TryGetValue(FB_PROFILE_PHOTO, out root);
			root.TryGetValue(FB_PROFILE_PHOTO_DATA, out root);
			root.TryGetValue(FB_PROFILE_PHOTO_URL, out this.profilePhoto);

			PlayerPrefs.SetString(FACEBOOK_ID_KEY, this.fbId);
			PlayerPrefs.SetString(FACEBOOK_FULL_NAME, this.fullName);
			PlayerPrefs.SetString(FACEBOOK_EMAIL_KEY, this.email);
			PlayerPrefs.SetString(FACEBOOK_PROFILE_PHOTO, this.profilePhoto);
			
			Debug.LogFormat("FacebookLogin::SavePlayerData FbId:{0} Name:{1} Email:{2} Picture:{3}\n", this.fbId, this.fullName, this.email, this.profilePhoto);
		}

		private void OnInitComplete() {
			Debug.LogFormat("FacebookLogin::OnInitComplete\n");
		}
		
		private void OnHideUnity(bool isGameShown) {
			Debug.LogFormat("FacebookLogin::OnHideUnity IsShow:{0}\n", isGameShown);
		}

		private bool HasLoggedInUser() {
			if (!PlayerPrefs.HasKey(FacebookLogin.FACEBOOK_EMAIL_KEY)) {
				return false;
			}

			if (PlayerPrefs.HasKey(FacebookLogin.FACEBOOK_ID_KEY)) {
				this.email = PlayerPrefs.GetString(FacebookLogin.FACEBOOK_EMAIL_KEY);
				this.fbId = PlayerPrefs.GetString(FacebookLogin.FACEBOOK_ID_KEY);
				return true;
			}

			return false;
		}

		#region Utils

		private void ProcessPermissions(string result) {
//			Debug.LogFormat("FacebookLogin::ProcessPermissions Result:{0}\n", result);
//
//			Dictionary<string, object> resultData = (Dictionary<string, object>)Json.Deserialize(result);
//			string[] permissions = this.GetPermissions(resultData);
//			List<object> grantedPermissions = this.GetGrantedPermissions(resultData);
//
//			foreach (string permission in permissions) {
//				this.permissions.Add(permission);
//				this.grantedPermissions.Add(new Permission(permission, false));
//				this.grantedPermissionMap.Add(permission, false);
//			}
//
//			foreach (object permission in grantedPermissions) {
//				string strPerm = (string)permission;
//				if (!this.grantedPermissionMap.ContainsKey(strPerm)) {
//					this.grantedPermissions.Add(new Permission(strPerm, true));
//					this.grantedPermissionMap.Add(strPerm, true);
//				}
//				else {
//					//Permission perm = this.grantedPermissions.Find(p => p.Type.Equals(strPerm));
//					//perm.Granted = true;
//					//this.grantedPermissions[this.grantedPermissions.IndexOf(perm)] = perm;
//					//this.grantedPermissionMap[strPerm] = true;
//					Permission holder = null;
//					for (int index = 0; index < this.grantedPermissions.Count; index++) {
//						Permission p = this.grantedPermissions[index];
//						if (p.Type.Equals(strPerm)) {
//							holder = p; 
//							holder.Granted = true;
//							this.grantedPermissions[index] = holder;
//							break;
//						}
//					}
//
//					if (holder == null) {
//						this.grantedPermissions.Add(new Permission(strPerm, true));
//					}
//
//					this.grantedPermissionMap[strPerm] = true;
//				}
//			}
//
//			Debug.LogFormat("FacebookLogin::ProcessPermissions Permissions:{0} GrantedPermissions:{1}", Json.Serialize(this.permissions), Json.Serialize(this.grantedPermissionMap));
		}

		private string[] GetPermissions(Dictionary<string, object> result) {
			string rawPermissions = (string)result["permissions"];
			return rawPermissions.Split(',');
		}

		// List<string>
		private List<object> GetGrantedPermissions(Dictionary<string, object> result) {
			return (List<object>)result["granted_permissions"];
		}

		#endregion
		*/
	}

	[Serializable]
	public class Permission {
		public string Type;
		public bool Granted;

		public Permission(string type, bool isGranted) {
			this.Type = type;
			this.Granted = isGranted;
		}
	}
	
}