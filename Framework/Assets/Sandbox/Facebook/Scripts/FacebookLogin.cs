using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

#if FACEBOOK_ENABLED
using Facebook;
using Facebook.Unity;
#endif

using UniRx;
using UniRx.Triggers;

using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;
using Common.Fsm.Action;
using Common.Time;

using Framework;

namespace Sandbox.Facebook
{
    using Sandbox.Downloader;
    using Sandbox.Services;
    
    public struct OnFacebookLoginSignal { }

    public struct OnFacebookLogoutSignal { }

    public class OnFacebookLoginSuccessSignal
    {
        public string Id;
        public string Token;
    }

    public class OnFacebookLoginFailedSignal { }

    public struct OnFacebookLogoutSuccessfulSignal
    {
        public string Id;
        public string Token;
    }
    
    [Serializable]
    public class Permission
    {
        public string Type;
        public bool Granted;

        public Permission(string type, bool isGranted)
        {
            Type = type;
            Granted = isGranted;
        }
    }

    [Serializable]
    public class FacebookPlayerData
    {
        public string Id;
        public string FullName;
        public string FirstName;
        public string MiddleName;
        public string LastName;
        public string ProfilePhoto;
        public string Email;
        public string Birthday;
        public string Gender;

        public Sprite FBPhoto;

        public string Token;
    }

    [Serializable]
    public class ProfilePhotoData
    {
        public bool is_silhouette;
        public int height;
        public int width;
        public string url;
    }

    [Serializable]
    public class Picture
    {
        public ProfilePhotoData data;
    }

    [Serializable]
    public class Profile
    {
        public string name;
        public string middle_name;
        public string first_name;
        public string last_name;
        public string gender;
        public string id;

        // Optional params
        public string email;
        public string birthday;

        public Picture picture;
    }

    public static class FBID
    {
        public const string HasLoggedInUser = "HasLoggedInUser";
        public const string UserEmail = "UserEmail";
        public const string UserFacebookId = "UserFacebookId";
        public const string UserFirstName = "UserFirstName";
        public const string UserLastName = "UserLastName";
        public const string UserFullName = "UserFullName";
        public const string UserProfilePhoto = "UserProfilePhoto";
        public const string UserGender = "UserGender";
        public const string UserBirthday = "UserBirthday";
        public const string FBPhoto = "FBPhoto";
    }

    public class FacebookLogin : BaseService
    {
        public static readonly string FB_PHOTO_ASSET_ID = "FBProfilePic.png";

        // Facebook Keys
        private static readonly string FB_ID_KEY = "id";
        private static readonly string FB_NAME_KEY = "name";
        private static readonly string FB_LAST_NAME_KEY = "last_name";
        private static readonly string FB_FIRST_NAME_KEY = "first_name";
        private static readonly string FB_MIDDLE_NAME_KEY = "middle_name";
        private static readonly string FB_EMAIL = "email";
        private static readonly string FB_PROFILE_PHOTO = "picture";
        private static readonly string FB_PROFILE_PHOTO_DATA = "data";
        private static readonly string FB_PROFILE_PHOTO_URL = "url";
        private static readonly string FB_USER_BIRTHDAY_KEY = "birthday";
        private static readonly string FB_USER_GENDER_KEY = "gender";

        // Prefs Keys
        private static readonly string FACEBOOK_EMAIL_KEY = "facebookEmailKey";
        private static readonly string FACEBOOK_ID_KEY = "facebookIdKey";
        private static readonly string FACEBOOK_TOKEN_KEY = "facebookTokenKey";
        private static readonly string FACEBOOK_MIDDLE_NAME = "facebookmiddleKey";
        private static readonly string FACEBOOK_FIRST_NAME = "facebookFistname";
        private static readonly string FACEBOOK_FULL_NAME = "facebookFullname";
        private static readonly string FACEBOOK_PROFILE_PHOTO = "facebookProfilePhoto";
        private static readonly string FACEBOOK_USER_GENDER = "facebookUserGender";
        private static readonly string FACEBOOK_USER_BIRTHDAY = "facebookUserBirthday";

        [SerializeField]
        private FacebookPlayerData FBdata;
        
        [SerializeField]
        private readonly List<string> PERMISSIONS = new List<string>()
        {
            "public_profile",
            "email",
            "user_friends",
            "user_birthday"
        };
        
        #region IService implementation

