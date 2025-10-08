using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKClockItem : MonoBehaviour
    {
        public Text txt1;
        public Text txt2;
        public Text txt3;

        public Animator anim;

        public float animSpeed = 1;

        public int number;

        public void SetText(int number)
        {
            this.number = number;

            txt1.text = number.ToString();

            txt1.gameObject.SetActive(true);
            txt2.gameObject.SetActive(false);
            txt3.gameObject.SetActive(false);
        }

        public void StartAminText(int number)
        {
            if (this.number == number)
                return;

            this.number = number;

            txt2.text = txt1.text;
            txt3.text = number.ToString();
            txt1.text = number.ToString();

            anim.speed = animSpeed;
            anim.SetTrigger("Change");
        }

        public void SetColor(Color color)
        {
            txt1.color = color;
            txt2.color = color;
            txt3.color = color;
        }
    }
}
