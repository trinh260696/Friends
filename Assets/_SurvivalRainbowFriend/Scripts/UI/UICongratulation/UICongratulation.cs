using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
public class UICongratulation : VKLayer
{
    public TMPro.TextMeshProUGUI contentText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init( string content)
    {
        this.contentText.text = content;
    }
    public void OnClickClose()
    {
        AudioManager.instance.Play("ButtonClick");
        Close();
    }
}
