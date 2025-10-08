using System;
#if FIREBASE
using Firebase;
#endif
using UnityEngine;

namespace VKSdk
{
    public class VKFirebaseManager : MonoBehaviour
    {
        public static string _messageToken = "";
        public static string _dynamicAction = "";

        public Action OnDynamicLinkRecieved;

#if FIREBASE
        public bool readyToInitialize;
        public VKFirebaseAnalytics analytics;
        public VKFirebaseDynamicLink dynamicLink;

        public string cacheDynamicLink;

        #region Singleton
        private static VKFirebaseManager instance;

        public static VKFirebaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKFirebaseManager>();
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

        private void Start()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

            Firebase.DynamicLinks.DynamicLinks.DynamicLinkReceived += OnDynamicLink;

            analytics = new VKFirebaseAnalytics();
            dynamicLink = new VKFirebaseDynamicLink();

            Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
            VKFirebaseInitializer.Initialize(dependencyStatus =>
            {
                if (dependencyStatus == DependencyStatus.Available)
                {
                    var app = Firebase.FirebaseApp.DefaultInstance;
                    
                    analytics.InitAnalytics();
                    // Debug.Log("Firebase Crashlytics Enabled: " + Firebase.Crashlytics.Crashlytics.IsCrashlyticsCollectionEnabled);

                    readyToInitialize = true;
                }
                else
                {
                    VKDebug.LogWarning("Could not resolve all Firebase dependencies: " + dependencyStatus);

                    analytics.DisableAnalytics();
                }
            });
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            _messageToken = token.Token;
            // UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            // UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        }

        // Display the dynamic link received by the application.
        void OnDynamicLink(object sender, System.EventArgs args) {
            var dynamicLinkEventArgs = args as Firebase.DynamicLinks.ReceivedDynamicLinkEventArgs;
            _dynamicAction = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString;

#if UNITY_ANDROID
            // try
            // {
            //     using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
            //     {
            //         using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            //         {
            //             var intent = activity.Call<AndroidJavaObject>("getIntent");
            //             intent.Call("removeExtra", "com.google.firebase.dynamiclinks.DYNAMIC_LINK_DATA");
            //             intent.Call("removeExtra", "com.google.android.gms.appinvite.REFERRAL_BUNDLE");
            //         }
            //     }
            // }
            // catch (System.Exception e)
            // {
            //     Debug.Log(e.Message);
            // }

            if(string.IsNullOrEmpty(cacheDynamicLink)) cacheDynamicLink = _dynamicAction;
            else if(cacheDynamicLink.Equals(_dynamicAction)) _dynamicAction = "";
#endif
            if(!string.IsNullOrEmpty(_dynamicAction) && OnDynamicLinkRecieved != null) OnDynamicLinkRecieved.Invoke();
        }

#else
        public bool readyToInitialize;

        private static VKFirebaseManager instance;
        public static VKFirebaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKFirebaseManager>();
                }
                return instance;
            }
        }

#endif
    }
}