using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VKSdk;
using VKSdk.Support;

namespace VKSdk.UI
{
    public class VKBannerSlide : MonoBehaviour
    {
        #region Properties
        [Space(20)]
        public VKHandAction vkHandAction;

        [Space(20)]
        public Image imgBannerFakeTop;
        public Image imgBannerFakeBottom;
        public List<Image> imgBanners;
        public List<Button> btDots;
        public List<GameObject> goLoadings;

        [Space(20)]
        public GameObject goImgBanner;

        [Space(20)]
        public float width;
        public float timeMove;
        public float countDownDefault;
        public float delayFirst;

        // private 
        [HideInInspector]
        public int indexClick;
        [HideInInspector]
        public int indexCurrent;
        private float countDown;

        private List<string> urlBanners;
        private List<Texture2D> texBanners;

        private IEnumerator ieAutoSlide;

        // callback
        [Serializable]
        public class OnBannerClickDelegate : UnityEvent { } // code, status, data, baseData
        public OnBannerClickDelegate OnBannerClick = new OnBannerClickDelegate();

        [Serializable]
        public class OnBannerSlideDelegate : UnityEvent { } // code, status, data, baseData
        public OnBannerSlideDelegate OnBannerSlide = new OnBannerSlideDelegate();
        #endregion

        #region Unity Method
        void OnDestroy()
        {
            ClearTexBanners();
        }
        #endregion

        #region Listener
        public void BtDotClickListener(int index)
        {
            if (index != indexCurrent)
            {
                SlideBannner(index, true);
            }
        }

        public void BtNextClickListener()
        {
            int index = indexCurrent + 1;
            if (index >= urlBanners.Count) index = 0;

            SlideBannner(index);
        }

        public void BtBackClickListener()
        {
            int index = indexCurrent - 1;
            if (index < 0) index = urlBanners.Count - 1;

            SlideBannner(index);
        }

        public void BtBannerClickListener(int index)
        {
            indexClick = index;
            if (OnBannerClick != null)
            {
                OnBannerClick.Invoke();
            }
        }

        public void OnHandActionListener()
        {
            switch (vkHandAction.handAction)
            {
                case VKHandAction.HandAction.Click:
                    BtBannerClickListener(indexCurrent);
                    break;
                case VKHandAction.HandAction.Next:
                    BtBackClickListener();
                    break;
                case VKHandAction.HandAction.Back:
                    BtNextClickListener();
                    break;
            }
        }
        #endregion

        #region Method
        public void Init(List<string> urls)
        {
            if(urlBanners != null && urlBanners.Count == urls.Count)
            {
                // check same
                bool isSame = true;
                for(int i = 0; i < urlBanners.Count; i++)
                {
                    if(!urlBanners[i].Equals(urls[i]))
                    {
                        isSame = false;
                        break;
                    }
                }
                if(isSame) 
                {
                    StartAutoSlide(true);
                    return;
                }
            }

            // clear old tex
            ClearTexBanners();

            urlBanners = urls;

            indexCurrent = 0;

            imgBanners.ForEach(a => a.gameObject.SetActive(false));
            btDots.ForEach(a => a.gameObject.SetActive(false));
            goLoadings.ForEach(a => a.SetActive(false));

            goImgBanner.transform.localPosition = Vector3.zero;
            
            for (int i = 0; i < urls.Count; i++)
            {
                var imgBanner = imgBanners[i];
                var goLoading = goLoadings[i];

                Image imgBannerFake = null;
                if(i == 0) imgBannerFake = imgBannerFakeBottom;
                else if(i == urls.Count - 1) imgBannerFake = imgBannerFakeTop;

                btDots[i].gameObject.SetActive(true);
                btDots[i].interactable = true;

                imgBanner.transform.localPosition = new Vector3((i * width), 0, 0);
                imgBanner.color = new Color(1f, 1f, 1f, 0f);
                imgBanner.gameObject.SetActive(true);
                goLoading.SetActive(true);

                if(imgBannerFake != null) 
                {
                    imgBannerFake.color = new Color(1f, 1f, 1f, 0f);
                    imgBannerFake.gameObject.SetActive(true);
                }

                // load imag
                string[] strs = urls[i].Split('/');
                string filename = strs[strs.Length - 1];

                Texture2D texture2D = null;
                if (!string.IsNullOrEmpty(filename) && FileHelper.CheckFileExists(filename))
                {
                    texture2D = FileHelper.LoadTexture(filename);
                }

                if (texture2D == null)
                {
                    StartCoroutine(VKCommon.DownloadImageFromURL(urls[i], (texture2DNew) =>
                    {
                        if (texture2DNew != null)
                        {
                            FileHelper.SaveTextureToFile(texture2DNew, filename);
                            goLoading.SetActive(false);

                            imgBanner.sprite = VKCommon.ConvertTexture2DSprite(texture2DNew);
                            imgBanner.color = Color.white;

                            if(imgBannerFake != null)
                            {
                                imgBannerFake.sprite = imgBanner.sprite;
                                imgBannerFake.color = Color.white;
                            }

                            texBanners.Add(texture2DNew);
                        }
                    }));
                }
                else
                {
                    goLoading.SetActive(false);

                    imgBanner.sprite = VKCommon.ConvertTexture2DSprite(texture2D);
                    imgBanner.color = Color.white;

                    if(imgBannerFake != null)
                    {
                        imgBannerFake.sprite = imgBanner.sprite;
                        imgBannerFake.color = Color.white;
                    }

                    texBanners.Add(texture2D);
                }
            }

            // img fake position
            imgBannerFakeTop.transform.localPosition = new Vector3(imgBanners[0].transform.localPosition.x - width, 0, 0);
            imgBannerFakeBottom.transform.localPosition = new Vector3(imgBanners[urlBanners.Count - 1].transform.localPosition.x + width, 0, 0);

            // default
            btDots[0].interactable = false;

            StartAutoSlide();
        }