        public override void InitializeService()
        {
#if !FACEBOOK_ENABLED
            base.InitializeService();

            this.Receive<OnFacebookLoginSignal>()
                .Subscribe(_ => this.Publish(new OnFacebookLoginFailedSignal()))
                .AddTo(this);
#endif

#if FACEBOOK_ENABLED
            Func<string, string> GetStoredData = (string key) =>
            {
                string value = PlayerPrefs.GetString(key, string.Empty);
                return value;
            };
            
            FBdata.Id = GetStoredData(FACEBOOK_ID_KEY);
            FBdata.Email = GetStoredData(FACEBOOK_EMAIL_KEY);
            FBdata.FirstName = GetStoredData(FACEBOOK_FIRST_NAME);
            FBdata.FullName = GetStoredData(FACEBOOK_FULL_NAME);
            FBdata.ProfilePhoto = GetStoredData(FACEBOOK_PROFILE_PHOTO);
            FBdata.Birthday = GetStoredData(FACEBOOK_USER_BIRTHDAY);
            FBdata.Gender = GetStoredData(FACEBOOK_USER_GENDER);

            PrepareFsm();

            QuerySystem.RegisterResolver(FBID.HasLoggedInUser, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(HasLoggedInUser());
            });

            QuerySystem.RegisterResolver(FBID.UserEmail, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.Email);
            });

            QuerySystem.RegisterResolver(FBID.UserFacebookId, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.Id);
            });

            QuerySystem.RegisterResolver(FBID.UserFirstName, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.FirstName);
            });

            QuerySystem.RegisterResolver(FBID.UserLastName, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.LastName);
            });


            QuerySystem.RegisterResolver(FBID.UserFullName, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.FullName);
            });

            QuerySystem.RegisterResolver(FBID.UserProfilePhoto, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.ProfilePhoto);
            });

            QuerySystem.RegisterResolver(FBID.UserGender, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.Gender);
            });

            QuerySystem.RegisterResolver(FBID.UserBirthday, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.Birthday);
            });

            QuerySystem.RegisterResolver(FBID.FBPhoto, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(FBdata.FBPhoto);
            });
            
            this.Receive<OnFacebookLoginSignal>()
                .Subscribe(_ => Fsm.SendEvent(LOGIN_REQUESTED))
                .AddTo(this);

            this.Receive<OnFacebookLogoutSignal>()
                .Subscribe(_ => Fsm.SendEvent(LOGOUT_REQUESTED))
                .AddTo(this);
#endif
        }

        public override IEnumerator InitializeServiceSequentially()
        {
            yield return null;

#if !FACEBOOK_ENABLED
            base.InitializeService();

            this.Receive<OnFacebookLoginSignal>()
                .Subscribe(_ => this.Publish(new OnFacebookLoginFailedSignal()))
                .AddTo(this);

            CurrentServiceState.Value = ServiceState.Initialized;
#else
            InitializeService();
#endif
        }

        public override void TerminateService()
        {
            base.TerminateService();

            QuerySystem.RemoveResolver(FBID.HasLoggedInUser);
            QuerySystem.RemoveResolver(FBID.UserEmail);
            QuerySystem.RemoveResolver(FBID.UserFacebookId);
            QuerySystem.RemoveResolver(FBID.UserFirstName);
            QuerySystem.RemoveResolver(FBID.UserFullName);
            QuerySystem.RemoveResolver(FBID.UserProfilePhoto);
            QuerySystem.RemoveResolver(FBID.UserGender);
            QuerySystem.RemoveResolver(FBID.UserBirthday);
        }
#endregion

