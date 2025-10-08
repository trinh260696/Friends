using UnityEngine;
namespace VKSdk
{
    public class VKGameConfig : MonoBehaviour
    {
        public enum IOSBuildType
        {
            TEST_FLIGHT,
            STORE,
            PERSIONAL_CER,
            ENTERPRICE_CER,
            SUPPER_CER,
        }

        public enum AndroidBuildType
        {
            STORE,
            WEBSITE
        }

        public string androidVersion;
        public string iosVersion;
        public string pcVersion;
        public string macosVersion;
        public string webGLVersion;

        [Space(10)]
        public IOSBuildType iosBuildType;
        public AndroidBuildType androidBuildType;

        [Space(10)]
        public bool webglMobile;
        public string webglPlatform;
        public string webglBrowser;

        [Space(10)]
        public float timeOut = 2f;

        private int normalFrameRate = 30;
        private int gameFrameRate = 45;

        #region Singleton
        private static VKGameConfig instance;

        public static VKGameConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKGameConfig>();
                }
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
            Application.runInBackground = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        #endregion

        #region Support
        public string VersionGame()
        {
#if UNITY_ANDROID
        return androidVersion;
#elif UNITY_IOS
        return iosVersion;
#elif UNITY_STANDALONE_WIN
            return pcVersion;
#elif UNITY_STANDALONE_OSX
        return macosVersion;
#elif UNITY_WEBGL
        return webGLVersion;
#endif
        }

        public string GetVersionGame()
        {
            return string.Format("VERSION" + " " + VersionGame());
        }

        public static string Platform()
        {
#if UNITY_ANDROID
        return "android";
#elif UNITY_IOS
        return "ios";
#elif UNITY_STANDALONE_WIN
            return "window";
#elif UNITY_STANDALONE_OSX
        return "macos";
#elif UNITY_WEBGL
        return "webgl";
#else
        return "unknow";
#endif
        }

        public string PlatformLogin()
        {
#if UNITY_WEBGL
        return Platform() + "-" + webglPlatform;
#elif UNITY_IOS
        string subText = "-store";
        switch(iosBuildType)
        {
            case IOSBuildType.ENTERPRICE_CER:
                subText = "-enterprice-cer";
                break;
            case IOSBuildType.PERSIONAL_CER:
                subText = "-persional-cer";
                break;
            case IOSBuildType.TEST_FLIGHT:
                subText = "-testflight";
                break;
            case IOSBuildType.SUPPER_CER:
                subText = "-supper-cer";
                break;
        }
        return Platform() + subText;
#elif UNITY_ANDROID
        string subText = "-store";
        switch(androidBuildType)
        {
            case AndroidBuildType.WEBSITE:
                subText = "-website";
                break;
        }
        return Platform() + subText;
#else
#if GTV_PLATFORM
        return Platform() + "-gtv";
#elif VIE_PLATFORM
        return Platform() + "-vie";
#else
            return Platform() + "-normal";
#endif
#endif
        }

        public string PlatformDownloadAsset()
        {
#if UNITY_WEBGL
        return Platform() + (webglMobile ? "/mobile" : "/pc");
#else
            return Platform();
#endif
        }

        public void SettingQuality(QUALITY_ENUM qualityEnum)
        {
            switch (qualityEnum)
            {
                case QUALITY_ENUM.BEST:
                    QualitySettings.SetQualityLevel(2, true);
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 60;                  
                    break;
                case QUALITY_ENUM.GOOD:
                    QualitySettings.SetQualityLevel(2, true);
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 40;                   
                    break;
                case QUALITY_ENUM.NORMAL:
                    QualitySettings.SetQualityLevel(1, true);
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 30;                  
                    break;
            }
        }
        public void SettingQuantity()
        {
#if UNITY_ANDROID
        if (SystemInfo.systemMemorySize > 6000)
        {
            QualitySettings.SetQualityLevel(2, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            normalFrameRate = 30;
            gameFrameRate = 60;
        }
        else if (SystemInfo.systemMemorySize > 2400)
        {
            QualitySettings.SetQualityLevel(2, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            normalFrameRate = 30;
            gameFrameRate = 45;
        }
        else if(SystemInfo.systemMemorySize > 600)
        {
            QualitySettings.SetQualityLevel(1, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            normalFrameRate = 30;
            gameFrameRate = 30;
        }
        else
        {
            QualitySettings.SetQualityLevel(0, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 20;
            normalFrameRate = 20;
            gameFrameRate = 30;
        }
#elif UNITY_IOS
        if (SystemInfo.systemMemorySize > 5000)
        {
            QualitySettings.SetQualityLevel(2, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            normalFrameRate = 30;
            gameFrameRate = 60;
        }
        else if (SystemInfo.systemMemorySize > 2400)
        {
            QualitySettings.SetQualityLevel(2, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            normalFrameRate = 30;
            gameFrameRate = 45;
        }
        else if(SystemInfo.systemMemorySize > 600)
        {
            QualitySettings.SetQualityLevel(1, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            normalFrameRate = 30;
            gameFrameRate = 30;
        }
        else
        {
            QualitySettings.SetQualityLevel(0, true);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 20;
            normalFrameRate = 20;
            gameFrameRate = 30;
        }
#elif UNITY_WEBGL
            Application.targetFrameRate = -1;
            QualitySettings.SetQualityLevel(1, true);
#else
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 45;
            normalFrameRate = 45;
            gameFrameRate = 60;
#endif
        }

        public void SetTargetFarmeRate(bool isGame)
        {
            Application.targetFrameRate = isGame ? gameFrameRate : normalFrameRate;
        }
        #endregion

        // #region API CONFIG
        // public void SaveCacheApiConfig(string data)
        // {
        //     try
        //     {
        //         FileHelper.WriteTextToFile(KEY_CONFIG_GAME + ".json", data);
        //     }
        //     catch {}
        // }

        // public void RemoveCacheApiConfig()
        // {
        //     try
        //     {
        //         FileHelper.DeleteFile(KEY_CONFIG_GAME + ".json");
        //     }
        //     catch {}
        // }

        // public string GetCacheApiConfig()
        // {
        //     string data = "";
        //     try
        //     {
        //         data = FileHelper.LoadTextFromFile(KEY_CONFIG_GAME + ".json");
        //     }
        //     catch {}
        //     return data;
        // }
        // #endregion
    }
}
public enum QUALITY_ENUM
{
    BEST=0,
    GOOD=1,
    NORMAL=2
}