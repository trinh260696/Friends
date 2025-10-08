using System.Collections.Generic;
#if FIREBASE
using Firebase.DynamicLinks;
using Firebase.Analytics;
#endif
/* Author: Anhnh1721
 * Version: 1.0
 */
namespace VKSdk
{
    public class VKFirebaseDynamicLink
    {
        public const string IOS_ID = "vn.gtv.tuongky";
        public const string ANDROID_ID = "vn.gtv.tuongky";

        public const string DYNAMIC_LINK = "https://gametuongky.page.link";

        public static string _dynamicLink = "";

#if FIREBASE
        public void LoadDynamicLink(string baseLink)
        {
            string dlink = DYNAMIC_LINK;
            // VKDebug.Log("DYNAMIC_LINK " + DYNAMIC_LINK);
            var components = new Firebase.DynamicLinks.DynamicLinkComponents(new System.Uri(baseLink), dlink)
            {
                IOSParameters = new Firebase.DynamicLinks.IOSParameters(IOS_ID),
                AndroidParameters = new Firebase.DynamicLinks.AndroidParameters(ANDROID_ID),
            };

            // do something with: components.LongDynamicLink
            var options = new Firebase.DynamicLinks.DynamicLinkOptions
            {
                PathLength = DynamicLinkPathLength.Unguessable
            };

            _dynamicLink = "";
            Firebase.DynamicLinks.DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    // VKDebug.LogError("GetShortLinkAsync was canceled.");
                    _dynamicLink = "null";
                    return;
                }
                if (task.IsFaulted)
                {
                    // VKDebug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
                    _dynamicLink = "null";
                    return;
                }

                // Short Link has been created.
                Firebase.DynamicLinks.ShortDynamicLink link = task.Result;
                // VKDebug.Log("Generated short link " + link.Url);
                _dynamicLink = link.Url.ToString();

                var warnings = new System.Collections.Generic.List<string>(link.Warnings);
                if (warnings.Count > 0)
                {
                    // Debug logging for warnings generating the short link.
                }
            });
        }
#endif
    }
}