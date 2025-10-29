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
    protected StateFriend state = StateFriend.FRIEND_INIT;//If the friend has ceased to see the gift, he will follow on PlayerLastPos
    
    // Public property để cho phép access state từ các class khác (như BossBase)
    public StateFriend State
    {
        get => state;
        set => state = value;
    }
    
    // Property để khóa vị trí NPC (sử dụng khi BossBase tấn công)
    private bool _isPositionLocked = false;
    public bool IsPositionLocked
    {
        get => _isPositionLocked;
        set => _isPositionLocked = value;
    }
    
    public bool isDecoy = false;
    public float v_npc_normal = 1f;
    public float v_npc_decoy= 3f;
    public float SpeedReal;
    public float Range = 5;
    public BodyPart bodyPart;
    public NPCObj friendObj;
    [SerializeField] protected GameObject frameBox;
    [SerializeField] protected Transform decoyTransform;
    protected SkeletonMecanim skeletonMecanim;
    protected Animator decoyAnimator;
    protected string box_name;
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
    private int currentWaypointIndexDetect = 0;
    private int currentWaypointIndexReturn = 0;
    private List<Vector2> listPathChase;
    private int currentWaypointIndexChase = 0;
    private StateFriend previousState;
    protected const string RunProperties = "run_normal";
    protected const string WinTrigger = "WinTrigger";
    protected const string WinParams = "WinParams";
    protected const string LoseTrigger = "LoseTrigger";
    protected const string LoseTrigger2 = "LoseTrigger2";
    protected const string FailTrigger = "FailTrigger";
    protected const string DieTrigger = "DieTrigger";
    protected const string HitTrigger = "HitTrigger";
    protected const string ReviveTrigger = "ReviveTrigger";
    protected const string ScareProperties = "Scare";
    protected const string RunBoosterProperties = "run_booster";
    protected const string ReturnProperties="box";
    protected const bool booster = false;
    public bool run = false;
    protected bool box = false;
    protected bool hide = false;
    protected bool run0 = false;
    protected bool boosterRun = false;
    protected bool die = false;
    protected int walk = -1;
    protected bool dangerous = false;
    protected float time_wait = 0f;
    private Dictionary<string, bool> DecoyDictionary = new Dictionary<string, bool>
    {
        {"carton",false },{"drum_box",false},{"shove",false},{"snow",false},{ "tree",false},{"tnt_box",false},{"gift_box",false},{"toe",false }
    };
    protected int boxIndex = 0;
    protected void Start()
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
        frameBox = this.animator.transform.GetChild(0).gameObject;
        frameBox.gameObject.SetActive(false);
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
        animator.SetTrigger(FailTrigger);
    }
    public void PlayRndBeginGame()
    {
        string[] arr = new string[] { StaticParam.poke_emo, StaticParam.emo2, StaticParam.tamgiac_emo_begin, StaticParam.tamgiac_emo_loop,StaticParam.smile_emo };
        int rnd = UnityEngine.Random.Range(0, 5);
        enum_emo(arr[rnd]);
    }
    public void PlayEmotionGetItem()
    {
        string[] arr = new string[] { StaticParam.here_emo,StaticParam.smile_emo,StaticParam.poke_emo};
        int rnd = UnityEngine.Random.Range(0, 3);
        enum_emo(arr[rnd]);
    }
    public void PlayEmotionRun()
    {
        string[] arr = new string[] { StaticParam.run_emo, StaticParam.hurry_up, StaticParam.poke_emo, StaticParam.smile_emo };
        int rnd = UnityEngine.Random.Range(0, 4);
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
                decoyAnimator = ContentAssistant.Instance.GetItem<Animator>(key,decoyTransform.position);
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
        currentWaypointIndexDetect = 0;
        currentWaypointIndexReturn = 0;
        // StopCoroutine("DetectGift");
        // StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
        Target = null;
    }
    
    private void ResetWaypointIndices()
    {
        currentWaypointIndexDetect = 0;
        currentWaypointIndexReturn = 0;
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
        animator.SetBool(ReturnProperties, box);      

        animator.SetBool(ScareProperties,run&&dangerous&&!box);
    }
    public virtual void Deal(int damage)
    {
        HP -= damage;
        animator.SetTrigger(HitTrigger);        
    }

    protected void ReturnToBeginPoint()
    {
        if (state == StateFriend.FRIEND_DIE) return;
        if (box)
        {
            if(!hide)
                frameBox.gameObject.SetActive(true);
            else
                frameBox.gameObject.SetActive(false);
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
    public virtual void Update()
    {

        if (state == StateFriend.FRIEND_DIE) return;
        
        // Nếu vị trí bị khóa (đang bị tấn công), bỏ qua tất cả lệnh di chuyển
        if (IsPositionLocked) 
        {
            SetKeyAnimations();
            return;
        }
        
        else if(state==StateFriend.FRIEND_GO_TARGET)
        {
           
        }
        else if (state == StateFriend.FRIEND_PATROL)
        {
            if (run)
                PatrolGift();
        }
        else if (state == StateFriend.FRIEND_GO_MAIN)
        {
            if (run)
                GoPathReturn();
        }
        else if (state == StateFriend.FRIEND_CHASED)
        {
            ChasePath();
        }
        
        SetKeyAnimations();
        if (!StaticData.IsPlay) return;
      

    }

    public virtual void FixedUpdate()
    {
        if (state == StateFriend.FRIEND_DIE) return;
        SpeedReal =  isDecoy? v_npc_decoy : v_npc_normal;
        
    }

  
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == StateFriend.FRIEND_DIE) return;
      
    }
  
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == StateFriend.FRIEND_DIE) return;
        if (collision.collider.CompareTag("Wall"))
        {
            // Xử lý phản xạ khi va chạm với tường
            HandleWallBounce(collision);
        }
    }
     private void HandleWallBounce(Collision2D collision)
    {
        // Lấy vận tốc hiện tại
        Vector2 currentVelocity = Body.linearVelocity;
        
        // Lấy hướng pháp tuyến (normal) của bề mặt va chạm
        Vector2 normal = collision.relativeVelocity.normalized;
        if (collision.contactCount > 0)
        {
            normal = -collision.GetContact(0).normal;
        }
        
        // Tính toán hướng phản xạ: reflection = velocity - 2 * (velocity · normal) * normal
        Vector2 reflection = currentVelocity - 2f * Vector2.Dot(currentVelocity, normal) * normal;
        
        // Áp dụng vận tốc phản xạ với một hệ số độ mạnh (bounce factor)
        float bounceFactor = 0.7f; // Điều chỉnh giá trị này để thay đổi độ mạnh của phản xạ
        Body.linearVelocity = reflection * bounceFactor;
        
        // Dịch chuyển gameobject ra ngoài tường một chút để tránh va chạm liên tục
        transform.position -= (Vector3)normal ;
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
         state = StateFriend.FRIEND_DIE;
        // StopCoroutine("DetectGift");
        // StopCoroutine("DetectPathReturn");
        // LeanTween.cancel(gameObject);
        
        // Cancel box pickup if in progress

        if (box && bodyPart != null)
        {
            BodyPart bp = bodyPart as BodyPart;
            if (bp != null)
            {
                bp.ReActive();
            }
        }

        
        animator.SetTrigger(DieTrigger);
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
    }
 
    void Die()
    {
        die = true;
    }
    public virtual void HideAndSneek(bool isTransparent)
    {
         hide = !isTransparent;
        // StopCoroutine("DetectGift");
        // StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);
        
        // Cancel box pickup if in progress

        if (hide)
        {
            gameObject.tag = "Hide";
            frameBox.gameObject.SetActive(false);
        }
        else
        {
            if (box)
                frameBox.gameObject.SetActive(true);
            gameObject.tag = "Player";
        }
    }
    void show()
    {
        frameBox.gameObject.SetActive(true);
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
        state = StateFriend.FRIEND_END;
        int rnd = UnityEngine.Random.Range(1, 5);
        animator.SetTrigger(NPC.WinTrigger);
        animator.SetInteger(NPC.WinParams, rnd);
        PlayWinEmotion();
        frameBox.gameObject.SetActive(false);
        decoyAnimator.gameObject.SetActive(false);
    }
    public void TurnOnLose()
    {
        state = StateFriend.FRIEND_END;
        int rnd = UnityEngine.Random.Range(0, 2);
        string loseTrigger = rnd == 0 ? LoseTrigger : LoseTrigger2;
        animator.SetTrigger(loseTrigger);
        PlayLoseEmotion();
        frameBox.gameObject.SetActive(false);
        decoyAnimator.gameObject.SetActive(false);
        
    }
    void PatrolGift()
    {
        if (listPathDetect == null || listPathDetect.Count == 0) return;
        
        Vector2 targetWaypoint = listPathDetect[currentWaypointIndexDetect];
        Vector2 currentPosition = (Vector2)transform.position;
        float distance = Vector2.Distance(currentPosition, targetWaypoint);
        
        // Nếu chưa đến waypoint, di chuyển tới
        if (distance > 0.2f)
        {
            Vector2 direction = (targetWaypoint - currentPosition).normalized;
            DirectionFriend = direction;
            animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
            
            // Di chuyển gameobject đến vị trí waypoint
            transform.position = Vector2.MoveTowards(currentPosition, targetWaypoint, SpeedReal * Time.deltaTime);
        }
        else
        {
            // Đã đến waypoint, chuyển sang waypoint tiếp theo
            currentWaypointIndexDetect++;
            if (currentWaypointIndexDetect >= listPathDetect.Count)
            {
                // Hoàn thành vòng tuần tra, reset chỉ số
                currentWaypointIndexDetect = 0;
                if (isDecoy)
                {
                   decoyAnimator.gameObject.SetActive(false);
                   isDecoy = false;
                   HideAndSneek(true);
                }
                FriendStartGame(); // Thiết lập lại đường đi
            }
        }
    }
    void OnPatrolGift()
    {
        state= StateFriend.FRIEND_PATROL;     
        dangerous=false;
    }
    void GoPathReturn()
    {
        if (listPathReturn == null || listPathReturn.Count == 0) return;
        
        Vector2 targetWaypoint = listPathReturn[currentWaypointIndexReturn];
        Vector2 currentPosition = (Vector2)transform.position;
        float distance = Vector2.Distance(currentPosition, targetWaypoint);
        
        // Nếu chưa đến waypoint, di chuyển tới
        if (distance > 0.2f)
        {
            Vector2 direction = (targetWaypoint - currentPosition).normalized;
            DirectionFriend = direction;
            animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
            
            // Di chuyển gameobject đến vị trí waypoint
            transform.position = Vector2.MoveTowards(currentPosition, targetWaypoint, SpeedReal * Time.deltaTime);
        }
        else
        {
            // Đã đến waypoint, chuyển sang waypoint tiếp theo
            currentWaypointIndexReturn++;
            if (currentWaypointIndexReturn >= listPathReturn.Count)
            {

                // Hoàn thành việc quay về
                if (isDecoy)
                {
                    decoyAnimator.gameObject.SetActive(false);
                    isDecoy = false;
                    HideAndSneek(true);
                }
                currentWaypointIndexReturn = 0;
                state = StateFriend.FRIEND_SORTING_FOOD;
                run = false;
                dangerous=false;
                DirectionFriend = Vector2.zero;
                
                // Lấy Slot tương ứng với ID của bodyPart
                Slot targetSlot = FindSlotForBodyPart(bodyPart);
                if (targetSlot != null)
                {
                    // Di chuyển NPC đến vị trí slot bằng LeanTween
                    Vector3 targetPosition = targetSlot.transform.position;
                    distance = Vector3.Distance(transform.position, targetPosition);
                    float duration = distance / SpeedReal;
                    
                    LeanTween.move(gameObject, targetPosition, duration)
                        .setOnComplete(() =>
                        {
                            // Tạo Chip tại slot
                            Chip chip = ContentAssistant.Instance.GetItem<Chip>("Chip");
                            Sprite avatarSprite = Resources.Load<Sprite>($"Avatar/Avatar_{bodyPart.ID}");
                            chip.Initialize(targetSlot, avatarSprite);
                            
                            // Ẩn box
                            frameBox.gameObject.SetActive(false);
                            box = false;
                            GameManager.Instance.OnSortComplete();
                            // Kiểm tra xem tất cả slot đã được điền
                            if (FieldAssistant.main.AreAllSlotsOccupied())
                            {
                                GameManager.Instance.OnVictory();
                            }
                            else
                            {
                                // Còn slot trống → setup đường đi tuần mới
                                bodyPart.DestroyNow();
                                FriendStartGame();
                            }
                        });
                }
            }
        }
    }
    
    private Slot FindSlotForBodyPart(BodyPart part)
    {
        if (part == null || FieldAssistant.main == null || FieldAssistant.main.field == null) 
            return null;
        
        // Công thức ID = x * width + y, trong đó x là hàng, y là cột
        // Nhưng trong code: slot.x = col (cột), slot.y = row (hàng)
        // Nên: ID = slot.y * field.width + slot.x
        int width = FieldAssistant.main.field.width;
        Dictionary<string, Slot> allSlots = FieldAssistant.main.GetAllSlots();
        
        foreach (var slot in allSlots.Values)
        {
            int slotID = slot.y * width + slot.x;
            if (slotID == part.ID)
            {
                return slot;
            }
        }
        
        return null;
    }
    void OnGoPathReturn()
    {
        state= StateFriend.FRIEND_GO_MAIN;
        dangerous=false;
    }
   
    public void DropBoxToSlot(Slot s)
    {
        
    }

    /// <summary>
    /// Hàm public để gọi di chuyển NPC đến vị trí targetPos một cách an toàn
    /// Sẽ tìm waypoint gần nhất và setup đường đi qua DIJKSTRA
    /// </summary>
    public void MoveTo(Vector2 targetPos)
    {
        // Tìm waypoint gần nhất với targetPos
        int targetWaypointIndex = GetIndex(targetPos);
        if (targetWaypointIndex == -1)
        {
            return;
        }
        
        // Setup đường đi an toàn thông qua DIJKSTRA
        SetupChasePath(targetWaypointIndex);
    }

    /// <summary>
    /// Setup đường đi an toàn từ vị trí hiện tại đến waypoint target
    /// </summary>
    private void SetupChasePath(int targetWaypointIndex)
    {
        // Lưu state trước khi vào FRIEND_CHASED để quay lại sau
        previousState = state;
        
        // Lấy danh sách waypoints (phải là danh sách mà chúng ta đang tuần tra/quay về)
        if (Waypoints == null || Waypoints.Count == 0)
        {
            return;
        }
        
        // Thiết lập graph waypoints với collision detection
        field = new FieldMini();
        field.WIDTH = Waypoints.Count;
        field.joints = new float[field.WIDTH, field.WIDTH];
        
        for (int i = 0; i < Waypoints.Count; i++)
        {
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
        }
        
        // Nếu đã ở vị trí target, không cần di chuyển, quay lại state cũ ngay
        if (currentIndex == targetWaypointIndex)
        {
            return;
        }
        
        // Dùng DIJKSTRA để tìm đường đi từ waypoint hiện tại đến target waypoint
        DIJKTRA.Init(field);
        DIJKTRA.InputSecond(currentIndex, targetWaypointIndex);
        var resultPath = DIJKTRA.Output();
        
        if (resultPath != null)
        {
            listPathChase = new List<Vector2>();
            for (int i = 0; i < resultPath.Length; i++)
            {
                listPathChase.Add(Waypoints[resultPath[i]]);
            }
            
            // Chuyển sang state FRIEND_CHASED
            currentWaypointIndexChase = 0;
            state = StateFriend.FRIEND_CHASED;
            dangerous=true;
        }
    }

    /// <summary>
    /// Xử lý di chuyển trong state FRIEND_CHASED
    /// </summary>
    private void ChasePath()
    {
        if (listPathChase == null || listPathChase.Count == 0)
        {
            state = previousState;
            return;
        }

        Vector2 targetWaypoint = listPathChase[currentWaypointIndexChase];
        Vector2 currentPosition = (Vector2)transform.position;
        float distance = Vector2.Distance(currentPosition, targetWaypoint);

        // Nếu chưa đến waypoint, di chuyển tới
        if (distance > 0.2f)
        {
            Vector2 direction = (targetWaypoint - currentPosition).normalized;
            DirectionFriend = direction;
            animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;

            // Di chuyển gameobject đến vị trí waypoint
            transform.position = Vector2.MoveTowards(currentPosition, targetWaypoint, SpeedReal * Time.deltaTime);
        }
        else
        {
            // Đã đến waypoint, chuyển sang waypoint tiếp theo
            currentWaypointIndexChase++;
            if (currentWaypointIndexChase >= listPathChase.Count)
            {
                // Hoàn thành FRIEND_CHASED, quay lại state trước đó
                currentWaypointIndexChase = 0;
                state = previousState;

                // Cập nhật waypoint index hiện tại cho state tiếp theo
                SetCurrentIndex((Vector2)transform.position);

                // Nếu quay lại FRIEND_PATROL, setup lại detect path
                if (previousState == StateFriend.FRIEND_PATROL)
                {
                    SetupDetectPath();
                }
                // Nếu quay lại FRIEND_GO_MAIN, setup lại return path
                else if (previousState == StateFriend.FRIEND_GO_MAIN)
                {
                    ReturnToBeginPoint();
                }
            }
        }
    }
     /// <summary>
    /// Gọi hàm di chuyển an toàn từ NPC base class
    /// Sẽ sử dụng DIJKSTRA pathfinding để tránh va chạm
    /// </summary>
    

    public virtual void ThrowFood(Vector3 npcPos, Vector3 enemyPos)
    {
        // Tính toán vị trí đối xứng của npcPos qua điểm enemyPos
        // Công thức: Symmetric_Point = 2 * enemyPos - npcPos
        Vector3 foodSpawnPos = 2 * enemyPos - npcPos;

        Vector3 throwDirection = (Vector3)UnityEngine.Random.insideUnitCircle.normalized;
        Food foodObj = BaitManager.Instance.SpawnFood(foodSpawnPos, throwDirection);
        //Rigidbody2D rb = foodObj.GetComponent<Rigidbody2D>();
        //if (rb != null)
        //{
        //    rb.linearVelocity = throwDirection * 5f;
        //}
    }

    public virtual void RecoverFriend()
    {
        
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
    FRIEND_SORTING_FOOD=7,
    FRIEND_END=8
}

