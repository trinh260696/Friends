using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKTextRun : MonoBehaviour
    {
        public RectTransform rectContain;
        public RectTransform rectText;

        public TMPro.TMP_Text txtTMPContain;
        public Text txtContain;
        public float speed;

        public bool isAutoHide;
        public GameObject goHide;

        public bool isHaveAnim;
        public Animator aminShowHide;

        private float widthText;

        private float xStart;
        private float xEnd;

        void OnEnable()
        {
            if(rectText.rect.size.x > 10f)
            {
                StartMove();
            }
        }

        void OnDisable()
        {
            StopMove();
        }

        // public string strTest = "";
        // [ContextMenu("INITTEST")]
        // public void InitTest()
        // {
        //     Init(strTest);
        // }

        public void Init(string contain)
        {
            StopMove(true);

            if(string.IsNullOrEmpty(contain)) return;
            
            if(txtTMPContain != null) txtTMPContain.text = contain;
            if(txtContain != null) txtContain.text = contain;

            LeanTween.delayedCall(gameObject, 1f, () => {
                float sizeText = rectText.rect.size.x;
                xStart = rectContain.rect.size.x/2f + sizeText/2 + 50;
                xEnd = -xStart;

                rectText.transform.localPosition = new Vector3(xStart, 0f, 0f);
                StartMove();
            });
        }

        public void StartMove()
        {
            float time = (rectText.transform.localPosition.x-xEnd)/speed;
            LeanTween.moveLocalX(rectText.gameObject, xEnd, time).setOnComplete(() => {
                StopMove();
                if(isHaveAnim)
                {
                    aminShowHide.SetTrigger("hide");
                }
                else if(isAutoHide) 
                {
                    goHide.SetActive(false);
                }
                else
                {
                   rectText.transform.localPosition = new Vector3(xStart, 0f, 0f);
                    StartMove(); 
                }
            });
        }

        public void StopMove(bool reset = false)
        {
            LeanTween.cancel(gameObject);
            LeanTween.cancel(rectText.gameObject);
            if(reset) rectText.localPosition = new Vector3(100000f, 0f, 0f);
        }

        public void OnAnimHideDone()
        {
            goHide.SetActive(false);
        }

    }
}
