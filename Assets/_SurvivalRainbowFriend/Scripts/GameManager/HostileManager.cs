using NewLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using System.Linq;
public class HostileManager : MonoBehaviour
{
    public static HostileManager Instance;
    public static Vector2 Win_PosA;
    public static Vector2 Win_PosB;
    public static Vector2 Win_Pos;
    
    public Transform BoxPointGroup;
    public Transform AllyPointGroup;
   
    public Transform StartupTransformBlue;
    public Transform StartupTransformNPC;
    public FriendManager friendManager;
    public List<FriendObj> Friends;
    public UIHostilePlay uiPlay;
    
    private Vector2[] AllyWaypoints;
    
    private Stack<Vector2> AllyPointsStack;

    //List<int[]> HamiltonAllyCycle;
    // List<int[]> HamiltonBossCycle;
    List<Vector2> BoxPoints;

    public List<ItemObject> StatusMissions;
    public int world = 1;
    public int level = 1;
    public LevelData levelData;
    private BluePlayer player;
    private GameObject winAreaObjectA;
    private GameObject winAreaObjectB;
    private Camera cCamera;
    public float addSpeed = 0f;
    public float addTime = 0f;
    public bool Lightdown = true;
    public float rateMoreItem = 0f;
    public float currentTime = 0f;
    public float totalTime = 0f;
    private void Awake()
    {
        Instance = this;
        cCamera = Camera.main;    
        AllyWaypoints = new Vector2[AllyPointGroup.childCount];
       
        for (int i = 0; i < AllyPointGroup.childCount; i++)
        {
            AllyWaypoints[i] = (Vector2)AllyPointGroup.GetChild(i).position;
        }
        BoxPoints = new List<Vector2>();        
        AllyPointsStack = new Stack<Vector2>();
        winAreaObjectA = GameObject.Find("WinAreaA");
        winAreaObjectB = GameObject.Find("WinAreaB");
        HostileManager.Win_PosA = (Vector2)winAreaObjectA.transform.position;
        HostileManager.Win_PosB = (Vector2)winAreaObjectB.transform.position;
        count = 0;
        StaticData.ISREADY = false;
    }
    // Start is called before the first frame update
    void Start()
    {

        levelData = InitData.Instance.GetLevelData(StaticData.LEVEL, 1);
        StaticData.TIME_MAX = levelData.timeout;
        StaticData.IsPlay = false;
        StaticData.startUpPoint = (Vector2)StartupTransformBlue.position;
        StatusMissions = new List<ItemObject>();
        for (int i = 0; i < levelData.Items.Count; i++)
        {
            StatusMissions.Add(new ItemObject { missionNumber = 0, missionMax = 0, typeMission = levelData.Items[i].typeMission });
        }
        BeginGame();
        Invoke("ReadyGame", 0.1f);
    }
    // Update is called once per frame

    #region Method

    public List<Vector2> GetAllyWaypoints()
    {
        return new List<Vector2>(AllyWaypoints);
    }
    IEnumerator SessionGame()
    {
        StaticData.ISREADY = true;
        currentTime = StaticData.TIME_MAX + HostileManager.Instance.addTime;
        totalTime = currentTime;
        while (currentTime >= 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
            totalTime = currentTime;
            if (currentTime <= 20)
            {
                friendManager.PlayEmotionTimeout();
            }
        }
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }
        //player.TurnOnLose();

