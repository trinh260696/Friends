using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using TMPro;
using UnityEngine.UI;
using System;

public class UIPlay :VKLayer
{
    [Header("Property")]
    public UIFriendManager uiFriendManager;
    public GameObject startGameObj;
    public TextMeshProUGUI textStartUI;
    public TextMeshProUGUI friendNameWasFound;
    public TextMeshProUGUI timeUp;
    public GameObject gameTimeObj;
    public VKCountDownLite timeCountDown;
    public RectTransform parentMissionTransform;
    public RectTransform findMissionTransform;
    public Sprite[] sprites;
    private Dictionary<int, TextMeshProUGUI> missionTextList;
    public VariableJoystick variableJoystick;
    public UIUser uiUser;
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
    public void Init(List<CFriendObj> CFriends,LevelData levelData)
    {
        canvas.sortingOrder = 100;
        var scaleFactor = VKLayerController.GetScale(Screen.width, Screen.height, new Vector2(1920, 1080));      
        gameObject.GetComponent<CanvasScaler>().scaleFactor = scaleFactor*0.95f;
        uiUser.InitStatusUser();
        uiFriendManager.InitData(CFriends);
        gameTimeObj.SetActive(false);
        startGameObj.SetActive(true);       
        float from =10f;      
        LeanTween.value(textStartUI.gameObject, (value) =>
        {
            if (value < 4)
            {
                textStartUI.text = string.Format("GAME WILL START IN <color=red>00:0{0}</color> SECONDS", (int)(value));
            }
            else
            {
                textStartUI.text = string.Format("GAME WILL START IN <color=green>00:0{0}</color> SECONDS", (int)(value));
            }
           
        }, from, 0f, 10f).setOnComplete(()=> {          
            GameManager.Instance.EnterGame();
            StopCoroutine("Warning");
        });
        StartCoroutine("Warning");
       
        foreach (RectTransform tr in parentMissionTransform)
        {
            tr.gameObject.SetActive(false);
        }
        missionTextList = new Dictionary<int, TextMeshProUGUI>();
        //for(int i=0; i<ItemObjects.Count; i++)
        //{
        //    var tr = parentMissionTransform.GetChild(i);
        //    tr.gameObject.SetActive(true);
        //    int iSprite = (int)ItemObjects[i].typeMission;
        //    tr.GetChild(0).GetComponent<Image>().sprite = sprites[iSprite];
        //    var textMesh = tr.GetChild(1).GetComponent<TextMeshProUGUI>();
        //    textMesh.text = string.Format("0/{0}", ItemObjects[i].missionNumber);
        //    tr.gameObject.name = iSprite.ToString();
        //    missionTextList.Add(iSprite, textMesh);
        //}

        //foreach (RectTransform tr in findMissionTransform)
        //{
        //    tr.gameObject.SetActive(false);
        //}
        //for (int i = 0; i < ItemObjects.Count; i++)
        //{
        //    var tr = findMissionTransform.GetChild(i);
        //    tr.gameObject.SetActive(true);
        //    int iSprite = (int)ItemObjects[i].typeMission;
        //    tr.GetChild(0).GetComponent<Image>().sprite = sprites[iSprite];
        //    tr.gameObject.name = iSprite.ToString();
        //}

        var uiBooster= VKLayerController.Instance.ShowLayer("UIBooster") as UIBooster;
        uiBooster.Init();
    }
    IEnumerator Warning()
    {
        yield return new WaitForSeconds(6f);
        for(int i=0; i<3; i++)
        {
            AudioManager.instance.Play("Warning");
            yield return new WaitForSeconds(1f);
        }
        
    }
    public void UpdateMission(TypeMission type, int number, int total)
    {
        int iType = (int)type;
        missionTextList[iType].text = string.Format("{0}/{1}", number, total);
    }
    public void UpdateMissionUser()
    {
        uiUser.AddItem();
    }
    public void ClearUser()
    {
        uiUser.ClearUser();
    }
    public void ReviveUser()
    {
        uiUser.Revive();
    }
    public void UIEnterGame()
    {
       
        startGameObj.SetActive(false);
        gameTimeObj.SetActive(true);
        timeCountDown.SetSeconds(StaticData.TIME_MAX + GameManager.Instance.addTime);
        timeCountDown.StartCountDown(null);
        timeCountDown.OnShowSpecial = () => { AudioManager.instance.Play("Warning"); };
        //float from = StaticData.TIME_MAX;
        //LeanTween.value(textTimeUI.gameObject, (value) =>
        //{
        //    textTimeUI.text = string.Format("00:{0}", (int)(value));
        //}, from, 0f, StaticData.TIME_MAX);
    }
    public void UIReviveGame(float timeRevive)
    {
        ReviveUser();
        startGameObj.SetActive(false);
        gameTimeObj.SetActive(true);
        timeCountDown.SetSeconds(timeRevive);
        timeCountDown.StartCountDown(null);
        timeCountDown.OnShowSpecial = () => { AudioManager.instance.Play("Warning"); };
    }
    public void RemoveFriend(int name)
    {
        uiFriendManager.ClearFriend(name);

        friendNameWasFound.gameObject.SetActive(true);
        friendNameWasFound.text = string.Format("<color=#D24D4D>{0}</color> Was Found", GameManager.Instance.Friends[name].name.ToString());
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
    public void OnClickHide()
    {
        AudioManager.instance.Play("ButtonClick");
        GameManager.Instance.Hideplayer();
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

    internal void OnVictory()
    {
        
    }
    #endregion
}
