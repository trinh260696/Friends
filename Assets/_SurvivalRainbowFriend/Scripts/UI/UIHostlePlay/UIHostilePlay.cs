using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VKSdk.UI;

public class UIHostilePlay : VKLayer
{
    [Header("Property")]
    public UIFriendManager uiFriendManager;
    public GameObject startGameObj;
    public TextMeshProUGUI textStartUI;
    public TextMeshProUGUI friendNameWasFound;
    public TextMeshProUGUI timeUp;
    public GameObject gameTimeObj;
    public VKCountDownLite timeCountDown;
   
    public Sprite[] sprites;
    public VariableJoystick variableJoystick;
   
    //public Button btnSetting;
    #region override
    public override void BeforeHideLayer()
    {
        base.BeforeHideLayer();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }

    public override void OnLayerCloseDone()
    {
        base.OnLayerCloseDone();
    }

    public override void OnLayerOpenDone()
    {
        base.OnLayerOpenDone();
    }

    public override void OnLayerOpenPopupDone()
    {
        base.OnLayerOpenPopupDone();
    }

    public override void OnLayerPopupCloseDone()
    {
        base.OnLayerPopupCloseDone();
    }

    public override void OnLayerReOpenDone()
    {
        base.OnLayerReOpenDone();
    }

    public override void OnLayerSlideHideDone()
    {
        base.OnLayerSlideHideDone();
    }

    public override void ReloadCanvasScale(float screenRatio, float screenScale)
    {
        base.ReloadCanvasScale(screenRatio, screenScale);
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
    #endregion

    #region method
    public void Init(List<CFriendObj> CFriends, LevelData levelData)
    {
        canvas.sortingOrder = 100;
        var scaleFactor = VKLayerController.GetScale(Screen.width, Screen.height, new Vector2(1920, 1080));
        gameObject.GetComponent<CanvasScaler>().scaleFactor = scaleFactor * 0.95f;
       
        uiFriendManager.InitData(CFriends);
        gameTimeObj.SetActive(false);
        startGameObj.SetActive(true);
        float from = 10f;
        LeanTween.value(textStartUI.gameObject, (value) =>
        {
            textStartUI.text = string.Format("GAME WILL START IN <color=green>00:0{0}</color> SECONDS", (int)(value));
        }, from, 0f, 10f).setOnComplete(() => {
            //HostileManager.Instance.EnterGame();
        });
       
      

        var uiBooster = VKLayerController.Instance.ShowLayer("UIBooster") as UIBooster;
        uiBooster.Init();
    }
   
   
   
    public void UIEnterGame()
    {

        startGameObj.SetActive(false);
        gameTimeObj.SetActive(true);
      //  timeCountDown.SetSeconds(StaticData.TIME_MAX + HostileManager.Instance.addTime);
        timeCountDown.StartCountDown(null);
        timeCountDown.OnShowSpecial = () => { AudioManager.instance.Play("Warning"); };
        //float from = StaticData.TIME_MAX;
        //LeanTween.value(textTimeUI.gameObject, (value) =>
        //{
        //    textTimeUI.text = string.Format("00:{0}", (int)(value));
        //}, from, 0f, StaticData.TIME_MAX);
    }
    
   
    public void RemoveFriend(int name)
    {
        uiFriendManager.ClearFriend(name);

        friendNameWasFound.gameObject.SetActive(true);
       // friendNameWasFound.text = string.Format("<color=#D24D4D>{0}</color> Was Found", HostileManager.Instance.Friends[name].name.ToString());
        Invoke("HideFriendWasFound", 3f);
    }

    void HideFriendWasFound()
    {
        friendNameWasFound.gameObject.SetActive(false);
    }

    public void OnTimeUp()
    {
        timeUp.gameObject.SetActive(true);
        LeanTween.scale(timeUp.gameObject, Vector2.one * 3, 0);
        LeanTween.scale(timeUp.gameObject, Vector2.one, 0.5f).setEase(LeanTweenType.easeInBack);
        LeanTween.alpha(timeUp.gameObject, 0, 0);
        LeanTween.alpha(timeUp.gameObject, 1, 1);
        Invoke("HideTimeUp", 2f);
    }
    void HideTimeUp()
    {
        timeUp.gameObject.SetActive(false);
    }
    #endregion
    #region Listener
    public void OnClickSetting()
    {
        AudioManager.instance.Play("ButtonClick");
        // GameManager.Instance.OnPauseGame();
        // StopTimeCountdown();
        VKLayerController.Instance.ShowLayer("UISetting");
    }
    public void StopTimeCountdown()
    {
        timeCountDown.StopCountDown();
    }
    public void ResumeTimeCountDown(float time)
    {
        timeCountDown.SetSeconds(time);
        timeCountDown.StartCountDown(null);
        timeCountDown.OnShowSpecial = () => { AudioManager.instance.Play("Warning"); };
    }
    
   
    public void OnEndGame()
    {
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }

        timeCountDown.StopCountDown();
    }
    public void OnClose()
    {
        AudioManager.instance.Play("ButtonClick");
        Close();
    }

    public void UpdateFriend(int name)
    {
        uiFriendManager.UpdateFriend(name);
    }
    #endregion
}
