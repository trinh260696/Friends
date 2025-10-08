using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.Support
{
    public class VKAutoHideScrollBar : MonoBehaviour
    {
        public Scrollbar scrollBar;

        public float rangeChange;
        public float timeShow;
        public float timeHide;

        public float lastValue;

        public Color colorHide; 
        private Color colorDefault;

        public void OnValueChange()
        {
            print("value: " + scrollBar.value);
            print("Mathf.Abs(value-lastValue): " + Mathf.Abs(scrollBar.value-lastValue));
            if(Mathf.Abs(scrollBar.value-lastValue) > rangeChange)
            {
                lastValue = scrollBar.value;
            }
        }
    }
}