        public void SlideBannner(int index, bool ignoreFake = false)
        {
            bool isMoveFake = false;
            float xMoveFake = 0;
            if (!ignoreFake)
            {
                int indexMax = urlBanners.Count - 1;
                if (indexMax > 1 && indexCurrent == 0 && index == indexMax)
                {
                    // fake 1 cai slide top
                    // imgBannerFake.sprite = imgBanners[indexMax].sprite;
                    // imgBannerFake.transform.localPosition = new Vector3(imgBanners[0].transform.localPosition.x - width, 0, 0);
                    isMoveFake = true;
                    xMoveFake = width;
                }
                else if (indexCurrent == indexMax && index == 0)
                {
                    // imgBannerFake.sprite = imgBanners[0].sprite;
                    // imgBannerFake.transform.localPosition = new Vector3(imgBanners[indexMax].transform.localPosition.x + width, 0, 0);
                    isMoveFake = true;
                    xMoveFake = -((indexCurrent + 1) * width);
                }
            }

            btDots[indexCurrent].interactable = true;
            btDots[index].interactable = false;

            indexCurrent = index;

            LeanTween.cancel(goImgBanner);
            if (isMoveFake)
            {
                LeanTween.moveLocalX(goImgBanner, xMoveFake, timeMove).setOnComplete(() =>
                {
                    goImgBanner.transform.localPosition = new Vector3(-(indexCurrent * width), goImgBanner.transform.localPosition.y, goImgBanner.transform.localPosition.z);
                });
            }
            else
            {
                LeanTween.moveLocalX(goImgBanner, -(index * width), timeMove);
            }

            countDown = countDownDefault - timeMove;

            if (OnBannerSlide != null)
            {
                OnBannerSlide.Invoke();
            }
        }

        public void ClearTexBanners()
        {
            if(texBanners != null && texBanners.Count > 0)
            {
                try
                {
                    for(int i = 0; i < texBanners.Count; i++)
                    {
                        if(texBanners[i] != null)
                        {
                            Destroy(texBanners[i]);
                        }
                    }
                }
                catch {}
                texBanners.Clear();
            }
            else
            {
                texBanners = new List<Texture2D>();
            }
        }

        public void StartAutoSlide(bool ignoreDelay = false)
        {
            StopAutoSlide();

            ieAutoSlide = IEAutoSlider(ignoreDelay);
            StartCoroutine(ieAutoSlide);
        }

        public void StopAutoSlide()
        {
            if (ieAutoSlide != null)
            {
                StopCoroutine(ieAutoSlide);
                ieAutoSlide = null;
            }
        }
        #endregion

        #region IE
        public IEnumerator IEAutoSlider(bool ignoreDelay)
        {
            yield return new WaitForEndOfFrame();
            if(!ignoreDelay)
            {
                yield return new WaitForSeconds(delayFirst);
            }

            while (true)
            {
                yield return new WaitForEndOfFrame();
                countDown -= Time.deltaTime;

                if (countDown <= 0)
                {
                    int index = indexCurrent + 1;
                    if (index >= urlBanners.Count) index = 0;

                    SlideBannner(index);
                }
            }
        }
        #endregion
    }
}