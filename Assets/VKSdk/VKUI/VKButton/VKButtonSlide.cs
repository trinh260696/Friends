using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKButtonSlide : MonoBehaviour
    {
        public GameObject goSelected;

        public List<VKButton> buttons;
        public List<Transform> transButtons;
#if VK_TMP_ACTIVE
        public List<TMPro.TMP_Text> txtButtons;
#else
        public List<Text> txtButtons;
#endif

        public Color cTextNormal;
        public Color cTextSelected;

        public float timeChange;

        [Space(10)]
        public bool scaleSelected;
        public float scaleTarget;
        public float scaleTime;

        [Space(10)]
        public int lastIndex;
        public int currentIndex;

        // callback
        [Serializable]
        public class OnTabChangeDelegate : UnityEvent { } // code, status, data, baseData
        public OnTabChangeDelegate OnTabChange = new OnTabChangeDelegate();

        public void OnDestroy()
        {
            CancelAll();
        }

        public void ButtonSelectedClickListener(int index)
        {
            if(currentIndex == index)
            {
                return;
            }

            CancelAll();
            lastIndex = currentIndex;
            currentIndex = index;

            if(timeChange > 0f)
            {
                if(goSelected != null)
                {
                    LeanTween.moveLocal(goSelected, transButtons[index].localPosition, timeChange-0.01f);
                }
                if (scaleSelected)
                {
                    LeanTween.scale(transButtons[index].gameObject, Vector3.one * scaleTarget, scaleTime);
                }
                
                LeanTween.value(gameObject, (value) => {
                    Color cNormal = Color.Lerp(cTextSelected, cTextNormal, value);
                    Color cSelected = Color.Lerp(cTextNormal, cTextSelected, value);

                    for(int i = 0; i < txtButtons.Count; i++)
                    {
                        if(i == currentIndex)
                        {
                            txtButtons[i].color = cSelected;
                        }
                        else if(i == lastIndex)
                        {
                            txtButtons[i].color = cNormal;
                        }
                    }

                }, 0f, 1f, timeChange).setOnComplete(() => {
                    SetupAll();
                });
            }
            else
            {
                SetupAll();
            }

            if(OnTabChange != null)
            {
                OnTabChange.Invoke();
            }
        }

        public void Init(int index)
        {
            lastIndex = currentIndex;
            currentIndex = index;
            CancelAll();
            SetupAll();

            DelaySelectPosition();
        }

        public void SetupAll()
        {
            if(currentIndex < 0)
            {
                return;
            }

            if(goSelected != null)
            {
                goSelected.transform.localPosition = transButtons[currentIndex].localPosition;
            }
            
            if(buttons.Count > 0)
            {
                buttons.ForEach(a => a.SetupAll(true));
                buttons[currentIndex].SetupAll(false);
            }
            
            if(txtButtons.Count > 0)
            {
                txtButtons.ForEach(a => a.color = cTextNormal);
                txtButtons[currentIndex].color = cTextSelected;
            }
            if (scaleSelected)
            {
                if(lastIndex >= 0)
                {
                    transButtons[lastIndex].localScale = Vector3.one;
                }
                transButtons[currentIndex].localScale = Vector3.one * scaleTarget;
            }
        }

        public void CancelAll()
        {
            LeanTween.cancel(gameObject);
            if(goSelected != null)
            {
                LeanTween.cancel(goSelected);
            }
        }

        public void Disable()
        {
            if(buttons.Count > 0)
            {
                buttons.ForEach(a => a.enabled = false);
            }
        }

        public void Enable()
        {
            if(buttons.Count > 0)
            {
                buttons.ForEach(a => a.enabled = true);
            }
        }

        public void SetNone(bool keepGoSelect = false)
        {
            if(!keepGoSelect)
            {
                CancelAll();

                if(goSelected != null)
                {
                    goSelected.SetActive(false);
                }

                if(txtButtons.Count > 0 && txtButtons.Count > currentIndex && currentIndex >= 0)
                {
                    txtButtons[currentIndex].color = cTextNormal;
                }
            }
            
            if(buttons.Count > 0)
            {
                buttons.ForEach(a => a.SetupAll(true));
            }

            if (scaleSelected && currentIndex >= 0)
            {
                transButtons[currentIndex].localScale = Vector3.one;
            }
            
            currentIndex = -1;
        }

        private void DelaySelectPosition()
        {
            if(gameObject.activeInHierarchy)
            {
                StartCoroutine(IEDelaySelectPosition());
            }
        }

        private IEnumerator IEDelaySelectPosition()
        {       
            yield return new WaitForEndOfFrame();
            SetupAll();
        }
    }
}