using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VKSdk;
using VKSdk.UI;
using TMPro;

public class UIPopup : VKSdk.UI.VKLayer
{
    #region Properties
    [Space(20)]
    [SerializeField] private TextMeshProUGUI txtInfo;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtBtOk;
    [SerializeField] private TextMeshProUGUI txtBtCancel;

    [SerializeField] private Button btOk;
    [SerializeField] private Button btActionCancel;
    [SerializeField] private Button btCancel;

    [SerializeField] private GameObject gButtonGroup;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Sprite[] sprIcons;

    public string content;

    private System.Action<bool> ResultAction;
    private System.Action closeAction;
    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();
        canvas.sortingLayerName = "UI";
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();

        gButtonGroup.SetActive(false);

        btOk.gameObject.SetActive(false);
        btActionCancel.gameObject.SetActive(false);
        btCancel.gameObject.SetActive(false);
    }
    #endregion

    #region Listener
    public void BtOkClickListener()
    {
        AudioManager.instance.Play("ButtonClick");
        Close();
        if (ResultAction != null)
            ResultAction.Invoke(true);

       
    }

    public void BtCancelClickListener()
    {
        AudioManager.instance.Play("ButtonClick");
        Close();
        if (!btActionCancel.gameObject.activeSelf && ResultAction != null)
            ResultAction.Invoke(false);
        if (closeAction != null)
            closeAction.Invoke();
       
    }

    public void BtActionCancelClickListener()
    {
        AudioManager.instance.Play("ButtonClick");
        Close();
        if (ResultAction != null)
            ResultAction.Invoke(false);

        
    }

    #endregion

    #region Method
    public void ShowPopup(string title = "Thông báo", string strInfo = "", string strBtOK = "", string strBtClose = "", System.Action<bool> action = null, bool isClose = false, System.Action closeAction = null)
    {
        this.ResultAction = action;
        this.content = strInfo;
        this.closeAction = closeAction;

        imgIcon.gameObject.SetActive(false);
        txtInfo.text = strInfo;
        txtTitle.text = title;

        if (!string.IsNullOrEmpty(strBtOK) || !string.IsNullOrEmpty(strBtClose))
        {
            gButtonGroup.SetActive(true);

            if (!string.IsNullOrEmpty(strBtOK))
            {
                btOk.gameObject.SetActive(true);
                txtBtOk.text = strBtOK;
            }

            if (!string.IsNullOrEmpty(strBtClose))
            {
                btActionCancel.gameObject.SetActive(true);
                txtBtCancel.text = strBtClose;
            }
            // txtInfo.transform.localPosition = new Vector3(0, 15f, 0);
        }
        else
        {
            gButtonGroup.SetActive(true);
            // txtInfo.transform.localPosition = new Vector3(0, -10f, 0);
        }

        btCancel.gameObject.SetActive(isClose);
    }

    private void SetIcon(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            imgIcon.sprite = sprIcons[0];
            return;
        }
        if (sprIcons.Length == 0) return;
        if (title.Equals("THÔNG BÁO"))
            imgIcon.sprite = sprIcons[1];
        else if (title.Equals("LỖI"))
            imgIcon.sprite = sprIcons[1];
        else
            imgIcon.sprite = sprIcons[0];
    }
    public void SetIconImg(string name)
    {
        if (imgIcon)
        {
            imgIcon.gameObject.SetActive(true);
            imgIcon.sprite = Resources.Load<Sprite>("Item/" + name);
        }
    }
    #endregion

    #region Open Popup
    public static void OpenPopup(string title, string content, bool isClose = true)
    {
        if (VKLayerController.Instance == null) return;
        
        UIPopup uiPopup = VKLayerController.Instance.GetLayer<UIPopup>();
        if (uiPopup != null && uiPopup.content.Equals(content))
            return;
        ((UIPopup)VKLayerController.Instance.ShowLayer("UIPopup")).ShowPopup(title: title, strInfo: content, isClose: isClose);
    }

    public static void OpenPopup(string title, string content, Action<bool> action, bool isClose)
    {
        if (VKLayerController.Instance == null) return;

        UIPopup uiPopup = VKLayerController.Instance.GetLayer<UIPopup>();
        if (uiPopup != null && uiPopup.content.Equals(content))
            return;
        ((UIPopup)VKLayerController.Instance.ShowLayer("UIPopup")).ShowPopup(title: title, strInfo: content, strBtOK: "OK", action: action, isClose: isClose);
    }

    public static void OpenPopup(string title, string content, string strBtOk, string strBtCancel, Action<bool> action, bool isClose, Action closeAct = null)
    {
        if (VKLayerController.Instance == null) return;

        UIPopup uiPopup = VKLayerController.Instance.GetLayer<UIPopup>();
        if (uiPopup != null && uiPopup.content.Equals(content))
            return;

        ((UIPopup)VKLayerController.Instance.ShowLayer("UIPopup")).ShowPopup(title: title, strInfo: content, strBtOK: strBtOk, strBtClose: strBtCancel, action: action, isClose: isClose, closeAction: closeAct);
    }
    #endregion
}




