using System.Text;
#if APPLEID
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif
using UnityEngine;

namespace VKSdk
{
    public class VKAppleSigninManager : MonoBehaviour
    {
#if APPLEID
        private const string AppleUserIdKey = "AppleUserId";

        public bool allowQuickLogin;
        public bool saveUserId;

        [HideInInspector]
        public IAppleAuthManager _appleAuthManager;
        [HideInInspector]
        public bool initialized;
        [HideInInspector]
        public string rawNonce;
        [HideInInspector]
        public string nonce;
        [HideInInspector]
        public string identityToken;
        [HideInInspector]
        public string authorizationCode;
        [HideInInspector]
        public ICredential userCredential;

        #region Singleton
        private static VKAppleSigninManager instance;

        public static VKAppleSigninManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKAppleSigninManager>();
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
            identityToken = "";
            initialized = false;

            // If the current platform is supported
#if UNITY_IOS
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                this._appleAuthManager = new AppleAuthManager(deserializer);
            }
#endif

            this.Initialize();
        }

        private void Update()
        {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if (this._appleAuthManager != null)
            {
                this._appleAuthManager.Update();
            }
        }

        private void Initialize()
        {
            initialized = true;

            // Check if the current platform supports Sign In With Apple
            if (this._appleAuthManager == null)
            {
                Debug.Log("Login Apple Unsupported platform");
                return;
            }

            // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
            this._appleAuthManager.SetCredentialsRevokedCallback(result =>
            {
                // login Apple bi user xoa thi callback vao day
                Debug.Log("Received revoked callback " + result);
                PlayerPrefs.DeleteKey(AppleUserIdKey);
            });

            // If we have an Apple User Id available, get the credential status for it
            if (saveUserId && PlayerPrefs.HasKey(AppleUserIdKey))
            {
                // Neu da login Apple thi check credential
                var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
                Debug.Log("CheckCredentialStatusForUserId: " + storedAppleUserId);
                this.CheckCredentialStatusForUserId(storedAppleUserId);
            }
            // If we do not have an stored Apple User Id, attempt a quick login
            else if (allowQuickLogin)
            {
                // Neu chua login Apple thu quicklogin
                this.AttemptQuickLogin();
            }
        }

        private void CheckCredentialStatusForUserId(string appleUserId)
        {
            // If there is an apple ID available, we should check the credential state
            this._appleAuthManager.GetCredentialState(
                appleUserId,
                state =>
                {
                    switch (state)
                    {
                        // If it's authorized, login with that user id
                        case CredentialState.Authorized:
                            Debug.Log("haslogin: ");
                            // identityToken = "haslogin";
                            // authorizationCode = PlayerPrefs.GetString(AppleUserIdKey+"_code");
                            return;

                        // If it was revoked, or not found, we need a new sign in with apple attempt
                        // Discard previous apple user id
                        case CredentialState.Revoked:
                        case CredentialState.NotFound:
                        case CredentialState.Transferred:
                            Debug.Log("Node: ");
                            identityToken = "none";
                            PlayerPrefs.DeleteKey(AppleUserIdKey);
                            return;
                    }
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
                    identityToken = "none";
                });
        }

        public void AttemptQuickLogin()
        {
            identityToken = "";

            var quickLoginArgs = new AppleAuthQuickLoginArgs();

            // Quick login should succeed if the credential was authorized before and not revoked
            this._appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    // If it's an Apple credential, save the user ID, for later logins
                    this.userCredential = credential;
                    var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {
                        identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                        authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                        PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                        // PlayerPrefs.SetString(AppleUserIdKey+"_code", authorizationCode);
                    }
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    identityToken = "none";
                });
        }

        public void AttemptQuickLoginNone()
        {
            identityToken = "";

            rawNonce = VKCommon.GenerateRandomString(32);
            nonce = VKCommon.GenerateSHA256NonceFromRawNonce(rawNonce);

            var quickLoginArgs = new AppleAuthQuickLoginArgs(nonce: nonce);

            // Quick login should succeed if the credential was authorized before and not revoked
            this._appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    // If it's an Apple credential, save the user ID, for later logins
                    this.userCredential = credential;
                    var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {
                        identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                        authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                        PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                        // PlayerPrefs.SetString(AppleUserIdKey+"_code", authorizationCode);
                    }
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    identityToken = "none";
                });
        }

        public void SignInWithApple()
        {
            identityToken = "";
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            this._appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                    this.userCredential = credential;
                    var appleIdCredential = credential as IAppleIDCredential;

                    identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
                    
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                    // PlayerPrefs.SetString(AppleUserIdKey+"_code", authorizationCode);
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    identityToken = "none";
                });
        }

        public void SignInWithAppleNonce()
        {
            identityToken = "";
            rawNonce = VKCommon.GenerateRandomString(32);
            nonce = VKCommon.GenerateSHA256NonceFromRawNonce(rawNonce);

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName, nonce: nonce);

            this._appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                    this.userCredential = credential;

                    var appleIdCredential = credential as IAppleIDCredential;

                    identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
                
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                    // PlayerPrefs.SetString(AppleUserIdKey+"_code", authorizationCode);
                },
                error =>
                {
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    identityToken = "none";
                });
        }

        public string GetUserId()
        {
            return userCredential != null ? userCredential.User : "";
        }

        public bool IsSupportPlatform()
        {
#if UNITY_IOS
            return AppleAuthManager.IsCurrentPlatformSupported;
#else
            return false;
#endif
        }

        public void Signout()
        {
            identityToken = "";
            authorizationCode = "";
        }

#else
        [HideInInspector]
        public bool initialized;
        [HideInInspector]
        public string rawNonce;
        [HideInInspector]
        public string nonce;
        [HideInInspector]
        public string identityToken;
        [HideInInspector]
        public string authorizationCode;

        public bool IsSupportPlatform()
        {
            return false;
        }

        public string GetUserID()
        {
            return "";
        }
#endif
    }
}