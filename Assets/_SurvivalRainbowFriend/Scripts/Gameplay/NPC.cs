using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using TMPro;
public class NPC : MonoBehaviour
{
    public int IQ = 1;//1-7
    public int HP = 100;
    public TextMeshProUGUI textName;
    public Animator animator;
    public GameObject emoji;
    public Rigidbody2D Body;//Enemy body
    private Transform Target;//Player transform
    public int ID;
    private StateFriend state = StateFriend.FRIEND_INIT;//If the friend has ceased to see the gift, he will follow on PlayerLastPos
    public float v_npc_normal = 1f;
    public float v_npc_decoy= 3f;
    public float SpeedReal;
    public float Range = 5;
    public BodyPart typeMission;
    public NPCObj friendObj;
    [SerializeField] private SpriteRenderer spriteRendererBox;
    [SerializeField] private Transform decoyTransform;
    private Animator decoyAnimator;
    private Transform enemyTransform;
    private RaycastHit2D hit;//for the visibility system     
    private List<Vector2> Waypoints;
    private List<Vector2> listPathReturn;
    private List<Vector2> listPathDetect;
    private int currentIndex = 0;
    private FieldMini field;
    private Vector2 centerPos;
    private Vector2 winPos;
    private bool sideA = true;
    private const string RunProperties = "run_normal";
    private const string ReturnProperties = "run_return";
    private const string WinProperties = "WinTrigger";
    private const string LoseProperties1 = "HitTrigger1";
    private const string LoseProperties2 = "HitTrigger2";
    private const string FailTrigger = "FailTrigger";
    private const string DieProperties = "die";
    private const string HideProperties = "hide";
    private const string HideTrigger = "HideTrigger";
    private const string RevivePropertis = "ReviveTrigger";
    private const string RunBoosterProperties = "run_booster_return";
  
    private bool run = false;
    private bool box = false;
    private bool hide = false;
    private bool run0 = false;
    private bool boosterRun = false;
    private bool die = false;
    private int walk = -1;
    private bool dangerous = false;
    private float time_wait = 0f;
    private Dictionary<string, bool> DecoyDictionary = new Dictionary<string, bool>
    {
        {"carton",false },{"drum_box",false},{"shove",false},{"snow",false},{ "tree",false},{"tnt_box",false},{"gift_box",false},{"toe",false }
    };
    private int boxIndex = 0;
    private void Start()
    {
        v_npc_decoy = v_npc_normal * 0.4f;
        emoji.gameObject.SetActive(false);
    }

    public void Init(NPCObj friendObj, GameObject animator, int id)
    {
        this.ID = id;
        this.friendObj = friendObj;
        this.animator = animator.GetComponent<Animator>();
        this.animator.transform.localPosition = Vector3.zero;
        this.animator.transform.SetAsFirstSibling();
        this.animator.transform.localScale = Vector3.one * 0.7f;
        spriteRendererBox = this.animator.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRendererBox.gameObject.SetActive(false);
        state = StateFriend.FRIEND_INIT;
        listPathReturn = new List<Vector2>();
        listPathDetect = new List<Vector2>();
        NotificationCenter.DefaultCenter().AddObserver(this, "FriendStartGame");
        NotificationCenter.DefaultCenter().AddObserver(this, "TurnOnWin");
        NotificationCenter.DefaultCenter().AddObserver(this, "TurnOnLose");
        // skeletonMecanim = animator.GetComponent<SkeletonMecanim>();        
        //skeletonMecanim.skeleton.SetSkin(friendObj.skinId);
        if (StaticData.GM_Mode==Mode.BattleMode)
        {
            textName.text = friendObj.name;
        }
        else
        {
            textName.gameObject.SetActive(false);
        }      
        walk = UnityEngine.Random.Range(1, 4);
        RndBox();
    }

