using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using VKSdk.UI;
public class LevelSelector : MonoBehaviour
{
    private int selectedLevel;
    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI textSurvive;
    [SerializeField] private TextMeshProUGUI textDie;
    [SerializeField] private TextMeshProUGUI textRuby;
    [SerializeField] private GameObject lockedObj;
    [SerializeField] private GameObject iconLock;
    [SerializeField] private GameObject panelUnLockObj;
    [SerializeField] private Image blueRuby;
    [SerializeField] private TextMeshProUGUI textTittle;
    [SerializeField] private TextMeshProUGUI textTittleLock;
    private LevelObject levelObj;

    public int SelectedLevel { get => selectedLevel; set  { selectedLevel = value; StaticData.LEVEL = value; } }

    void Start()
    {

    }
    
    public void OpenScene()
    {
        AudioManager.instance.Play("ButtonClick");
        if (levelObj.status)
        {
            VkAdjustTracker.TrackProgressStart("survival", levelObj.name, levelObj.level);
            SelectedLevel = levelObj.level;
            LoadScene.Instance.LoadSceneAndLoading("Level_" + SelectedLevel);

            return;
        }
        else
        {
            if (levelObj.level > UserData.Instance.GameData.level)
            {
                int previous = levelObj.level - 1;
                LevelData levelPrevious = InitData.Instance.GetLevelData(previous, 1);
                UIPopup.OpenPopup("NOTIFY", string.Format("You must pass "+levelPrevious.name+" stage."), false);
                return;
            }
            if (UserData.Instance.GameData.ruby < 10)
            {
                UIPopup.OpenPopup("NOTIFY", string.Format("Not enough ruby!", levelObj.name),(isOK)=> {
                    VKLayerController.Instance.ShowLayer("UIPopupGetMoreRuby");
                },false);
                return;
            }
            UIPopup.OpenPopup("Unlock", "Do you want to unlock: " + levelObj.name, "  10", "", (b) =>
            {
                if (b)
                {
                    lockedObj.SetActive(false);
                    AudioManager.instance.Play("UnlockAchievement");
                    UserData.Instance.GameData.ruby -= 10;
                    VkAdjustTracker.TrackResourceLose("ruby", 10, UserData.Instance.GameData.ruby, "unlock_level");
                    NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                    UserData.Instance.OpenLevelSurvival(levelObj.level);
                    UserData.Instance.SaveLocalData();
                    var layer = VKSdk.UI.VKLayerController.Instance.ShowLayer("UICongratulation") as UICongratulation;
                    layer.Init(string.Format("You unlocked successful: " + levelObj.name));
                }
            }, true, null);
            var uiPopup = VKSdk.UI.VKLayerController.Instance.GetLayer("UIPopup") as UIPopup;
            uiPopup.SetIconImg("ruby");
        }
    }
    public void LateUpdate()
    {
        textRuby.text = UserData.Instance.GameData.ruby.ToString() + "/10";
        blueRuby.fillAmount = (float)UserData.Instance.GameData.ruby / 10;
    }
    public void InitDatas(LevelObject levelObject)
    {
        this.levelObj = levelObject;
        textSurvive.text = string.Format("Survive: {0}", this.levelObj.survive);
        textDie.text = string.Format("Die: {0}", this.levelObj.die);
        lockedObj.SetActive(!levelObj.status);
        textTittle.text = levelObject.name;
        textTittleLock.text = "Spend 10 ruby to unlock this level";

        if (levelObj.survive >= 1)
        {
            panelUnLockObj.SetActive(true);
            iconLock.SetActive(false);
        }
    }

}