#region Facebook Fsm
        // initial state
        private const string IDLE = "Idle";

        // events
        private const string INIT_FB = "InitFb";
        private const string FINISHED = "Finished";
        private const string LOGIN_REQUESTED = "LoginRequested";
        private const string LOGOUT_REQUESTED = "LogoutRequested";
        private const string LOGIN_SUCCESS = "LoginSuccess";
        private const string LOGIN_FAILED = "LoginFailed";
        private const string SUCCESS = "Success";
        private const string FAILED = "Failed";
        private const string REQUEST_FAILED = "RequestFailed";

        [SerializeField]
        private Fsm Fsm;

        private void PrepareFsm()
        {
#if FACEBOOK_ENABLED
            Fsm = new Fsm("FacebookLogin");

            // states
            FsmState idle = Fsm.AddState(IDLE);
            FsmState init = Fsm.AddState("Init");
            FsmState login = Fsm.AddState("Login");
            FsmState requestPlayerData = Fsm.AddState("RequestPlayerData");
            FsmState loginSuccessful = Fsm.AddState("LoginSuccessful");
            FsmState loginFailed = Fsm.AddState("loginFailed");
            FsmState requestFailed = Fsm.AddState("requestFailed");
            FsmState logout = Fsm.AddState("Logout");

            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
                owner.SendEvent(INIT_FB);
            }));
            
            init.AddAction(new FsmDelegateAction(init,
                owner =>
                {
                    if (FB.IsInitialized)
                    {
                        owner.SendEvent(FINISHED);
                    }
                    else
                    {
                        FB.Init(OnInitComplete, OnHideUnity);
                    }
                },
                owner => 
                {
                    // Wait for FB to be initialized
                    if (FB.IsInitialized)
                    {
                        owner.SendEvent(FINISHED);
                    }
                },
                owner =>
                {
                    if (FB.IsInitialized)
                    {
                        CurrentServiceState.Value = ServiceState.Initialized;
                    }
                }));

            TimeReferencePool.GetInstance().Add("Timer");

            TimedWaitAction tiimeOut = new TimedWaitAction(login, "Timer", LOGIN_FAILED);
            tiimeOut.Init(15f);

            login.AddAction(new FsmDelegateAction(login,
                owner => 
                {
                    // already logged in
                    if (FB.IsLoggedIn)
                    {
                        owner.SendEvent(FINISHED);
                    }
                    else
                    {
                        tiimeOut.Init(15f);
                        DoFbLogin();
                    }
                }));

            login.AddAction(tiimeOut);

            requestPlayerData.AddAction(new FsmDelegateAction(requestPlayerData, 
                owner =>
                {
                    FB.API("me?fields=email,name,middle_name,first_name,last_name,picture.height(713).width(713),gender,birthday", HttpMethod.GET, HandlePlayerDataResult);
                }));

            loginSuccessful.AddAction(new FsmDelegateAction(loginSuccessful,
                owner =>
                { 
                    FBdata.Token = AccessToken.CurrentAccessToken.TokenString;

                    this.Publish(new OnFacebookLoginSuccessSignal() { Id = FBdata.Id, Token = FBdata.Token });
                    this.Publish(new DownloadAssetSignal()
                    {
                        AllowCaching = true,
                        AssetID = FB_PHOTO_ASSET_ID,
                        Url = FBdata.ProfilePhoto,
                        Timestamp = DateTime.Now.ToString()
                    });
                }));

            loginFailed.AddAction(new FsmDelegateAction(loginFailed, 
                owner =>
                {
                    this.Publish(new OnFacebookLoginFailedSignal());
                    owner.SendEvent(FINISHED);
                }));

            logout.AddAction(new FsmDelegateAction(logout, 
                owner =>
                {
                    DoFbLogout();
                    Fsm.SendEvent(IDLE);
                }));
            
            requestFailed.AddAction(new TriggerAction(requestFailed, FINISHED));

            // transitions
            idle.AddTransition(INIT_FB, init);
            idle.AddTransition(LOGIN_REQUESTED, login);

            init.AddTransition(FINISHED, idle);

            login.AddTransition(LOGIN_SUCCESS, requestPlayerData);
            login.AddTransition(FINISHED, requestPlayerData);
            login.AddTransition(LOGIN_FAILED, loginFailed);

            loginFailed.AddTransition(FINISHED, idle);

            requestPlayerData.AddTransition(FAILED, requestPlayerData); // request again if it failed
            requestPlayerData.AddTransition(SUCCESS, loginSuccessful);
            requestPlayerData.AddTransition(REQUEST_FAILED, requestFailed);

            requestFailed.AddTransition(FINISHED, logout);

            loginSuccessful.AddTransition(LOGOUT_REQUESTED, logout);
            loginSuccessful.AddTransition(LOGIN_SUCCESS, requestPlayerData);

            logout.AddTransition(IDLE, idle);

            // auto start fsm
            Fsm.Start(IDLE);

            this.UpdateAsObservable()
                .Subscribe(_ => Fsm.Update())
                .AddTo(this);
#endif
        }

