using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
//using Library;
#endif

public class Menu<T> : MonoBehaviour where T : Component
{
    //[SerializeField] private string PrefabResourcesPath = "Prefabs/UI/Panels/";

    private List<Panel> panels = new List<Panel>();

    private Stack<Panel> activing = new Stack<Panel>();
    public int ActivingCount
    {
        get
        {
            return activing.Count;
        }
    }

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<T>();
            if (instance == null)
            {
                GameObject g = new GameObject(typeof(T).Name);
                instance = g.AddComponent<T>();
                g.AddComponent<GraphicRaycaster>();
                g.AddComponent<Canvas>();
                g.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                g.AddComponent<CanvasScaler>();
                CanvasScaler cs = g.GetComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
#if UNITY_EDITOR
                cs.referenceResolution = UnityEditor.Handles.GetMainGameViewSize();
#else
				cs.referenceResolution = new Vector2(Screen.width, Screen.height);
#endif
            }
            return instance;
        }
    }

    public enum ShowType { NotHide, DissmissCurrent, PauseCurrent, Duplicate }

    private void Awake()
    {
        if (instance == null) {

            instance = this as T;
            for(int i=0; i<transform.childCount; i++)
            {
                panels.Add(transform.GetChild(i).GetComponent<Panel>());
            }
        }
        if (instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject);
    }

    public void Show<T>(object data = null, ShowType showType = ShowType.DissmissCurrent) where T : Panel
    {
        Panel p = GetPanel<T>();
        if (p == null) return;

        if (Activing(p))
        {
            if (showType != ShowType.Duplicate) return; // maybe need on top
            p = Instantiate(p.gameObject, transform).GetComponent<T>();
        }

        if (activing.Count > 0)
        {
            if (activing.Peek().isAwayShow)
            {
                showType = ShowType.PauseCurrent;
            }
            if (showType == ShowType.DissmissCurrent) activing.Pop().Hide();
            else if (showType == ShowType.PauseCurrent) activing.Peek().Hide();
        }

        //p.Show(showType);
        p.Show(data);
        activing.Push(p);
    }

    public void Show<T>(out T p, object data = null, ShowType showType = ShowType.DissmissCurrent) where T : Panel
    {
        p = (T)GetPanel<T>();
        if (p == null) return;

        if (Activing(p))
        {
            if (showType != ShowType.Duplicate) return;
            p = Instantiate(p.gameObject, transform).GetComponent<T>();
        }

        if (activing.Count > 0)
        {
            if (activing.Peek().isAwayShow)
            {
                showType = ShowType.PauseCurrent;
            }
            if (showType == ShowType.DissmissCurrent) activing.Pop().Hide();
            else if (showType == ShowType.PauseCurrent) activing.Peek().Hide();
        }

        //p.Show(showType);
        p.Show(data);
        activing.Push(p);
    }

    public void Hide<T>(object data = null) where T : Panel
    {
        if (activing.Count == 0) return;

        Panel p = activing.Peek();
        if (!p.GetType().Name.Equals(typeof(T).Name))
        {
            Debug.LogErrorFormat("[Menu] {0} is not in current activing!", typeof(T).Name);
            return;
        }
        activing.Pop().Hide();
        if (activing.Count > 0) activing.Peek().Show();
    }

    public void HideAll()
    {
        if (activing.Count == 0) return;
        foreach (var p in activing)
        {
            p.Hide();
        }
        activing.Clear();

    }

    public void Back<T>() where T : Panel
    {
        if (activing.Count == 0) return;

        Panel current = activing.Peek();
        if (!current.PhysicBackEnable) return;

        if (activing.Count > 0) activing.Pop().Back();
        if (activing.Count > 0) activing.Peek().Show();
    }

    public bool Activing(Panel panel)
    {
        if (panel == null) return false;
        return activing.Contains(panel);
    }

    public bool CheckActiving()
    {
        return activing.Count > 0 ? true : false;
    }

    private Panel GetPanel<T>() where T : Panel
    {
        var p = panels.Find(i => i.GetType().Name.Equals(typeof(T).Name));
        //if (p == null)
        //{
        //    GameObject prefab = Resources.Load<GameObject>(string.Format("{0}{1}", PrefabResourcesPath, typeof(T).Name));
        //    if (prefab == null)
        //    {
        //        Debug.LogErrorFormat("[Menu] Can't find prefab at path Resources/{0}{1}", PrefabResourcesPath, typeof(T).Name);
        //        return null;
        //    }

        //    p = Instantiate(prefab, transform).GetComponent<T>();
        //    if (p != null) panels.Add(p);
        //}

        if (p == null) Debug.LogErrorFormat("[Menu] {0} is not assign", typeof(T).Name);
        return p;
    }

    public Panel GetPanelByName<T>() where T : Panel
    {
        return panels.Find(i => i.GetType().Name.Equals(typeof(T).Name));
    }
}

/** <summary> Base Panel in UI</summary> */
public class Panel : MonoBehaviour
{
    [HideInInspector] public bool isAwayShow = false;
    [SerializeField] protected bool tapToHide = true;

#if UNITY_ANDROID
    [SerializeField] protected bool physicBackEnable = true;
#endif

    protected bool duplicated = false;

    protected object data = null;

    protected virtual void Start()
    {

    }


    public virtual void Show(object data = null, bool duplicated = false)
    {
        this.duplicated = duplicated;
        this.data = data;
        gameObject.transform.SetAsLastSibling();
        gameObject.SetActive(true);

        //SoundManager.Instance.PlayAudioClip(AudioClipDefine.SHOW_POPUP);
        

    }

    public virtual void Hide(object data = null)
    {
        if (duplicated) Destroy(gameObject);
        else gameObject.SetActive(false);
        //SoundManager.Instance.PlayAudioClip(AudioClipDefine.SHOW_POPUP);
        //EventDispatcher.Instance.Dispatch(EventKey.GEM_CHANGED);
       
    }

    public void Back()
    {
#if UNITY_ANDROID
        if (PhysicBackEnable) Hide();
#endif
    }

    public virtual bool PhysicBackEnable
    {
        get
        {
#if UNITY_ANDROID
            return physicBackEnable;
#else
            return false;
#endif
        }
    }
}