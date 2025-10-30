using NewLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  
    public static Vector2 Win_Pos;
    public Transform BoxPointGroup;
    public Transform AllyPointGroup;
    public Transform BossPointGroup;
    public Transform StartupTransform;
    public FriendManager friendManager;
    public List<NPCObj> Friends;
    public UIPlay uiPlay;
    private Vector2[] BossWaypoints;
    private Vector2[] AllyWaypoints;
    private Stack<Vector2> BossPointsStack;
    private List<Vector2> BossListPos;
    private Stack<Vector2> AllyPointsStack;
    
    //List<int[]> HamiltonAllyCycle;
    // List<int[]> HamiltonBossCycle;
    List<Vector2> BoxPoints;
    
    public Sprite[] allSprites;
    public int world=1;
    public int level=1;
    public LevelData levelData;
    private Player player;
    private GameObject winAreaObject;
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
        BossWaypoints = new Vector2[BossPointGroup.childCount];
        AllyWaypoints = new Vector2[AllyPointGroup.childCount];
        BossListPos = new List<Vector2>();
        for(int i=0; i<BossPointGroup.childCount; i++)
        {
            BossWaypoints[i] = (Vector2)BossPointGroup.GetChild(i).position;
            BossListPos.Add(BossWaypoints[i]);
        }
        for(int i=0; i<AllyPointGroup.childCount; i++)
        {
            AllyWaypoints[i] = (Vector2)AllyPointGroup.GetChild(i).position;
        }
        BoxPoints = new List<Vector2>();
        BossPointsStack = new Stack<Vector2>();
        AllyPointsStack = new Stack<Vector2>();
        winAreaObject = GameObject.Find("WinArea");
        GameManager.Win_Pos = (Vector2)winAreaObject.transform.position;
        count = 0;
        StaticData.ISREADY = false;
    }
    void Start()
    {
        if (BaitManager.Instance == null)
        {
            gameObject.AddComponent<BaitManager>();
        }
        allSprites = Resources.LoadAll<Sprite>("Avatar/Avatar");
        levelData = InitData.Instance.GetLevelData(StaticData.LEVEL, 1);
        Field field = new Field();
        field.width = levelData.WIDTH;
        field.height = levelData.HEIGHT;
        field.chips = new int[field.width, field.height];
        for(int i= 0; i < field.width; i++)
            for (int j = 0; j < field.height; j++)
            {
                field.chips[i, j] = i*field.width+j;
            }
        FieldAssistant.main.CreateField(field);
        StaticData.TIME_MAX = levelData.timeout;             
        StaticData.IsPlay = false;
        StaticData.startUpPoint = (Vector2)StartupTransform.position;
       
        NotificationCenter.DefaultCenter().AddObserver(this, "Victory");
        NotificationCenter.DefaultCenter().AddObserver(this, "ChipPlaced");
        
        BeginGame();
        Invoke("ReadyGame", 0.2f);       
    }
    // Update is called once per frame
   
    #region Method
    
    public List<Vector2> GetAllyWaypoints()
    {
        return new List<Vector2>(AllyWaypoints);
    }
    public Vector2 GetBossPos()
    {
        if (BossListPos.Count == 0) return Vector2.zero;
        int rnd = UnityEngine.Random.Range(0, BossListPos.Count);
        Vector3 pos = (Vector3)BossListPos[rnd];
        BossListPos.RemoveAt(rnd);
        return pos;
    }
    public List<Vector2> GetBossPath(Transform bossTransform, out int s, out int f)
    {
        if (BossPointsStack.Count == 0)
        {
            GenerateDesBoss();
        }
       
        Vector2 boxPos = BossPointsStack.Pop();
        s = GetIndexPositionByBoss((Vector2)bossTransform.position);
        f = GetIndexPositionByBoss(boxPos);
        List<Vector2> result = new List<Vector2>();
        return new List<Vector2>(BossWaypoints);
    }
    private int GetIndexPositionByBoss(Vector2 BosPosition)
    {
        float minDistance = 10000f;
        int currentIndex=0;
        for (int i = 0; i < BossWaypoints.Length; i++)
        {
            float distance = Vector2.Distance(BosPosition, BossWaypoints[i]);
            var raycastHit = Physics2D.Raycast(BosPosition, BossWaypoints[i] - BosPosition, distance, 1<<13|1<<1);
            if (raycastHit.collider == null)
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentIndex = i;
                }
            }
        }
        return currentIndex;
    }

    public void Hideplayer()
    {
        player.playerNPC.ActiveDecoy();
    }
    public void OnPauseGame()
    {
        StopCoroutine("SessionGame");
        StopCoroutine("ReviveSession");
        uiPlay.StopTimeCountdown();
        friendManager.Stop();
        StaticData.IsPlay = false;
        NotificationCenter.DefaultCenter().PostNotification(this, "OnFinish");
    }
    public void OnResumeGame()
    {
        NotificationCenter.DefaultCenter().PostNotification(this, "BossStartGame");
        StaticData.IsPlay = true;
        uiPlay.UIReviveGame(currentTime);
        StartCoroutine("ReviveSession");      
        //revive
        player.RevivePlayer();
        friendManager.Recover();
    }

    IEnumerator SessionGame()
    {
        StaticData.ISREADY = true;
        currentTime = StaticData.TIME_MAX + GameManager.Instance.addTime;
        totalTime = currentTime;
        while (currentTime>=0)
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
        var layerBoosterTime = VKLayerController.Instance.ShowLayer("UIPopupMoreBoosterItem") as UIPopupMoreBoosterItem;
        layerBoosterTime.Init();
    }
    IEnumerator ReviveSession()
    {
        while (currentTime >= 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
            totalTime += 1f;
        }
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }
        uiPlay.OnTimeUp();
        // player.TurnOnLose();
        NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnLose");
        NotificationCenter.DefaultCenter().PostNotification(this, "OnFinish");
        StaticData.IsPlay = false;
        yield return new WaitForSeconds(2f);
        var layerBoosterTime = VKLayerController.Instance.ShowLayer("UIPopupMoreBoosterItem") as UIPopupMoreBoosterItem;
        layerBoosterTime.Init();
    }
    IEnumerator RecoverTime()
    {
        currentTime = 15f;
        yield return new WaitForSeconds(1f);
        player.RecoverPlayer();
        friendManager.Recover();
        while (currentTime >= 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
            totalTime += 1f;
            if ((int)(currentTime)%5==0 )
            {
                friendManager.PlayEmotionTimeout();
            }
        }
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }
        uiPlay.OnTimeUp();
        player.TurnOnLose();
        friendManager.Stop();
        NotificationCenter.DefaultCenter().PostNotification(this, "OnFinish");
        StaticData.IsPlay = false;
        yield return new WaitForSeconds(2f);
        var layerBoosterTime = VKLayerController.Instance.ShowLayer("UIPopupMoreBoosterItem") as UIPopupMoreBoosterItem;
        layerBoosterTime.Init();
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
        NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnLose");
    }
    public void Revive()
    {
        NotificationCenter.DefaultCenter().PostNotification(this, "BossStartGame");
        StaticData.IsPlay = true;
        uiPlay.UIReviveGame(currentTime);
        StartCoroutine("ReviveSession");
       // RegenerateBox();
        //revive
        player.RevivePlayer();
        friendManager.Recover();
    }
    public void Recover()
    {
        NotificationCenter.DefaultCenter().PostNotification(this, "BossStartGame");
        StaticData.IsPlay = true;
        uiPlay.UIReviveGame(15);
        StartCoroutine("RecoverTime");
     //   RegenerateBox();
        //revive
        
    }
    public void ProcessLose()
    {
        if (!StaticData.IsPlay) return;
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }
        player.playerNPC.PlayLoseEmotion();
        friendManager.PlayEmotionLose();
        uiPlay.OnEndGame();
        StopCoroutine("SessionGame");
        StopCoroutine("RecoverTime");
        StopCoroutine("ReviveSession");
        StartCoroutine("process_lose");
        StaticData.IsPlay = false;
    }
    public void ProcessDeath()
    {
        if (!StaticData.IsPlay) return;
        var uiSetting = VKLayerController.Instance.GetLayer("UISetting");
        if (uiSetting)
        {
            uiSetting.Close();
        }
        uiPlay.ClearUser();
        uiPlay.OnEndGame();
        StopCoroutine("SessionGame");
        StopCoroutine("RecoverTime");
        StopCoroutine("ReviveSession");
        StartCoroutine("process_lose");
        StaticData.IsPlay = false;
    }
    public List<Vector2> GetAllyWaypoints(Transform allyTransform, out Vector2 BoxPos,out Vector2 centerPos)
    {
            
        List<Vector2> result = new List<Vector2>(AllyWaypoints);

        if(AllyPointsStack.Count==0)
        {
            GenerateDesAlly();
        }
        if (AllyPointsStack.Count == 0)

        {
            centerPos = (Vector2)StartupTransform.position;
            BoxPos = Vector2.zero;
            return result;
        }
                
        BoxPos = AllyPointsStack.Pop();
        centerPos = (Vector2)StartupTransform.position;
        return result;
    }
    private int GetIndexPositionByAlly(Vector2 allyPos)
    {
        float minDistance = 10000f;
        int Index = -1;
        for (int i = 0; i < AllyWaypoints.Length; i++)
        {
            float distance = Vector2.Distance(allyPos, AllyWaypoints[i]);
            var raycastHit = Physics2D.Raycast(allyPos, AllyWaypoints[i] - allyPos, distance, 1<<13|1<<1);
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
       
        NotificationCenter.DefaultCenter().PostNotification(this, "PopulatePos");
        GenerateFriends();    
        uiPlay = VKLayerController.Instance.ShowLayer("UIPlay") as UIPlay;
        var cFriends = ConvertFriendList(Friends);
        uiPlay.Init(cFriends,levelData);
        GeneratePlayer();
        
    }
    List<CFriendObj> ConvertFriendList(List<NPCObj> list)
    {
        List<CFriendObj> result = new List<CFriendObj>();
        for(int i=0; i<list.Count; i++)
        {
            var cFriendObj = new CFriendObj { id = (int)list[i].friendType, number = 0 };
            result.Add(cFriendObj);
        }
        return result;
    }
    public void ReadyGame()
    {
        StaticData.IsPlay = true;

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
        GenerateDesBoss();
        StartCoroutine("SessionGame");
        NotificationCenter.DefaultCenter().PostNotification(this, "FriendStartGame");
        
        uiPlay.UIEnterGame();
        yield return new WaitForSeconds(1f);
        NotificationCenter.DefaultCenter().PostNotification(this, "BossStartGame");
    }
    private void GenerateFriends()
    {
        Friends = FriendData.Instance.GetGroupFriend(levelData.friendNumber);
        friendManager.GenerateFriend(Friends,StartupTransform.position);
    }
    private void GeneratePlayer()
    {
        var go = ContentAssistant.Instance.GetItem("User",StartupTransform.position+Vector3.left*3f,Quaternion.identity);
        player = go.GetComponent<Player>();
        
        player.playerNPC.Init( UserData.Instance.GameData.currentSkin.SkinObject.name);
        player.variableJoystick = uiPlay.variableJoystick;
        
        FieldAssistant.main.player = player;
        
        player.InitPlayer();
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
        // Sprite[] allSprites = Resources.LoadAll<Sprite>("Avatar/Avatar");
        for (int iIndex = 0; iIndex < levelData.WIDTH; iIndex++)
        {
            for (int j = 0; j < levelData.HEIGHT; j++)
            {
               int ID= iIndex * levelData.WIDTH + j;
                var go = ContentAssistant.Instance.GetItem<BodyPart>("BodyPart");
                
                go.InitBodyPart(iIndex * levelData.WIDTH + j, allSprites[ID], BoxPointGroup);
                go.transform.position = BoxPoints[rnd];
                rnd = (rnd + 1) % BoxPoints.Count;
            }
        }

    }
    public void RegenerateBox(Vector2 pos, int ID)
    {
        foreach (Transform tr in BoxPointGroup)
        {
            if (!tr.gameObject.activeSelf)
            {
                if (tr.GetComponent<BodyPart>().ID == ID)
                {
                    tr.gameObject.SetActive(true);
                    tr.GetComponent<BodyPart>().ReActive();
                    tr.position = (Vector3)pos;
                    break;
                }
            }
            
        }
    }
    public void GenerateDesAlly()
    {          
        AllyPointsStack.Clear();
        for(int i=0; i<BoxPointGroup.childCount; i++)
        {
            var go = BoxPointGroup.GetChild(i).GetComponent<BodyPart>();
            if(go)
            if (go.Free)
            {
                var pos_vect2 = (Vector2)go.transform.position;
                AllyPointsStack.Push(pos_vect2);
            }             
        }
    }
        
    private void GenerateDesBoss()
    {
        BossPointsStack.Clear();
        for(int i=0; i<BoxPoints.Count; i++)
        {
            BossPointsStack.Push(BoxPoints[i]);
        }
    }
   
    int count = 0;
    public void ChipPlaced(Notification notification)
    {
        player.playerNPC.PlayEmotionGetItem();
        player.playerNPC.UserComeback();
    }
    public void OnSortComplete()
    {
        if (!StaticData.IsPlay) return;

        ContentAssistant.Instance.GetItem("Confetti Cannon Ribbon - Heart", winAreaObject.transform.position);
        AudioManager.instance.Play(string.Format("gru{0}", Random.Range(1, 11)));             
        GenerateDesAlly();
    }
    public void OnVictory()
    {
        friendManager.PlayEmotionWin();
        player.playerNPC.PlayWinEmotion();
        AudioManager.instance.Play("StateWin");
        StaticData.IsPlay = false;
        StopAllCoroutines();
        UserData.Instance.UpdateLevelWin(StaticData.LEVEL);
        NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnWin");
        NotificationCenter.DefaultCenter().PostNotification(this, "OnFinish");
        player.TurnOnWin();
        StopCoroutine("SessionGame");
        uiPlay.OnEndGame();
        LeanTween.move(cCamera.gameObject, winAreaObject.transform.position - Vector3.forward * 10, 2f);
        winAreaObject.GetComponent<WinAreaScript>().WinEffect();
        Invoke("ShowUIWin", 6f);
    }
    void ShowUIWin()
    {
        
        StopAllCoroutines();
        NotificationCenter.DefaultCenter().PostNotification(this, "OnFinish");
        AudioManager.instance.Play("Win");
        var uiLayer = VKLayerController.Instance.ShowLayer("UIYouSuvived") as UIYouSuvived;
        uiLayer.Init("");
    }
    void ShowUILose()
    {
        
        StopAllCoroutines();
        NotificationCenter.DefaultCenter().PostNotification(this, "OnFinish");
        AudioManager.instance.Play("Lose");
        var uiLayer = VKLayerController.Instance.ShowLayer("UIReviveDialog") as UIReviveDialog;
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
    
  

    public void OnVictory(Notification notification = null)
    {
        StaticData.IsPlay = false;
        StopCoroutine("SessionGame");
        
        friendManager.PlayEmotionWin();
        player.playerNPC.PlayWinEmotion();
        
        NotificationCenter.DefaultCenter().PostNotification(this, "TurnOnWin");
        uiPlay.OnVictory();
    }

    #region Booster
    public void MoreSpeed()
    {
        player.BoostSpeed();
    }
    public void MoreTime()
    {
        this.addTime = 20f;
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
    private void OnDrawGizmos()
    {
        for(int i=0; i<BossPointGroup.childCount-1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(BossPointGroup.GetChild(i).position, 0.2f);
            for(int j= i+1; j< BossPointGroup.childCount; j++)
            {
                // if (i < j)
                //     Gizmos.DrawLine(BossPointGroup.GetChild(i).position, BossPointGroup.GetChild(j).position);
            }          
        }
        for (int i = 0; i < AllyPointGroup.childCount - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(AllyPointGroup.GetChild(i).position, 0.2f);
            for (int j = i + 1; j < AllyPointGroup.childCount; j++)
            {
                if (i < j)
                    Gizmos.DrawLine(AllyPointGroup.GetChild(i).position, AllyPointGroup.GetChild(j).position);
            }
        }
    }
}


