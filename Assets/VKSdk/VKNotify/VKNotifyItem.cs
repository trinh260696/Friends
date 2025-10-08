using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.Notify
{
    public class VKNotifyItem : MonoBehaviour
    {
#if VK_TMP_ACTIVE
        public TMPro.TMP_Text txtNoti;
#else
        public Text txtNoti;
#endif
        public Image imgBackground;
        public CanvasGroup group;

        public Color[] cBackgrounds;
        public Color[] cTexts;

        public float timeDelay;

        public bool isActive;
        private List<NotifyItemData> contents;
        private NotifyItemData currentItem;

        public void Init()
        {
            contents = new List<NotifyItemData>();
            isActive = false;
        }

        public void Show(NotifyItemData item)
        {
            if (gameObject.activeSelf)
            {
                if (!currentItem.content.Equals(item.content))
                {
                    if(contents.Count > 0)
                    {
                        var lastAdd = contents[contents.Count - 1];
                        if (!lastAdd.content.Equals(item.content))
                        {
                            contents.Add(item);
                        }
                    }
                    else
                    {
                        contents.Add(item);
                    }
                }
            }
            else
            {
                contents.Add(item);
                gameObject.SetActive(true);
                isActive = true;

                StartCoroutine(Move());
            }
        }

        IEnumerator Move()
        {
            while (true)
            {
                currentItem = contents[0];
                contents.RemoveAt(0);

                @group.alpha = 0;
                transform.localPosition = new Vector3(0, -100, 0);
#if LANG
                txtNoti.text = VKLanguage.GetI2LanguageTranslation(currentItem.content);
#else
                txtNoti.text = currentItem.content;
#endif

        //        txtNoti.color = cTexts[(int)currentItem.type];
               // imgBackground.color = cBackgrounds[(int)currentItem.type];

                LeanTween.value(this.gameObject, 0, 1, 0.2f).setOnUpdate(delegate (float f)
                {
                    @group.alpha = f;
                });
                yield return new WaitForSeconds(timeDelay + (currentItem.content.Length / 15));

                if (contents.Count <= 0)
                {
                    isActive = false;
                    LeanTween.moveLocalY(this.gameObject, 150, 0.5f).setEase(LeanTweenType.easeInOutBack);
                    yield return new WaitForSeconds(0.6f);
                    gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}