using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadingScene : MonoBehaviour {
    public static LoadingScene Instance;
    AsyncOperation sceneAO;
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingProgbar;
    //[SerializeField] Text loadingText;

    private void Awake()
    {
        Instance = this;
    }
    private const float LOAD_READY_PERCENTAGE = 0.9f;

    public void ChangeScene(string sceneName)
    {
        loadingUI.SetActive(true);
        //loadingText.text = "LOADING...";
        LoadingSceneRealProgress(sceneName);
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
        }
        else
        {
            CancelInvoke();
            Time.timeScale = 1f;
            loadingUI.SetActive(false);
            loadingProgbar.fillAmount = 0;
        }
    }
}
