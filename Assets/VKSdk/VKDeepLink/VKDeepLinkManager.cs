using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Author: Anhnh1721
 * Version: 1.0
 */
namespace VKSdk
{
    public class VKDeepLinkManager : MonoBehaviour
    {
        public string deeplinkURL;

        #region Singleton
        private static VKDeepLinkManager instance;

        public static VKDeepLinkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKDeepLinkManager>();
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

                Application.deepLinkActivated += onDeepLinkActivated;
                if (!String.IsNullOrEmpty(Application.absoluteURL))
                {
                    // Cold start and Application.absoluteURL not null so process Deep Link.
                    onDeepLinkActivated(Application.absoluteURL);
                }
                // Initialize DeepLink Manager global variable.
                else deeplinkURL = "[none]";
                DontDestroyOnLoad(gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        private void onDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            deeplinkURL = url;
            
            // Decode the URL to determine action. 
            // In this example, the app expects a link formatted like this:
            // unitydl://mylink?scene1
            string sceneName = url.Split("?"[0])[1];
            // bool validScene;
            // switch (sceneName)
            // {
            //     case "scene1":
            //         validScene = true;
            //         break;
            //     case "scene2":
            //         validScene = true;
            //         break;
            //     default:
            //         validScene = false;
            //         break;
            // }
            // if (validScene) SceneManager.LoadScene(sceneName);
        }
    }
}
