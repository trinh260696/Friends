using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VKSdk.UI;

public class LoadScene : MonoBehaviour
{
    public static LoadScene Instance;
    AsyncOperation sceneAO;
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingProgbar;
    public Transform displayTr;
    // [SerializeField] Text loadingText;  
    private void Start()

    {
        Instance = this;

    }
    private const float LOAD_READY_PERCENTAGE = 0.9f;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    void LoadingSceneRealProgress(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 1f;
        loadingProgbar.fillAmount = 0f;
        InvokeRepeating("loading_run", 1f, 0.01f);
    }
    void loading_run()
    {
        if (loadingProgbar.fillAmount < 1f)
        {
            loadingProgbar.fillAmount += 0.01f;
            displayTr.localPosition += new Vector3(3f, 0f, 0f);
        }
        else
        {
            CancelInvoke();
            Time.timeScale = 1f;
            loadingUI.SetActive(false);
            loadingProgbar.fillAmount = 0;
        }
    }
    public void LoadSceneAndLoading(string name)
    {
        if (name == "MainStartUI")
        {
            ChangeScene(name);
            AudioManager.instance.PlayMusic(false);
        }
        else
        {
            AudioManager.instance.PlayMusic(true);
            loadingUI.SetActive(true);
           
            var scaleFactor = VKLayerController.GetScale(Screen.width, Screen.height, new Vector2(1920, 1080));
            
            if (UserData.Instance.GameData.vip == 0)
            {
                scaleFactor = scaleFactor * 0.88f;
            }
            loadingUI.GetComponent<CanvasScaler>().scaleFactor = scaleFactor;
            displayTr.localPosition = Vector3.zero;
            // loadingText.text = "LOADING...";
            LoadingSceneRealProgress(name);
        }
       
    }
   

}