        uiPlay.OnTimeUp();
        friendManager.Stop();
        StaticData.IsPlay = false;
        yield return new WaitForSeconds(2f);
        ShowUIWin();
        TurnWin();
    }   
    IEnumerator process_lose()
    {
        yield return new WaitForSeconds(2f);
        //NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnLose");       
        friendManager.Stop();
        yield return new WaitForSeconds(1f);
        ShowUILose();
    }
    public void TurnLose()
    {
        player.TurnOnLose();
        NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnWin");
    }
    public void TurnWin()
    {
        player.TurnOnWin();
        
    }


    public void ProcessLose()
    {
        if (!StaticData.IsPlay) return;
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }
        
        friendManager.PlayEmotionWin();
        uiPlay.OnEndGame();
        StopCoroutine("SessionGame");     
        StartCoroutine("process_lose");
        StaticData.IsPlay = false;
    }
    
    public List<Vector2> GetAllyWaypoints(Transform allyTransform, out Vector2 BoxPos, out Vector2 centerPos)
    {

        List<Vector2> result = new List<Vector2>(AllyWaypoints);

        if (AllyPointsStack.Count == 0)
        {
            GenerateDesAlly();
        }
        if (AllyPointsStack.Count == 0)

        {
            centerPos = (Vector2)StartupTransformBlue.position;
            BoxPos = Vector2.zero;
            return result;
        }

        BoxPos = AllyPointsStack.Pop();
        centerPos = (Vector2)StartupTransformBlue.position;
        return result;
    }
    private int GetIndexPositionByAlly(Vector2 allyPos)
    {
        float minDistance = 10000f;
        int Index = -1;
        for (int i = 0; i < AllyWaypoints.Length; i++)
        {
            float distance = Vector2.Distance(allyPos, AllyWaypoints[i]);
            var raycastHit = Physics2D.Raycast(allyPos, AllyWaypoints[i] - allyPos, distance, 1 << 13 | 1 << 1);
            if (raycastHit.collider == null)
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                    Index = i;
                }
            }
        }
        return Index;
    }
    public void BeginGame()
    {
        //GenerateHamiltonBossPath();
        // GenerateHamiltonAllyPath();
        StaticData.IsPlay = true;
        NotificationCenter.DefaultCenter().PostNotification(this, "PopulatePos");
        GenerateFriends();
        uiPlay = VKLayerController.Instance.ShowLayer("UIHostilePlay") as UIHostilePlay;
        var cFriends = ConvertFriendList(Friends);
        uiPlay.Init(cFriends, levelData.Items);
        GeneratePlayer();

    }
    List<CFriendObj> ConvertFriendList(List<FriendObj> list)
    {
        List<CFriendObj> result = new List<CFriendObj>();
        for (int i = 0; i < list.Count; i++)
        {
            var cFriendObj = new CFriendObj { id = i, skinId = list[i].skinId, number = 0 };
            result.Add(cFriendObj);
        }
        return result;
    }
    public void ReadyGame()
    {
        //StaticData.IsPlay = true;

        friendManager.NotifyMakeNoise();
    }
    public void EnterGame()
    {
        StartCoroutine("enum_enter");

    }
    IEnumerator enum_enter()
    {
        AudioManager.instance.Play("OpenDoor");
        NotificationCenter.DefaultCenter().PostNotification(this, "OpenDoor");
        yield return new WaitForSeconds(1f);
        GenerateBox();
        GenerateDesAlly();
        StartCoroutine("SessionGame");
        NotificationCenter.DefaultCenter().PostNotification(this, "FriendStartGame");

        uiPlay.UIEnterGame();
        yield return new WaitForSeconds(1f);
        NotificationCenter.DefaultCenter().PostNotification(this, "BossStartGame");
    }
    private void GenerateFriends()
    {
        Friends = FriendData.Instance.GetGroupFriend(levelData.friendNumber);
        friendManager.GenerateFriend(Friends, StartupTransformNPC.position);
    }
    private void GeneratePlayer()
    {
        var go = Instantiate(Resources.Load<GameObject>("Friends/BluePlayer"), StartupTransformBlue.position, Quaternion.identity);
        player = go.GetComponent<BluePlayer>();                          
        player.variableJoystick = uiPlay.variableJoystick;
       
    }
    private void GenerateBox()
    {
        int length = BoxPointGroup.childCount;
        foreach (Transform tr in BoxPointGroup)
        {
            BoxPoints.Add((Vector2)tr.position);
            Destroy(tr.gameObject);
        }

        int rnd = Random.Range(0, BoxPointGroup.childCount);

        for (int iIndex = 0; iIndex < levelData.Items.Count; iIndex++)
        {
            for (int j = 0; j < levelData.Items[iIndex].missionNumber + 2; j++)
            {
                Debug.Log(levelData.Items[iIndex].typeMission.ToString());
                var prefBox = Resources.Load<GameObject>("Box/" + levelData.Items[iIndex].typeMission.ToString());
                var go = Instantiate(prefBox, BoxPoints[rnd], Quaternion.identity, BoxPointGroup) as GameObject;
                go.transform.position = BoxPoints[rnd];
                rnd = (rnd + 1) % BoxPoints.Count;
            }
        }

    }
    public void RegenerateBox(Vector2 pos, TypeMission typeMission)
    {
        foreach (Transform tr in BoxPointGroup)
        {
            if (!tr.gameObject.activeSelf)
            {
                if (tr.GetComponent<BoxScript>().typeMission == typeMission)
                {
                    tr.gameObject.SetActive(true);
                    tr.GetComponent<BoxScript>().ReActive();
                    tr.position = (Vector3)pos;
                    break;
                }
            }

        }
    }
    public void GenerateDesAlly()
    {
        AllyPointsStack.Clear();
        for (int i = 0; i < BoxPointGroup.childCount; i++)
        {
            var go = BoxPointGroup.GetChild(i).GetComponent<BoxScript>();
            if (go)
                if (go.IsAlive)
                {
                    var pos_vect2 = (Vector2)go.transform.position;
                    AllyPointsStack.Push(pos_vect2);
                }
        }
    }
   
    int count = 0;
    public void AddMission(TypeMission typeMission, Vector3 winPos, bool sideA)
    {
        if (!StaticData.IsPlay) return;
        
       
        ContainAssistant.Instance.GetItem("Confetti Cannon Ribbon - Heart", winPos);
        Vector2 detectVec = winPos - player.transform.position;

        if (detectVec.magnitude < 10f)
        {

            AudioManager.instance.Play(string.Format("gru{0}", Random.Range(1, 11)));
        }
        count = Mathf.Min(count, 3);
        if (sideA)
            winAreaObjectA.GetComponent<WinAreaScript>().UpdateSprite(count);
        else
            winAreaObjectB.GetComponent<WinAreaScript>().UpdateSprite(count);
        count++;
        for (int i = 0; i < StatusMissions.Count; i++)
        {
            if (StatusMissions[i].typeMission == typeMission)
            {
                StatusMissions[i].missionNumber++;
                break;
            }
        }
        if (CheckWin())
        {          
            AudioManager.instance.Play("StateWin");
            StaticData.IsPlay = false;
            StopAllCoroutines();
            UserData.Instance.UpdateLevelWin(StaticData.LEVEL);
            NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnWin");
            player.TurnOnLose();
            StopCoroutine("SessionGame");
            uiPlay.OnEndGame();          
            Invoke("ShowUILose", 6f);

        }
        GenerateDesAlly();
    }
    bool CheckWin()
    {
        for (int i = 0; i < StatusMissions.Count; i++)
        {
            if (StatusMissions[i].missionNumber < levelData.Items[i].missionNumber)
            {
                return false;
            }
        }

        return true;
    }
    void ShowUIWin()
    {

        StopAllCoroutines();
        
        AudioManager.instance.Play("Win");
        var uiLayer = VKLayerController.Instance.ShowLayer("UIYouSuvived") as UIYouSuvived;
        uiLayer.Init("");
    }
    void ShowUILose()
    {

        StopAllCoroutines();       
        AudioManager.instance.Play("Lose");
        var uiLayer = VKLayerController.Instance.ShowLayer("UIYouDiedDialog") as UIReviveDialog;
    }
    public void RemoveFriend(int name)
    {
        uiPlay.RemoveFriend(name);
    }
    public void UpdateFriend(int name)
    {
        uiPlay.UpdateFriend(name);
    }
    public void CloseGame()
    {
        uiPlay.OnClose();
    }
    #endregion
    #region Booster
    public void MoreSpeed()
    {
        player.BoostSpeed();
    }
    
    public void TurnOnLight()
    {
        player.LightUp();
    }
    public void MoreItem()
    {
        this.rateMoreItem = 0.5f;
    }
    #endregion
}


