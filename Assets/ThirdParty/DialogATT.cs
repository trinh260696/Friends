using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogATT : Panel
{
    public void OnNoThankClick()
    {
        int count = PlayerPrefs.GetInt("key_count_no_att", 0);
        count++;
        PlayerPrefs.SetInt("key_count_no_att", count);
        if (count < 2)
        {
            MyApp.Instance.OpenTrackATT(0);
        }
        else
        {
            MyApp.Instance.RequestATT();
        }
        DialogStack.Instance.Hide<DialogATT>();
    }

    public void OnYesClick()
    {
        MyApp.Instance.RequestATT();
        DialogStack.Instance.Hide<DialogATT>();
    }
}
