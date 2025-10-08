using System;
using System.Collections;
using UnityEngine;
using VKSdk;

namespace VKSdk.UI
{
    public class VKLoading : MonoBehaviour
    {
        private Action actionCallback;
       
        public void ShowLoading(bool autoHide, Action actionAutoHide = null, bool changeBG=false)
        {
            actionCallback = actionAutoHide;

         
           
            if (autoHide)
                StartCoroutine("WaitToHideLoading");
        }

        public void HideLoading()
        {
            StopAllCoroutines();
           
        }

        public IEnumerator WaitToHideLoading()
        {
            yield return new WaitForSeconds(2f);
            VKLayerController.Instance.HideLoading();

            if(actionCallback != null) actionCallback.Invoke();
        }
    }
}