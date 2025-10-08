using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using VKSdk.UI;

namespace VKSdk.Support
{
    public class VKHandAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler//, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Serializable]
        public enum HandAction
        {
            Up,
            Down,
            Next,
            Back, 
            Click
        }

        // public 
        public float maxRange;
        public float maxTimeClick;

        public bool allowNextBack;
        public bool allowUpDown;

        [HideInInspector]
        public HandAction handAction;

        // callback
        [Serializable]
        public class OnHandActionDelegate : UnityEvent { }
        public OnHandActionDelegate OnHandActionCallback = new OnHandActionDelegate();

        // private
        private Vector3 posDown;
        private DateTime timeDown;
        private bool isCallback;

        public void OnDrag(PointerEventData data)
        {
            if(isCallback)
            {
                return;
            }

            Vector3 posCurrent = transform.InverseTransformPoint(VKLayerController.Instance.GetMousePoint());
            if(allowNextBack)
            {
                float xRange = posCurrent.x - posDown.x;
                if(xRange < -maxRange)
                {
                    isCallback = true;
                    handAction = HandAction.Back;
                    if(OnHandActionCallback != null)
                    {
                        OnHandActionCallback.Invoke();
                    }
                    return;
                }
                else if(xRange > maxRange)
                {
                    isCallback = true;
                    handAction = HandAction.Next;
                    if(OnHandActionCallback != null)
                    {
                        OnHandActionCallback.Invoke();
                    }
                    return;
                }
            }
            if(allowUpDown)
            {
                float yRange = posCurrent.y - posDown.y;
                if(yRange < -maxRange)
                {
                    isCallback = true;
                    handAction = HandAction.Down;
                    if(OnHandActionCallback != null)
                    {
                        OnHandActionCallback.Invoke();
                    }
                    return;
                }
                else if(yRange > maxRange)
                {
                    isCallback = true;
                    handAction = HandAction.Up;
                    if(OnHandActionCallback != null)
                    {
                        OnHandActionCallback.Invoke();
                    }
                    return;
                }
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            posDown = transform.InverseTransformPoint(VKLayerController.Instance.GetMousePoint());
            timeDown = DateTime.Now;

            isCallback = false;
        }
        
        public void OnPointerUp(PointerEventData data)
        {
            if(!isCallback && (DateTime.Now - timeDown).TotalSeconds < maxTimeClick)
            {
                isCallback = true;
                handAction = HandAction.Click;
                if(OnHandActionCallback != null)
                {
                    OnHandActionCallback.Invoke();
                }
            }
        }

    }
}
