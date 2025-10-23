using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAreaScript : MonoBehaviour
{
    //[SerializeField] Sprite[] sprites;
    //private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        //spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //transform.GetChild(0).gameObject.SetActive(false);
    }
    //public void UpdateSprite(int iIndex)
    //{
    //    transform.GetChild(0).gameObject.SetActive(true);
    //    spriteRenderer.sprite = sprites[iIndex];
    //}
    public void WinEffect()
    {
        StartCoroutine("show_effect");
    }
    IEnumerator show_effect()
    {

        yield return new WaitForSeconds(2f);
        ContentAssistant.Instance.GetItem("2D_Aim_01", transform.position);
        //yield return new WaitForSeconds(3f);
       // transform.GetChild(1).gameObject.SetActive(true);
    }
}