    public void PlayLoseEmotion()
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        string anim_name = rnd == 0 ? StaticParam.cry2_emo : StaticParam.cry_emo;
        enum_emo(anim_name);
    }
    public void PlayWinEmotion()
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        string anim_name = rnd == 0 ? StaticParam.smile_emo : StaticParam.poke_emo;
        enum_emo(anim_name);
    }
    public void PlayEmotionTimeout()
    {
        string[] arr = new string[] {StaticParam.tired_emo,StaticParam.hurry_up,StaticParam.scare_emo,StaticParam.run_emo };
        int rnd = UnityEngine.Random.Range(0, 4);
        enum_emo(arr[rnd]);
    }
    public void PlayRndBeginGame()
    {
        string[] arr = new string[] { StaticParam.poke_emo, StaticParam.emo2, StaticParam.tamgiac_emo_begin, StaticParam.tamgiac_emo_loop,StaticParam.smile_emo };
        int rnd = UnityEngine.Random.Range(0, 5);
        enum_emo(arr[rnd]);
    }
    void PlayEmotionGetItem()
    {
        string[] arr = new string[] { StaticParam.here_emo,StaticParam.smile_emo,StaticParam.poke_emo};
        int rnd = UnityEngine.Random.Range(0, 3);
        enum_emo(arr[rnd]);
    }
    private void RndBox()
    {
        int rnd = UnityEngine.Random.Range(0, DecoyDictionary.Count);
        int i = 0;
        foreach (var pair in DecoyDictionary)
        {
            if (i == rnd)
            {
                DecoyDictionary[pair.Key] = true;
                string key= pair.Key;
                decoyAnimator = ContentAssistant.main.GetItem<Animator>(key,decoyTransform.position);
                decoyAnimator.transform.parent = transform;
                decoyAnimator.gameObject.SetActive(false);
                break;
            }
            i++;
        }
    }
    public void StopGame()
    {
        run = false;
        StopCoroutine("DetectGift");
        StopCoroutine("DetectPathReturn");
        LeanTween.cancel(gameObject);
        Target = null;
    }
    public void FriendStartGame()
    {
        // SetupWayPoints();
        if (state == StateFriend.FRIEND_DIE) return;
        StopCoroutine("move_noise");
        LeanTween.cancel(gameObject);
        hide = false;
        run = false;
        SetupDetectPath();
        if (listPathDetect.Count > 0)
        {
            run = true;
            OnPatrolGift();
            //StartCoroutine("DetectGift");
        }
        else
        {
            run = false;
        }
    }
    void SetupDetectPath()
    {
        Vector2 FPos=Vector2.zero;
        if (StaticData.GM_Mode == Mode.SurvivalMode)
        {
            Waypoints = GameManager.Instance.GetAllyWaypoints(transform, out FPos, out centerPos);
        }
        
        //int count = Waypoints.Count;
        //Vector2 dirLast = (FPos - Waypoints[count - 1]).normalized;
        Waypoints.Add(FPos);
        state = StateFriend.FRIEND_PATROL;
        SetCurrentIndex((Vector2)transform.position);
        field = new FieldMini();
        field.WIDTH = Waypoints.Count;
        field.joints = new float[field.WIDTH, field.WIDTH];
        for (int i = 0; i < Waypoints.Count; i++)
            for (int j = i + 1; j < Waypoints.Count; j++)
            {
                float distance = Vector2.Distance((Vector2)Waypoints[i], Waypoints[j]);
                var raycastHit = Physics2D.Raycast(Waypoints[i], Waypoints[j] - Waypoints[i], distance, 1 << 13 | 1 << 1);
                if (raycastHit.collider == null)
                {
                    field.joints[i, j] = Vector2.Distance(Waypoints[i], Waypoints[j]);
                }
                else
                {
                    field.joints[i, j] = DIJKTRA.MAXC;
                }

            }
        DIJKTRA.Init(field);
        DIJKTRA.InputSecond(currentIndex, Waypoints.Count - 1);
        var resultSecond = DIJKTRA.Output();
        if (resultSecond != null)
        {
            listPathDetect.Clear();
            for (int i = 0; i < resultSecond.Length; i++)
            {
                listPathDetect.Add(Waypoints[resultSecond[i]]);
            }
        }
        else
        {
            run = false;
            listPathDetect.Clear();
        }
    }
    void SetKeyAnimations()
    {
        run0 = run && boosterRun;
        animator.SetBool(RunProperties, run);
        animator.SetBool(RunBoosterProperties, run0);
        animator.SetBool(ReturnProperties, run && box);
        animator.SetBool(HideProperties, hide);       
        animator.SetBool(DieProperties, die);
    }

    private void ReturnToBeginPoint()
    {
        if (state == StateFriend.FRIEND_DIE) return;
        if (box)
        {
            if(!hide)
                spriteRendererBox.gameObject.SetActive(true);
            else
                spriteRendererBox.gameObject.SetActive(false);
        }
        
        if (StaticData.GM_Mode == Mode.SurvivalMode)
        {
            Waypoints = GameManager.Instance.GetAllyWaypoints();
            Waypoints.Add(GameManager.Win_Pos);
        }
        
        SetCurrentIndex((Vector2)transform.position);
        field = new FieldMini();
        field.WIDTH = Waypoints.Count;
        field.joints = new float[field.WIDTH, field.WIDTH];
        for (int i = 0; i < Waypoints.Count; i++)
            for (int j = i + 1; j < Waypoints.Count; j++)
            {
                float distance = Vector2.Distance((Vector2)Waypoints[i], Waypoints[j]);
                var raycastHit = Physics2D.Raycast(Waypoints[i], Waypoints[j] - Waypoints[i], distance, 1 << 13 | 1 << 1);
                if (raycastHit.collider == null)
                {
                    field.joints[i, j] = Vector2.Distance(Waypoints[i], Waypoints[j]);
                }
                else
                {
                    field.joints[i, j] = DIJKTRA.MAXC;
                }

            }
        DIJKTRA.Init(field);
        DIJKTRA.InputSecond(currentIndex, Waypoints.Count - 1);
        var resultSecond = DIJKTRA.Output();
        if (resultSecond != null)
        {
            listPathReturn.Clear();
            for (int i = 0; i < resultSecond.Length; i++)
            {
                listPathReturn.Add(Waypoints[resultSecond[i]]);
            }
            run = true;
            OnGoPathReturn();
            //StartCoroutine("DetectPathReturn");
        }
        else
        {
            run = false;
        }
    }
    private void SetCurrentIndex(Vector2 currentPosition)
    {

        float minDistance = 10000f;
        currentIndex = 0;
        for (int i = 0; i < Waypoints.Count; i++)
        {
            float distance = Vector2.Distance(currentPosition, Waypoints[i]);
            var raycastHit = Physics2D.Raycast(currentPosition, Waypoints[i] - currentPosition, distance, 1 << 13 | 1 << 1);
            if (raycastHit.collider == null)
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentIndex = i;
                }
            }
        }
    }
    private int GetIndex(Vector2 currentPosition)
    {
        float minDistance = 10000f;
        int result = -1;
        for (int i = 0; i < Waypoints.Count; i++)
        {
            float distance = Vector2.Distance(currentPosition, Waypoints[i]);
            var raycastHit = Physics2D.Raycast(currentPosition, Waypoints[i] - currentPosition, distance, 1 << 13 | 1 << 1);
            if (raycastHit.collider == null)
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = i;
                }
            }
        }
        return result;
    }
    Vector2 DirectionFriend;
    void Update()
    {

        if (state == StateFriend.FRIEND_DIE) return;
        
        SetKeyAnimations();
        if (!StaticData.IsPlay) return;
        //if (hide && enemyTransform != null)
        //{
        //    float d = Vector2.Distance((Vector2)transform.position, (Vector2)enemyTransform.position);
        //    if (d > 10f)
        //    {
        //        HideAndSneek();              
        //        enemyTransform = null;
        //    }
        //    return;
        //}
        //if (dangerous &&enemyTransform!=null)
        //{
           
        //    if (time_wait+0.5f<Time.time)
        //    {
        //        HideAndSneek();
        //        dangerous = false;
        //        enemyTransform = null;
        //    }
        //}
        //if (state != StateFriend.Follow) return;

        //#region not yet detect

        //if (Target == null) return;
        

        
        //float distance = Vector2.Distance((Vector2)Target.position, (Vector2)transform.position);
        //hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)Target.position - (Vector2)transform.position, distance, 1 << 13);//a ray from the enemy to the player
        //var hitH = Physics2D.Raycast((Vector2)transform.position + Vector2.up * 1.8f, (Vector2)Target.position - (Vector2)transform.position, 2f, 1 << 13);

        //if (hit.collider != null)
        //{
        //    Vector3 moveDirection = Target.transform.position - transform.position;
        //    float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
        //    DirectionFriend = Vector2.Perpendicular(moveDirection).normalized;
        //}

        //else if (state == StateFriend.Follow)
        //{
        //    Vector3 moveDirection = Target.transform.position - transform.position;
        //    if (moveDirection != Vector3.zero)
        //    {
        //        DirectionFriend = moveDirection.normalized;

        //    }

        //}
        //if (hitH.collider != null)
        //{

        //    transform.position = Target.position - (Vector3)DirectionFriend;
        //}
        //if (!Target.gameObject.activeSelf)
        //{
        //    Target = null;
        //    state = StateFriend.FRIEND_PATROL;
        //    FriendStartGame();
        //    return;
        //}

        //if (!Target.GetComponent<BodyPart>().Free && state == StateFriend.Follow)
        //{
        //    state = StateFriend.FRIEND_PATROL;
        //    FriendStartGame();
        //    Target = null;
        //}
        //#endregion

    }

    public virtual void FixedUpdate()
    {
        if (state == StateFriend.FRIEND_DIE) return;
        SpeedReal = hide ? v_npc_decoy : v_npc_normal;
        if (!StaticData.IsPlay) return;
        if (StateFriend.FRIEND_GO_TARGET != state) return;
        animator.transform.localScale = DirectionFriend.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;

        Body.AddRelativeForce(DirectionFriend * SpeedReal);//the speed depends on the current hp
    }

    int n = 0;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (state == StateFriend.FRIEND_DIE) return;
        if (state == StateFriend.FRIEND_GO_TARGET)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                n++;
                if (n >= 2 && Target)
                {
                    transform.position = Target.position;
                    n = 0;
                }
            }
        }
        if (collision.gameObject.CompareTag("Box"))
        {
            n = 0;
            var boxScript = collision.gameObject.GetComponent<BodyPart>();
            var distance = Vector2.Distance(transform.position, boxScript.transform.position);
            if ( state == StateFriend.FRIEND_PATROL)
            {
                PlayEmotionGetItem();
                StopCoroutine("DetectGift");
                LeanTween.cancel(gameObject);
                Target = collision.transform;
                state = StateFriend.FRIEND_GO_TARGET;

            }else if (state == StateFriend.FRIEND_GO_TARGET && distance<0.5f&&Target.gameObject.activeSelf)
            {
                boxScript.Hide();
                box = true;
                typeMission = boxScript;
                if (!hide)
                {
                    spriteRendererBox.sprite = Resources.Load<Sprite>("Avatar/" + boxScript.ID.ToString());
                    Invoke("show", 0.3f);
                    //spriteRendererBox.gameObject.SetActive(true);
                }
                else
                {
                    spriteRendererBox.gameObject.SetActive(false);
                }


                state = StateFriend.FRIEND_GO_MAIN;
                PlayEmotionGetItem();
                ReturnToBeginPoint();
            }
            //else if (!boxScript.IsAlive)
            //{
            //    if (state == StateFriend.Follow)
            //    {
            //        Target = null;
            //        FriendStartGame();
            //        boxScript.Inactive();
            //    }
            //}

        }
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == StateFriend.FRIEND_DIE) return;
      
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        //if (state == StateFriend.Die) return;
        //if (hide)
        //{
        //    if (collision.gameObject.CompareTag("Enemy"))
        //    {
        //        if (box)
        //            spriteRendererBox.gameObject.SetActive(true);
        //        gameObject.tag = "Player";
        //        hide = false;
        //    }
        //}
    }
    

  
    void enum_emo(string anim_name)
    {
        if (StaticData.GM_Mode == Mode.HostleMode) return;
        int rnd = UnityEngine.Random.Range(0, 10);
        if (rnd > 3) return;
       
        emoji.SetActive(true);
        Invoke("hide_emo", 2f);
    }
    void hide_emo()
    {
        emoji.SetActive(false);
    }
    int count = 0;
    public virtual void Death()
    {
        //Debug.Log("Death:" + count);
        //if (box)
        //{
        //    spriteRendererBox.gameObject.SetActive(false);
        //    if (StaticData.GM_Mode == Mode.SurvivalMode)
        //        GameManager.Instance.RegenerateBox(transform.position, typeMission.ID);
            
        //}
        //StopCoroutine("DetectGift");
        //StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
        //Target = null;
        //state = StateFriend.FRIEND_DIE;
        //hide = false;
        //foreach (var param in animator.parameters)
        //{
        //    animator.ResetTrigger(param.name);
        //}
        //animator.SetTrigger(NPC.LoseProperties1);
        //var colliders = GetComponents<Collider2D>();
        //for (int i = 0; i < colliders.Length; i++)
        //    colliders[i].enabled = false;
        //if (StaticData.GM_Mode == Mode.SurvivalMode)
        //{
        //    GameManager.Instance.RemoveFriend(this.ID);
        //}     
        //Invoke("DestroyGameObject", 5f);
        //Invoke("Die", 3f);
    }
    public virtual void DeathGreen(Vector3 pos)
    {
        //StopCoroutine("DetectGift");
        //StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
        //Target = null;
        //hide = false;
        //state = StateFriend.FRIEND_DIE;
        //transform.position = pos;
        //animator.transform.localScale = Vector3.one * 0.7f;
        //foreach (var param in animator.parameters)
        //{
        //    animator.ResetTrigger(param.name);
        //}
        //animator.SetTrigger(NPC.LoseProperties2);
        //if (box)
        //{
        //    spriteRendererBox.gameObject.SetActive(false);
        //    if (StaticData.GM_Mode == Mode.SurvivalMode)
        //        GameManager.Instance.RegenerateBox(transform.position, 0);
          
        //}
        //var colliders = GetComponents<Collider2D>();
        //for (int i = 0; i < colliders.Length; i++)
        //    colliders[i].enabled = false;

        //if (StaticData.GM_Mode == Mode.SurvivalMode)
        //{
        //    GameManager.Instance.RemoveFriend(this.ID);
        //}
        
        
        //enum_emo(StaticParam.tired_emo);
        //Invoke("DestroyGameObject", 5f);
    }
    void Die()
    {
        die = true;
    }
    public virtual void HideAndSneek()
    {
        //hide = !hide;
        //StopCoroutine("DetectGift");
        //StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
        //if (hide)
        //{
        //    gameObject.tag = "Hide";
        //    spriteRendererBox.gameObject.SetActive(false);
         
        //}
        //else
        //{
        //    if (box)
        //        spriteRendererBox.gameObject.SetActive(true);
        //    gameObject.tag = "Player";
        //}
        //if (state == StateFriend.FRIEND_PATROL)
        //{                   
        //    FriendStartGame();
        //}
        //else if (state == StateFriend.FRIEND_GO_MAIN)
        //{          
        //    ReturnToBeginPoint();
        //}
    }
    void show()
    {
        spriteRendererBox.gameObject.SetActive(true);
    }
    public void MakeNoise(Vector2 center)
    {
        run = true;
        StartCoroutine("move_noise", center);

    }
    IEnumerator move_noise(Vector2 center)
    {
        yield return null;
        Vector2 pos1, pos2;
        pos1 = center; pos2 = center;
        Vector2 dir1 = (center - (Vector2)transform.position).normalized;
        var ray1 = Physics2D.Raycast((Vector2)transform.position, dir1, 40, 1 << 13);
        if (ray1.collider != null)
        {
            pos1 = ray1.point - dir1;
        }
        var ray2 = Physics2D.Raycast((Vector2)transform.position, -dir1, 40, 1 << 13);
        if (ray2.collider != null)
        {
            pos2 = ray2.point + dir1;
        }
        var arr = new Vector2[] { pos1, pos2, pos1, pos2, pos1, pos2 };
        Vector2 currentPos = transform.position;
        for (int i = 0; i < arr.Length; i++)
        {
            float distance = Vector2.Distance(currentPos, arr[i]);
            var dir = arr[i] - currentPos;
            animator.transform.localScale = dir.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
            var rnd = UnityEngine.Random.Range(1, 10);           
            LeanTween.move(gameObject, arr[i], distance / v_npc_normal);
            yield return new WaitForSeconds(distance / v_npc_normal);
            if(rnd<=3)
            AudioManager.instance.Play(string.Format("voice{0}", rnd));
            PlayRndBeginGame();
            currentPos = arr[i];
        }
        run = false;
    }
    public void Return()
    {
        Debug.Log("Return");
    }
    void DestroyGameObject()
    {
        gameObject.SetActive(false);
    }
    public void TurnOnWin()
    {
        //animator.SetTrigger(NPC.WinProperties);
        //spriteRendererBox.gameObject.SetActive(false);
        //StopCoroutine("DetectGift");
        //StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
    }
    public void TurnOnLose()
    {
        //animator.SetTrigger(NPC.FailTrigger);
        //spriteRendererBox.gameObject.SetActive(false);
        //StopCoroutine("DetectGift");
        //StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
    }
    void PatrolGift()
    {

    }
    void OnPatrolGift()
    {
        state= StateFriend.FRIEND_PATROL;
        //yield return null;
        //Vector2 currentPos = transform.position;
        //while (state == StateFriend.FRIEND_PATROL)
        //{
        //    for (int i = 0; i < listPathDetect.Count; i++)
        //    {
        //        float distance = Vector2.Distance(currentPos, listPathDetect[i]);
        //        var dir = listPathDetect[i] - currentPos;
        //        animator.transform.localScale = dir.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;

        //        LeanTween.move(gameObject, listPathDetect[i], distance / SpeedReal);
        //        yield return new WaitForSeconds(distance / SpeedReal);
        //        if (i >= listPathDetect.Count) yield break;
        //        currentPos = listPathDetect[i];
        //    }
        //    SetupDetectPath();
        //    currentPos = transform.position;
        //    yield return new WaitForSeconds(0.1f);
        //}

    }
    void GoPathReturn()
    {

    }
    void OnGoPathReturn()
    {
        state= StateFriend.FRIEND_GO_MAIN;
        //yield return null;
        //Vector2 currentPos = transform.position;
        //for (int i = 0; i < listPathReturn.Count; i++)
        //{
        //    float distance = Vector2.Distance(currentPos, listPathReturn[i]);
        //    var dir = listPathReturn[i] - currentPos;
        //    animator.transform.localScale = dir.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
        //    LeanTween.move(gameObject, listPathReturn[i], distance / SpeedReal);
        //    yield return new WaitForSeconds(distance / SpeedReal);
        //    if (i >= listPathReturn.Count) yield break;
        //    currentPos = listPathReturn[i];
        //}
        //PlayWinEmotion();
        //spriteRendererBox.gameObject.SetActive(false);
        //box = false;

        //FriendStartGame();
        //if (StaticData.GM_Mode == Mode.SurvivalMode)
        //{
        //    GameManager.Instance.AddMission(typeMission, this.ID);
        //}else if (StaticData.GM_Mode == Mode.HostleMode)
        //{
        //    HostileManager.Instance.AddMission(typeMission,this.winPos,sideA);
        //}

    }
    void OnConveyBoxToSlot()
    {

    }
    public void DropBoxToSlot(Slot s)
    {
        
    }
    public void RecoverFriend()
    {
        //foreach (var param in animator.parameters)
        //{
        //    animator.ResetTrigger(param.name);
        //}
        //animator.SetTrigger(NPC.RevivePropertis);

        //if (box)
        //{
        //    hide = false;
        //    state = StateFriend.FRIEND_GO_MAIN;
        //    spriteRendererBox.gameObject.SetActive(true);
        //    ReturnToBeginPoint();
        //}
        //else
        //{
        //    spriteRendererBox.gameObject.SetActive(false);
        //    state = StateFriend.FRIEND_PATROL;
        //    FriendStartGame();
        //}
        //Target = null;
    }
}

public enum StateFriend
{
    FRIEND_INIT = 0,
    FRIEND_PATROL = 1,
    FRIEND_GO_TARGET = 2,
    FRIEND_GO_MAIN = 3,
    FRIEND_DIE = 4,
    FRIEND_CHASED=5,
    FRIEND_BEATEN=6,
    FRIEND_SORTING_FOOD=7
}

