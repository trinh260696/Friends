using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowScript : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    // public Vector2Int[] defaultWindowSizeLandscapes;
    // public Vector2Int[] defaultWindowSizePortraits;
 
    public Vector2Int defaultWindowSize;
    public Vector2Int borderSize;

    [Space(20)]
    public GameObject goControl;
    public float timeMove;

    [Space(20)]
    public Vector3 vHide;

    [Space(20)]
    public bool autoHideBar;
    public bool autoHideControl;

    private Vector2 _deltaValue = Vector2.zero;
    private bool _maximized;


    public void Awake()
    {
#if UNITY_STANDALONE
        DontDestroyOnLoad(this.transform.parent);
#else
        DestroyImmediate(transform.parent.gameObject);
#endif
    }

    public void Start()
    {
#if UNITY_STANDALONE

        if(Application.isEditor) 
        {
            goControl.transform.localPosition = autoHideControl ? vHide : Vector3.zero;
            return; //We don't want to hide the toolbar from our editor!
        }
        
        if(autoHideBar)
        {
            StartCoroutine(WaitHideToolBar());
        }
#endif
    }

    IEnumerator WaitHideToolBar()
    {
        yield return new WaitForEndOfFrame();
        OnNoBorderBtnClick();

        yield return new WaitForEndOfFrame();
        ResetWindowSize();
    }

    public void OnBorderBtnClick()
    {
        if (BorderlessWindow.framed)
            return;

        BorderlessWindow.SetFramedWindow();        
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width + borderSize.x, Screen.height + borderSize.y); // Compensating the border offset.
    }

    public void OnNoBorderBtnClick()
    {
        if (!BorderlessWindow.framed)
            return;

        BorderlessWindow.SetFramelessWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width - borderSize.x, Screen.height - borderSize.y);
    }

    public void ResetWindowSize()
    {
        bool isLanscape = Screen.currentResolution.width > Screen.currentResolution.height;
        float ratio = ((float)defaultWindowSize.x)/((float)defaultWindowSize.y);
        float height = Screen.currentResolution.height * (isLanscape ? 0.9f : 0.7f);
        float width = height * ratio;

        Vector2Int pos = new Vector2Int(0, (int)((float)Screen.currentResolution.height/100f));

        // BorderlessWindow.MoveWindowPos(Vector2Int.zero, defaultWindowSize.x, defaultWindowSize.y);
        BorderlessWindow.MoveWindowPos(pos, (int)width, (int)height);
    }

    public void OnCloseBtnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
    }

    public void OnMinimizeBtnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        BorderlessWindow.MinimizeWindow();
    }

    public void OnMaximizeBtnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (_maximized)
            BorderlessWindow.RestoreWindow();
        else
            BorderlessWindow.MaximizeWindow();

        _maximized = !_maximized;
    }

    public void OnDrag(PointerEventData data)
    {
        if (BorderlessWindow.framed)
            return;

        _deltaValue += data.delta;
        if (data.dragging)
        {
            BorderlessWindow.MoveWindowPos(_deltaValue, Screen.width, Screen.height);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        LeanTween.cancel(goControl);
        LeanTween.moveLocal(goControl, Vector3.zero, timeMove);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        LeanTween.cancel(goControl);
        LeanTween.moveLocal(goControl, vHide, timeMove);
    }
}
