using UnityEngine;

namespace VKSdk.Support
{
    public class VKAutoRotate : MonoBehaviour
    {

        public Vector3 velocity;

        public bool isRotate = true;
        public bool isLocal = true;

        // Update is called once per frame
        void Update()
        {
            if (isRotate)
            {
                if (isLocal)
                {
                    this.transform.localEulerAngles += velocity * Time.deltaTime;
                }
                else
                {
                    this.transform.eulerAngles += velocity * Time.deltaTime;
                }
            }

        }

        public void StartRotate()
        {
            isRotate = true;
        }

        public void StopRotate()
        {
            isRotate = false;
        }

        public void Reload()
        {
            gameObject.transform.localEulerAngles = Vector3.zero; ;
        }

    }
}
