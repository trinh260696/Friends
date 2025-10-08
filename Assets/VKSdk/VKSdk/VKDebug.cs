using UnityEngine;

namespace VKSdk
{
    public class VKDebug
    {
        public static void Log(string log)
        {
#if UNITY_EDITOR || DEVELOPER
            Debug.Log(log);
#endif
        }

        public static void Log(string log, string hexColor)
        {
#if UNITY_EDITOR || DEVELOPER
            Debug.Log(VKCommon.FillColorString(log, hexColor));
#endif
        }

        public static void LogWarning(string log)
        {
#if UNITY_EDITOR || DEVELOPER
            Debug.LogWarning(log);
#endif
        }

        public static void LogWarning(string log, string hexColor)
        {
#if UNITY_EDITOR || DEVELOPER
            Debug.LogWarning(VKCommon.FillColorString(log, hexColor));
#endif
        }

        public static void LogError(string log)
        {
#if UNITY_EDITOR || DEVELOPER
            Debug.LogError(log);
#endif
        }

        public static void LogError(string log, string hexColor)
        {
#if UNITY_EDITOR || DEVELOPER
            Debug.LogError(VKCommon.FillColorString(log, hexColor));
#endif
        }
    }
}

