using System;
using System.Collections;
using UnityEngine;
using VKSdk.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
#if VK_RATEAPP
using Google.Play.Review;
#endif
#endif

namespace VKSdk
{
    public class VKRateAppController : MonoBehaviour
    {
        #region Properties
        public int minDayRange = 10;
        public int minCountShow = 10;

        private DateTime lastTimeShow;
        private int lastCountShow;

        private bool allowShowRate;

#if UNITY_ANDROID
#if VK_RATEAPP
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
#endif
#endif

        #endregion

        #region Sinleton
        private static VKRateAppController instance;
        public static VKRateAppController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKRateAppController>();
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
            DontDestroyOnLoad(this.transform);
        }
        
        void Start()
        {
            Init();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
            {
            }
            else
            {
                Init(); 
            }
        } 
        #endregion

        #region Method
        private void Init()
        {
#if UNITY_IOS || UNITY_ANDROID
            bool ignore = PlayerPrefs.GetInt("VK_RATE_IGNORE", 0) == 1;

            if(ignore)
            {
                allowShowRate = false;
            }
            else
            {
                string ld = PlayerPrefs.GetString("VK_RATE_LASTDATE", "");
                if(string.IsNullOrEmpty(ld))
                {
                    lastTimeShow = DateTime.Now.AddDays(-7);
                }
                else
                {
                    lastTimeShow = DateTime.ParseExact(ld, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }

                lastCountShow = PlayerPrefs.GetInt("VK_RATE_LASTCOUNT", 0);

                allowShowRate = (DateTime.Now - lastTimeShow).TotalDays >= minDayRange;
            }
#else
            allowShowRate = false;
#endif

#if UNITY_ANDROID
#if VK_RATEAPP
            _reviewManager = new ReviewManager();
#endif
#endif
        }

        public void ShowRate()
        {
            Debug.Log("allowShowRate " + allowShowRate);
            Debug.Log("lastCountShow " + lastCountShow);
            Debug.Log("minCountShow " + minCountShow);
            if(allowShowRate)
            {
                lastCountShow++;
                PlayerPrefs.SetInt("VK_RATE_LASTCOUNT", lastCountShow);
                if(lastCountShow >= minCountShow)
                {
#if UNITY_IOS
                    bool isOk = Device.RequestStoreReview();
                    if(isOk)
                    {
                        VKAnalytics.TrackFirst("first_open_ratenative");
                        ShowRateSuccess();
                    }
                    else
                    {
                        // show popup rate open store
                        VKLayerController.Instance.ShowLayer("LRateApp");
                    }
#elif UNITY_ANDROID
#if VK_RATEAPP
                    if(_playReviewInfo != null)
                    {
                        StartCoroutine(IEWaitShowReview());
                    }
#else
                    VKLayerController.Instance.ShowLayer("LRateApp");
#endif
#endif
                }
            }
        }

        public void ShowRateSuccess(int dayAdd = 0)
        {
            PlayerPrefs.SetInt("VK_RATE_LASTCOUNT", 0);
            PlayerPrefs.SetString("VK_RATE_LASTDATE", DateTime.Now.AddDays(dayAdd).ToString("yyyy-MM-dd HH:mm:ss"));
            allowShowRate = false;
        }

        public void IgnoreRate()
        {
            PlayerPrefs.SetInt("VK_RATE_IGNORE", 1);
            allowShowRate = false;
        }
        #endregion

#if UNITY_ANDROID
        #region Android review
        public void StartWaitReviewInfo()
        {
#if VK_RATEAPP
            if(allowShowRate && lastCountShow >= minCountShow && _playReviewInfo == null)
            {
                StartCoroutine(IEWaitReviewInfo());
            }
#endif
        }

#if VK_RATEAPP
        public IEnumerator IEWaitReviewInfo()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                VKDebug.LogWarning("requestFlowOperation: " + requestFlowOperation.Error.ToString());
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();
        }

        public IEnumerator IEWaitShowReview()
        {
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                VKDebug.LogWarning("launchFlowOperation: " + launchFlowOperation.Error.ToString());

                // show popup rate open store
                VKLayerController.Instance.ShowLayer("LRateApp");
                yield break;
            }
            else
            {
                VKAnalytics.TrackFirst("first_open_ratenative");
                ShowRateSuccess();
            }
        }
#endif
        #endregion
#endif
    }
}