#endregion
#if FACEBOOK_ENABLED
        private void OnFacebookShare(string url)
        {
            Assertion.Assert(FB.IsInitialized);
            FB.FeedShare(link: new Uri(url), callback: ShareCallback);
        }
        
        private void DoFbLogin()
        {
            FB.LogInWithReadPermissions(PERMISSIONS, HandleLoginResult);
        }

        private void DoFbLogout()
        {
            PlayerPrefs.DeleteKey(FacebookLogin.FACEBOOK_EMAIL_KEY);
            PlayerPrefs.DeleteKey(FacebookLogin.FACEBOOK_ID_KEY);

            FB.LogOut();

            // Delete all keys
            PlayerPrefs.DeleteAll();

            this.Publish(new OnFacebookLogoutSuccessfulSignal()
            {
                Id = FBdata.Id,
                Token = FBdata.Token,
            });

            // Clear cached values
            FBdata.Id = string.Empty;
            FBdata.FullName = string.Empty;
            FBdata.FirstName = string.Empty;
            FBdata.LastName = string.Empty;
            FBdata.ProfilePhoto = string.Empty;
            FBdata.Email = string.Empty;
            FBdata.Birthday = string.Empty;
            FBdata.Gender = string.Empty;

        }

        private void HandleLoginResult(IResult result)
        {
            if (result == null)
            {
                Debug.Log(D.FB + "FacebookLogin::HandleLoginResult Null Result\n");
                Fsm.SendEvent(LOGIN_FAILED);
                return;
            }

            // Some platforms returns an empty string instead of null.
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.LogFormat(D.FB + "FacebookLogin::HandleLoginResult Login Error: {0}\n", result.Error);
                Fsm.SendEvent(LOGIN_FAILED);
            }
            else if (result.Cancelled)
            {
                Debug.LogFormat(D.FB + "FacebookLogin::HandleLoginResult Login Cancelled: {0}\n", result.RawResult);
                Fsm.SendEvent(LOGIN_FAILED);
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                Debug.LogFormat("FacebookLogin::HandleLoginResult Login Success: {0}\n", result.RawResult);
                Fsm.SendEvent(LOGIN_SUCCESS);
            }
            else
            {
                Debug.Log(D.FB + "FacebookLogin::HandleLoginResult Login Empty Response\n");
                Fsm.SendEvent(LOGIN_FAILED);
            }
        }

        private void HandlePlayerDataResult(IResult result)
        {
            if (result == null)
            {
                Debug.Log(D.FB + "FacebookLogin::HandlePlayerDataResult Null Result");
                Fsm.SendEvent(FAILED);
                return;
            }

            // Some platforms return the empty string instead of null.
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.LogFormat(D.FB + "FacebookLogin::HandlePlayerDataResult Error: {0}\n", result.Error);
                Fsm.SendEvent(FAILED);
            }
            else if (result.Cancelled)
            {
                Debug.LogFormat(D.FB + "FacebookLogin::HandlePlayerDataResult  Cancelled: {0}", result.RawResult);
                Fsm.SendEvent(FAILED);
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                Debug.LogFormat(D.FB + "FacebookLogin::HandlePlayerDataResult  Success: {0}\n", result.RawResult);
                SavePlayerData(result.RawResult);
                Fsm.SendEvent(SUCCESS);
            }
            else
            {
                Debug.Log(D.FB + "FacebookLogin::HandlePlayerDataResult Empty Response\n");
                Fsm.SendEvent(FAILED);
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
        private void SavePlayerData(string rawResult)
        {
            Debug.LogFormat(D.FB + "FacebookLogin::SavePlayerData RawResult:{0}\n", rawResult);

            Profile profile = JsonUtility.FromJson<Profile>(rawResult);
            FBdata.Id = profile.id;
            FBdata.FullName = profile.name;
            FBdata.FirstName = profile.first_name;
            FBdata.MiddleName = profile.middle_name;
            FBdata.LastName = profile.last_name;
            FBdata.Gender = profile.gender;
            FBdata.Email = profile.email;
            FBdata.Birthday = profile.birthday;
            FBdata.ProfilePhoto = profile.picture.data.url;
            FBdata.Id = profile.id;

            PlayerPrefs.SetString(FACEBOOK_ID_KEY, FBdata.Id);
            PlayerPrefs.SetString(FACEBOOK_FULL_NAME, FBdata.FullName);
            PlayerPrefs.SetString(FACEBOOK_FIRST_NAME, FBdata.FirstName);
            PlayerPrefs.SetString(FACEBOOK_MIDDLE_NAME, FBdata.MiddleName);
            PlayerPrefs.SetString(FACEBOOK_USER_GENDER, FBdata.Gender);
            PlayerPrefs.SetString(FACEBOOK_USER_BIRTHDAY, FBdata.Birthday);
            PlayerPrefs.SetString(FACEBOOK_EMAIL_KEY, FBdata.Email);
            PlayerPrefs.SetString(FACEBOOK_PROFILE_PHOTO, FBdata.ProfilePhoto);

        }

        private void OnInitComplete()
        {
            Debug.LogFormat(D.FB + "FacebookLogin::OnInitComplete\n");
        }

        private void OnHideUnity(bool isGameShown)
        {
            Debug.LogFormat(D.FB + "FacebookLogin::OnHideUnity IsShow:{0}\n", isGameShown);
        }

        private bool HasLoggedInUser()
        {
            if (FB.IsLoggedIn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShareCallback(IShareResult result)
        {
            if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
            {
                Debug.LogFormat(D.FB + "FacebookLogin::ShareCallback Error:{0}\n", result.Error);
            }
            else if (!String.IsNullOrEmpty(result.PostId))
            {
                Debug.LogFormat(D.FB + "FacebookLogin::ShareCallback PostId:{0}\n", result.PostId);
            }
            else
            {
                Debug.LogFormat(D.FB + "FacebookLogin::ShareCallback Success!:{0}\n", result.RawResult);
            }
        }
#endif
    }
}