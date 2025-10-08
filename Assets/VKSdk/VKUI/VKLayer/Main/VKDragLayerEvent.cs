using UnityEngine;
using UnityEngine.EventSystems;

namespace VKSdk.UI
{
    public class VKDragLayerEvent : MonoBehaviour
    {
        public EventTrigger trigger;

        public Vector2 paddingTB;
        public Vector2 paddingLR;
        public RectTransform rectScreen;

        public bool allowLocalSort;
        public bool allowGlobalSort;

        public CanvasGroup canvasGroup;

        private Vector2 maxPos;
        private Vector2 minPos;

        private Vector3 offset;

        [HideInInspector]
        public bool isStop;

        void Start()
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.InitializePotentialDrag;
            entry3.callback.AddListener((data) => { OnInitializePotentialDrag((PointerEventData)data); });
            trigger.triggers.Add(entry3);

            float haftWidth = rectScreen.sizeDelta.x / 2;
            float haftHeigh = rectScreen.sizeDelta.y / 2;

            // cal max
            minPos = new Vector2(-haftWidth + paddingLR.x, -haftHeigh + paddingTB.x);
            maxPos = new Vector2(haftWidth - paddingLR.y, haftHeigh - paddingTB.y);
        }

        public void OnDrag(PointerEventData data)
        {
            if (isStop)
                return;
            Vector3 v = VKLayerController.Instance.GetMousePoint();
            transform.position = new Vector3(v.x, v.y, transform.position.z) + offset;
            transform.localPosition = FixedMove();
        }

        public void OnInitializePotentialDrag(PointerEventData data)
        {
            if (allowLocalSort)
                transform.SetAsLastSibling();
            if (allowGlobalSort)
                VKLayerController.Instance.FocusMiniGame(this);

            offset = gameObject.transform.position - VKLayerController.Instance.GetMousePoint();
            offset = new Vector3(offset.x, offset.y, 0);
        }

        Vector3 FixedMove()
        {
            float posX = transform.localPosition.x;
            if (posX > maxPos.x)
                posX = maxPos.x;
            else if (posX < minPos.x)
                posX = minPos.x;

            float posY = transform.localPosition.y;
            if (posY > maxPos.y)
                posY = maxPos.y;
            else if (posY < minPos.y)
                posY = minPos.y;

            return new Vector3(posX, posY);
        }
    }
}