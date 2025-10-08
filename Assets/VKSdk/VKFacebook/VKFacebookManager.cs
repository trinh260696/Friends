using System;
using System.Collections.Generic;
using UnityEngine;
using VKSdk;
using LitJson;
#if FACEBOOK
using Facebook.Unity;
#endif

/* Author: Anhnh1721
 * Version: 1.0
 */
namespace VKSdk
{
    public class VKFacebookManager : MonoBehaviour
    {
#if FACEBOOK
        public enum FacebookAction
        {
            Init,
            Login,
            Logout,
            GetMyData,
            GetAvatar,
            GetFriend
        }

        //private List<object> friends; 
        //public List<FBFriend> fbFriends;
        public string URL_SHARE_ANDROID = "";
        public string URL_SHARE_IOS = "";
        public string SHARE_CAPTION = "";
        public string SHARE_LINK_NAME = "";

        public delegate void FacebookResultDelegate(FacebookAction action, bool success);
        public event FacebookResultDelegate OnFacebookResult;

        public string accessToken;
        public FBPlayer fbPlayer;
        public List<FBFriend> myFriends;

        #region Singleton
        private static VKFacebookManager instance;

        public static VKFacebookManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKFacebookManager>();
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }

            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        #region Init
        public void InitFacebook()
        {
            if (!FB.IsInitialized)
            {
                fbPlayer = new FBPlayer();
                FB.Init(OnInitCallBack, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
        }

        public void OnInitCallBack()
        {
            if(FB.IsInitialized)
            {
                if (FB.IsLoggedIn)
                {
                    var aToken = AccessToken.CurrentAccessToken;
                    accessToken = aToken.TokenString;
                }

                if (OnFacebookResult != null)
                    OnFacebookResult(FacebookAction.Init, true);

                FB.ActivateApp();
            }
            else
            {
                VKDebug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        #endregion

        #region Login
        public void FBlogin()
        {
            List<string> permissions = new List<string>() { "public_profile", "user_friends" };
            FB.LogInWithReadPermissions(permissions, OnLoginReadCallBack);
        }

        public void OnLoginReadCallBack(ILoginResult result)
        {
            if (!result.Cancelled && string.IsNullOrEmpty(result.Error))
            {
                var aToken = AccessToken.CurrentAccessToken;
                accessToken = aToken.TokenString;
                // foreach (var item in AccessToken.CurrentAccessToken.Permissions)
                // {
                //     Debug.Log("Permissions " + item);
                // }
            }

            if (OnFacebookResult != null)
                OnFacebookResult(FacebookAction.Login, string.IsNullOrEmpty(result.Error));
        }
        #endregion

        #region Logout
        public void FBlogout()
        {
            FB.LogOut();
            accessToken = "";
            if (OnFacebookResult != null)
                OnFacebookResult(FacebookAction.Logout, true);
        }
        #endregion

        #region Avatar
        private int cacheSize = 0;
        public void FBGetAvatar(int size)
        {
            cacheSize = size;
            FB.API("/me/picture?type=square&height=" + size + "&width=" + size, HttpMethod.GET, OnGetAvatarCallBack);
        }

        public void OnGetAvatarCallBack(IGraphResult result)
        {
            if (result.Texture != null)
                fbPlayer.avatar = VKCommon.ResizeTexture2D(result.Texture, cacheSize, cacheSize);


            if (OnFacebookResult != null)
                OnFacebookResult(FacebookAction.GetAvatar, string.IsNullOrEmpty(result.Error));
        }
        #endregion

        #region My Data
        public void FBGetMyData()
        {
            FB.API("/me?fields=name,first_name,last_name,age_range,gender,birthday", HttpMethod.GET, OnGetMyDataCallBack);
        }

        public void OnGetMyDataCallBack(IGraphResult result)
        {
            if (result.Error == null)
            {
                if (result.ResultDictionary.ContainsKey("id"))
                    fbPlayer.id = result.ResultDictionary["id"].ToString();
                if (result.ResultDictionary.ContainsKey("name"))
                    fbPlayer.name = result.ResultDictionary["name"].ToString();
                if (result.ResultDictionary.ContainsKey("first_name"))
                    fbPlayer.first_name = result.ResultDictionary["first_name"].ToString();
                if (result.ResultDictionary.ContainsKey("last_name"))
                    fbPlayer.last_name = result.ResultDictionary["last_name"].ToString();
                if (result.ResultDictionary.ContainsKey("gender"))
                    fbPlayer.gender = result.ResultDictionary["gender"].ToString();
                if (result.ResultDictionary.ContainsKey("birthday"))
                    fbPlayer.birthday = result.ResultDictionary["birthday"].ToString();
                if (result.ResultDictionary.ContainsKey("age_range"))
                {
                    IDictionary<string, object> ages = (IDictionary<string, object>)result.ResultDictionary["age_range"];
                    if (ages.ContainsKey("min"))
                    {
                        fbPlayer.age_range_min = int.Parse(ages["min"].ToString());
                    }
                    if (ages.ContainsKey("max"))
                    {
                        fbPlayer.age_range_max = int.Parse(ages["max"].ToString());
                    }
                }
            }

            if (OnFacebookResult != null)
                OnFacebookResult(FacebookAction.GetMyData, string.IsNullOrEmpty(result.Error));
        }
        #endregion

        #region Share
        public void FBShare()
        {
            //string caption = LocalizationManager.GetTermTranslation(SHARE_CAPTION);
            //string name = LocalizationManager.GetTermTranslation(SHARE_LINK_NAME);
            string caption = "";
            string name = "";
            if (string.IsNullOrEmpty(caption))
                caption = SHARE_CAPTION;
            if (string.IsNullOrEmpty(name))
                name = SHARE_LINK_NAME;

#if UNITY_ANDROID
        FB.FeedShare(
            link: new Uri(URL_SHARE_ANDROID),
            linkCaption: caption,
            linkName: name
        );
#elif UNITY_IOS
        FB.FeedShare(
            link: new Uri(URL_SHARE_IOS),
            linkCaption: caption,
            linkName: name
        );
#endif
        }
        #endregion

        #region Friend
        public void FBGetFriends(int avatarSize, int limit)
        {
            string queryString = "/me/friends?fields=id,name,first_name,last_name&limit=" + limit;

            if(avatarSize > 0)
            {
                queryString = "/me/friends?fields=id,name,first_name,last_name,picture.width(" + avatarSize + ").height(" + avatarSize + ")&limit=" + limit;
            }
            FB.API(queryString, HttpMethod.GET, GetFriendsCallback);
        }

        private void GetFriendsCallback(IGraphResult result)
        {
            if (result.Error != null)
            {
                VKDebug.LogError(result.Error);
                return;
            }
            else
            {
                myFriends = new List<FBFriend>();
                try
                {
                    var dict = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
                    var friendList = (List<object>)dict["data"];
                    
                    // VKDebug.Log("Data: " + result.RawResult);
                    // VKDebug.Log(Facebook.MiniJSON.Json.Serialize(friendList));

                    myFriends = JsonMapper.ToObject<List<FBFriend>>(Facebook.MiniJSON.Json.Serialize(friendList));

                    // VKDebug.Log(JsonMapper.ToJson(myFriends));
                }
                catch {}
            }
            OnFacebookResult(FacebookAction.GetFriend, string.IsNullOrEmpty(result.Error));
        }

        public void LoadFriendImgFromID(string userID, int size, Action<Texture2D> callback)
        {
            FB.API(GraphUtil.GetPictureQuery(userID, size, size),
                   HttpMethod.GET,
                   delegate (IGraphResult result)
                   {
                       if (result.Error != null)
                       {
                        //    Debug.LogWarning(result.Error + ": for friend " + userID);
                           return;
                       }
                       if (result.Texture == null)
                       {
                        //    VKDebug.Log("LoadFriendImg: No Texture returned");
                           return;
                       }
                       if (callback != null)
                           callback(result.Texture);
                   });
        }
        #endregion

        #region GET DATA
        public FBPlayer GetFBPlayer()
        {
            return fbPlayer;
        }

        public string GetUserId()
        {
            return fbPlayer.id;
        }

        public bool IsLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        public bool IsInitialized()
        {
            return FB.IsInitialized;
        }
        #endregion
#else
        public enum FacebookAction
        {
            Init,
            Login,
            Logout,
            GetMyData,
            GetAvatar,
            GetFriend
        }

        public string accessToken;
        public FBPlayer fbPlayer;
        public List<FBFriend> myFriends;

        public bool isInitialized;

        public delegate void FacebookResultDelegate(FacebookAction action, bool success);
        public event FacebookResultDelegate OnFacebookResult;

        private static VKFacebookManager instance;

        public static VKFacebookManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKFacebookManager>();
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        public void InitFacebook()
        {
            isInitialized = true;
        }

        public void FBlogin()
        {
            if(OnFacebookResult != null) OnFacebookResult.Invoke(FacebookAction.Login, false);
        }

        public void FBlogout()
        {
            if(OnFacebookResult != null) OnFacebookResult.Invoke(FacebookAction.Logout, true);
        }

        public void FBGetAvatar(int size)
        {
        }

        public void FBGetMyData()
        {
        }

        public void FBGetFriends(int avatarSize, int limit)
        {
        }

        public void LoadFriendImgFromID(string userID, int size, Action<Texture2D> callback)
        {
            callback(null);
        }

        public FBPlayer GetFBPlayer()
        {
            return new FBPlayer();
        }

        public string GetUserId()
        {
            return "";
        }

        public bool IsLoggedIn()
        {
            return false;
        }

        public bool IsInitialized()
        {
            return false;
        }
#endif
    }
}