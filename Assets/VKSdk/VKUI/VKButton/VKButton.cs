using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKButton : Button
    {
        public List<Image> imageAll;
        public List<Text> textAll;
        public List<Outline> outlineTextAll;

        public List<Color> colorImageAll;
        public List<Sprite> spriteImageAll;

        public List<Color> colorTextAll;
        public List<string> valueTextAll;
        public List<Color> colorOutlineTextAll;

        public List<GameObject> goActives;
        public List<GameObject> goInActives;


        private List<Color> baseColorImageAll;
        private List<Sprite> baseSpriteImageAll;
        private List<Color> baseColorTextAll;
        public List<string> baseValueTextAll;
        public List<Color> baseColorOutlineTextAll;

        protected override void Awake()
        {
            base.Awake();

            if (imageAll != null && imageAll.Count > 0)
            {
                baseColorImageAll = new List<Color>();
                baseSpriteImageAll = new List<Sprite>();

                for (int i = 0; i < imageAll.Count; i++)
                {
                    if (imageAll[i] != null)
                    {
                        baseColorImageAll.Add(imageAll[i].color);
                        baseSpriteImageAll.Add(imageAll[i].sprite);
                    }
                    else
                    {
                        baseColorImageAll.Add(Color.white);
                        baseSpriteImageAll.Add(null);
                    }
                }
            }

            if (textAll != null && textAll.Count > 0)
            {
                baseColorTextAll = new List<Color>();
                baseValueTextAll = new List<string>();

                for (int i = 0; i < textAll.Count; i++)
                {
                    if (textAll[i] != null)
                    {
                        baseColorTextAll.Add(textAll[i].color);
                        baseValueTextAll.Add(textAll[i].text);
                    }
                    else
                    {
                        baseColorTextAll.Add(Color.white);
                        baseValueTextAll.Add("");
                    }
                }
            }

            if (outlineTextAll != null && outlineTextAll.Count > 0)
            {
                baseColorOutlineTextAll = new List<Color>();

                for (int i = 0; i < outlineTextAll.Count; i++)
                {
                    if (outlineTextAll[i] != null)
                    {
                        baseColorOutlineTextAll.Add(outlineTextAll[i].effectColor);
                    }
                    else
                    {
                        baseColorOutlineTextAll.Add(Color.white);
                    }
                }
            }
        }

        public bool VKInteractable
        {
            get
            {
                return interactable;
            }
            set
            {
                if (value != interactable)
                {
                    interactable = value;
                    OnInteractableChanged();
                }
            }
        }

        protected void OnInteractableChanged()
        {
            SetupAll(interactable);
        }

        public void SetupAll(bool isEnable)
        {
            if(goActives != null && goActives.Count > 0 && goInActives != null && goInActives != null)
            {
                goActives.ForEach(a => a.SetActive(isEnable));
                goInActives.ForEach(a => a.SetActive(!isEnable));
            }

            if (imageAll != null && imageAll.Count > 0)
            {
                for (int i = 0; i < imageAll.Count; i++)
                {
                    if (imageAll[i] != null)
                    {
                        if (colorImageAll != null && colorImageAll.Count > i)
                        {
                            imageAll[i].color = isEnable ? baseColorImageAll[i] : colorImageAll[i];
                        }

                        if (spriteImageAll != null && spriteImageAll.Count > i)
                        {
                            imageAll[i].sprite = isEnable ? baseSpriteImageAll[i] : spriteImageAll[i];
                        }
                    }
                }
            }

            if (textAll != null && textAll.Count > 0)
            {
                for (int i = 0; i < textAll.Count; i++)
                {
                    if (textAll[i] != null)
                    {
                        if (colorTextAll != null && colorTextAll.Count > i)
                        {
                            textAll[i].color = isEnable ? baseColorTextAll[i] : colorTextAll[i];
                        }

                        if (valueTextAll != null && valueTextAll.Count > i)
                        {
                            textAll[i].text = isEnable ? baseValueTextAll[i] : valueTextAll[i];
                        }
                    }
                }
            }

            if (outlineTextAll != null && outlineTextAll.Count > 0)
            {
                for (int i = 0; i < outlineTextAll.Count; i++)
                {
                    if (outlineTextAll[i] != null)
                    {
                        if (colorOutlineTextAll != null && colorOutlineTextAll.Count > i)
                        {
                            outlineTextAll[i].effectColor = isEnable ? baseColorOutlineTextAll[i] : colorOutlineTextAll[i];
                        }
                    }
                }
            }
        }
    }
}
