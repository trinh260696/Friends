using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPolicy : Panel
{
    public void OnPolicyClick(int index)
    {
        string[] links = { Config.LINK_POLICY, Config.LINK_TERMS };
        GameHelper.OpenUrl(links[index]);
    }

    public void OnAcceptClick()
    {
        PlayerPrefs.SetInt(Config.KEY_POLICY, 1);
        if (MyApp.Instance.mOpenAtt == 1)
        {
            MyApp.Instance.ActionATT();
        }
        else
        {
            DialogStack.Instance.Hide<DialogPolicy>();
        }
    }
}
