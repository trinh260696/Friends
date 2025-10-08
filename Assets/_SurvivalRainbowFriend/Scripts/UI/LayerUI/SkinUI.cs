using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinUI : MonoBehaviour
{
    UiManager uiManager;
    AudioManager audioManager;

    private void Start()
    {
        uiManager = UiManager.instance;
        audioManager = AudioManager.instance;
    }
    public void BackBtnClick()
    {
        audioManager.Play("ButtonClick");
        Destroy(uiManager.SkinUI, 0.2f);
    }
}
