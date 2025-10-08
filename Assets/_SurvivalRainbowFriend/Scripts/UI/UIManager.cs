using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.Rendering.FilterWindow;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    private int Index;
   // private GameObject StartMainUI;

    //public List<GameObject> ListUI = new List<GameObject>();

    public GameObject SettingUI;
    public GameObject SpinUI;
    public GameObject SkinUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
        //StartMainUI = Resources.Load<GameObject>($"UI/Layer/StartMainUI");
        //Instantiate(StartMainUI, transform.position, Quaternion.identity);
        //StartMainUI.SetActive(true);
    }
    private void Start()
    {
        SetActiveObj();
    }
    void SetActiveObj()
    {
        Index = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(Index);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainStartUI");
    }
    public void Retry()
    {
        SceneManager.LoadScene(Index);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(Index + 1);
    }

    public void SettingBtnClick()
    {
        SettingUI.SetActive(true);
    }
    public void SpinBtnClick()
    {
        SpinUI.SetActive(true);
    }
    public void BackBtnClick()
    {

    }

}
