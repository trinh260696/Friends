using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Unity.VisualScripting;

/// <summary>
/// BossBase - Lớp quản lý hành vi của Boss
/// 
/// HỆ THỐNG TẤN CÔNG THEO THỜI GIAN:
/// - durationAttack: Thời gian (giây) giữa các lần tấn công
/// - attackTimer: Biến đếm thời gian tấn công hiện tại
/// - AttackTrigger: Trigger animation tấn công
/// - HandleTimedAttack(): Quản lý logic tấn công có kiểm soát thời gian
/// 
/// WORKFLOW:
/// 1. Khi mục tiêu trong RANGE_NEAR, gọi HandleTimedAttack() thay vì AttackTarget()
/// 2. HandleTimedAttack() đếm thời gian từng frame
/// 3. Khi attackTimer >= durationAttack:
///    - Gọi AttackTarget() (chuẩn bị trạng thái, trigger animation)
///    - Gọi DealDamage() (gọi npc.Deal(damage))
///    - Reset timer
/// 4. Khi mục tiêu ra ngoài tầm hoặc Boss chuyển trạng thái, timer được reset
/// </summary>
public class BossBase : MonoBehaviour
{
    public string nameBoss = "Boss";
    public int IQ = 1;
    public float attackSpeed = 1f;
    public int damage = 10;
    
    private EnemyState _state;
    public EnemyState State { get => _state; set => _state = value; }
    
    public Animator animator;
    public Collider2D DetectionCollider;//Enemy detection collider
    public Rigidbody2D Body;//Enemy body      
    public Transform pivotTransform;
    public Transform Target;//Player transform
    public GameObject emoAnimation;
    private bool _follow;
    public bool Follow
    {
        get => _follow; set
        {
            _follow = value;
            deltaSpeed = 0;
        }
    }

   
    //If the enemy has ceased to see the player, he will follow on PlayerLastPos
    public float Range;
    public float Speed=4.0f;     
    //blood sprites (GameObjects)
   
   
   
    public bool isMoveWayPoint = false;
    

    private RaycastHit2D hit;
    public bool beating = false;
    private List<Vector2> Waypoints;
    private List<Vector2> DetectPoints;
    private int wallCollisionCount = 0; // Đếm số lần va chạm với tường để phát hiện tắc
    private const string BossRunProperties = "bossrun";
    private const string BossChaseProperties = "bosschase";
    public const string BossBeatProperties = "bosskill";
    private const string BossDamageProperties = "bossattacked";
    private const string TriggerDie = "dietrigger";
    private const string BossEatProperties = "bosseat";
    private const string ReviveTrigger = "revivetrigger";
    private const string AttackTrigger = "attacktrigger";
    private bool run = false;
    protected bool eat = false;
    protected bool stun = false;
    public float RANGE_FAR = 10f;
    public float RANGE_NEAR = 1f;
    public float v_slow = 2f;
    public float v_fast = 6f;
    public float durationAttack = 1f; // Thời gian giữa các lần tấn công
    private float stunDuration = 0f;
    private float stunTimer = 0f;
    private float attackTimer = 0f; // Timer để đếm thời gian tấn công
    private Vector2 lastTargetPos = Vector2.zero;
    private float velocityMultiplier = 1f;
    
    // Biến để quản lý khóa vị trí Target trong quá trình tấn công
    private NPC targetNPC = null; // Reference đến NPC component của Target
       
    public void SetKeyAnimation()
    {
        // Cập nhật các animation parameter dựa trên state hiện tại
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        // Reset tất cả các boolean trước tiên để tránh xung đột
        animator.SetBool(BossRunProperties, false);
        animator.SetBool(BossChaseProperties, false);
        animator.SetBool(BossEatProperties, false);
        animator.SetBool(BossBeatProperties, false);
        animator.SetBool(BossDamageProperties, false);

        // Set parameter dựa trên state hiện tại
        switch (State)
        {
            case EnemyState.PATROL_STATE:
                if (run)
                    animator.SetBool(BossRunProperties, true);
                break;

            case EnemyState.CHASE_STATE:
                animator.SetBool(BossChaseProperties, true);
                break;

            case EnemyState.FIGHT_STATE:
                animator.SetBool(BossBeatProperties, beating);
                break;

            case EnemyState.EAT_STATE:
                animator.SetBool(BossEatProperties, eat);
                break;

            case EnemyState.STUN_STATE:
                animator.SetBool(BossDamageProperties, stun);
                break;

            case EnemyState.IDLE_STATE:
                // IDLE state - không set parameter nào
                break;
        }
    }

    public void PlayRunAnimation()
    {
        run = true;
        animator.SetBool(BossRunProperties, true);
    }

    public void PlayChaseAnimation()
    {
        animator.SetBool(BossChaseProperties, true);
    }

    public void PlayBeatAnimation()
    {
        beating = true;
        animator.SetBool(BossBeatProperties, true);
    }

    public void PlayAttackTrigger()
    {
        animator.SetTrigger(AttackTrigger);
    }

    public void PlayEatAnimation()
    {
        eat = true;
        animator.SetBool(BossEatProperties, true);
    }

    public void PlayStunAnimation()
    {
        stun = true;
        animator.SetBool(BossDamageProperties, true);
    }

    public void PlayReviveAnimation()
    {
        animator.SetTrigger(ReviveTrigger);
    }

    public void StopAllAnimations()
    {
        run = false;
        beating = false;
        eat = false;
        stun = false;
        animator.SetBool(BossRunProperties, false);
        animator.SetBool(BossChaseProperties, false);
        animator.SetBool(BossBeatProperties, false);
        animator.SetBool(BossEatProperties, false);
        animator.SetBool(BossDamageProperties, false);
    }
    int currentIndex;
    private int _travelPointIndex = 0;
    private float _waypointReachThreshold = 0.5f;
    
    private void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "BossStartGame");
        NotificationCenter.DefaultCenter().AddObserver(this, "OnFinish");
        DetectPoints = new List<Vector2>();
        OnFinishTravel();
        if (emoAnimation != null)
        {
            emoAnimation.SetActive(false);
        }    
        DetectionCollider=GetComponent<Collider2D>();
    }
    
    public virtual void OnInit()
    {
        if (isMoveWayPoint)
        {
            StartDetect();
        }
    }
    public void BossStartGame()
    {
        OnInit();
    }
    public void PopulatePos(Notification receivedData)
    {
        var pos = GameManager.Instance.GetBossPos();
        transform.position = pos;
    }
    public void StartDetect()
    {
        if (isMoveWayPoint)
        {
            SetupPointDetect();
            if (DetectPoints.Count > 0)
            {
                PlayRunAnimation();
                OnTravelPoint();
                //StartCoroutine("TravelPoint");
            }             
            else
            {
                run = false;
            }
        }
        else
        {
            run = false;
            State = EnemyState.IDLE_STATE;
        }
    }
    void SetupPointDetect()
    {
        int S, F;
        Waypoints = GameManager.Instance.GetBossPath(transform, out S, out F);
        FieldMini field = new FieldMini();
        field.WIDTH = Waypoints.Count;
        field.joints = new float[field.WIDTH, field.WIDTH];
        for (int i = 0; i < Waypoints.Count; i++)
            for (int j = i + 1; j < Waypoints.Count; j++)
            {
                float distance = Vector2.Distance((Vector2)Waypoints[i], Waypoints[j]);
                var raycastHit = Physics2D.Raycast(Waypoints[i], Waypoints[j] - Waypoints[i], distance, 1<<13|1<<1);
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
        DIJKTRA.InputSecond(S, F);
        var resultSecond = DIJKTRA.Output();
        if(resultSecond!=null)
        {
            run=true;
            DetectPoints.Clear();
            for (int i = 0; i < resultSecond.Length; i++)
            {
                DetectPoints.Add(Waypoints[resultSecond[i]]);
            }
        }
        else
        {
            DetectPoints.Clear();
            run=false;
        }
           
    }
    public void BossReturn()
    {
        attackTimer=0;
        beating = false;
        Follow = false;
        _travelPointIndex = 0; // Reset về điểm đầu tiên
        // Reset attack timer
        PlayRunAnimation();
        BossStartGame();
    }
    public void BossEnd()
    {

    }
    public void StopDetect()
    {
        run = false;
        //Stop Travel Point
        
    }
    Vector3 Direction;
    public virtual void Update()
    {
        SetKeyAnimation();
        if (!isMoveWayPoint) return;
       

        if (!StaticData.IsPlay) return;
       
        if (State == EnemyState.PATROL_STATE && run)
        {
            TravelPoint();
        }else if (State == EnemyState.CHASE_STATE)
        {
            DetectTargets();
        }
        //check of reaching the last pos 

    }
    float deltaSpeed = 0f;

  

    public virtual void FixedUpdate()
    {
        
    }
        
    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Target = col.transform;
            targetNPC = Target.GetComponent<NPC>();
            State = EnemyState.CHASE_STATE;
        }
    }
    public virtual void OnTriggerStay2D(Collider2D col)
    {

    }
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.collider.CompareTag("Finish"))
        {
            BossReturn();
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            if(State!=EnemyState.PATROL_STATE)
                return;
            DetectionCollider.isTrigger=true;     
            // Xử lý phản xạ khi va chạm với tường
            Invoke(nameof(HandleWallBounce), 0.3f);
        }

    }
    
   
    
    private void HandleWallBounce()
    {
        DetectionCollider.isTrigger=false;
    }
    public virtual void Beat(Transform Destination)
    {
        _follow = false;
        // Stop TravelPoint

        State = EnemyState.FIGHT_STATE;
        PlayBeatAnimation();
        Vector3 dir = Destination.position - transform.position;

        ContentAssistant.Instance.PlayEffectDeath(Destination, Destination.position+Vector3.right*Mathf.Sign(dir.x)+Vector3.up);
    }
    void OnTravelPoint()
    {
        State = EnemyState.PATROL_STATE;
    }
    void TravelPoint()
    {
        Debug.Log("TravelPoint");
        // Nếu không có điểm để di chuyển tới
        if (DetectPoints.Count == 0)
        {
            run = false;
            State = EnemyState.IDLE_STATE;
            return;
        }

        // Nếu _follow = true (đang theo player), dừng di chuyển
        if (_follow)
        {
            return;
        }
        if(_travelPointIndex>=DetectPoints.Count)
        {
            _travelPointIndex = 0;
            SetupPointDetect(); // Tính toán lại đường đi
            return;
        }
        // Lấy điểm waypoint hiện tại
        Vector2 currentPos = transform.position;
        Vector2 targetPoint = DetectPoints[_travelPointIndex];

        // Tính khoảng cách đến waypoint
        float distance = Vector2.Distance(currentPos, targetPoint);

        // Nếu đến gần đủ waypoint, chuyển sang waypoint tiếp theo
        if (distance < _waypointReachThreshold)
        {
            _travelPointIndex++;
            
            // Nếu đã xong tất cả waypoint, setup lại và bắt đầu từ đầu
            if (_travelPointIndex >= DetectPoints.Count)
            {
                _travelPointIndex = 0;
                SetupPointDetect(); // Tính toán lại đường đi
                return;
            }

            targetPoint = DetectPoints[_travelPointIndex];
        }

        // Di chuyển về phía waypoint
        Vector2 direction = (targetPoint - currentPos).normalized;
        Body.linearVelocity = direction * Speed;
        Debug.Log("move to point: " + targetPoint);
        // Cập nhật hướng nhìn của boss
        animator.transform.localScale = direction.x < 0 ? Vector3.one  : StaticData.ScaleInverse;
    }
    
    public void OnFinishTravel()
    {
        //run = false;
        //StopCoroutine("TravelPoint");
        //Target = null;
        //_follow = false;
    }
    public void Stun(float duration)
    {
        State = EnemyState.STUN_STATE;
        Follow = false;
        stunDuration = duration;
        stunTimer = 0f;
        attackTimer = 0f; // Reset attack timer khi bị choáng
        PlayStunAnimation();
    }

    public void DetectTargets()
    {
        if (State == EnemyState.STUN_STATE)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunDuration)
            {
                State = EnemyState.PATROL_STATE;
                StartDetect();
            }
            return;
        }

        Food nearestFood = BaitManager.Instance.FindNearestFoodForEnemy(this);
        if (nearestFood != null && IQ <= 3 && State!=EnemyState.EAT_STATE)
        {
            BaitManager.Instance.MoveEnemyTowardFood(this, nearestFood);
            //State = EnemyState.EAT_STATE;
            return;
        }
        //  if (Target == null&&attackTimer>5f)
        // {         
        //     BossReturn();
        //     return;
        // }
        if (Target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, Target.position);

            if (distanceToTarget < RANGE_NEAR)
            {
                // Sử dụng HandleTimedAttack() để quản lý tấn công theo thời gian
                HandleTimedAttack();
            }
            else if (distanceToTarget < RANGE_FAR)
            {
                ChaseTarget();
                // Reset attack timer khi không còn ở trong RANGE_NEAR
                
            }
            else
            {
                PatrolState();
               // Reset attack timer khi mục tiêu ra ngoài tầm
               
            }
        }
        else
        {
            PatrolState();
            // Reset attack timer khi không có mục tiêu
           
        }
    }

    private void ChaseTarget()
    {
        State = EnemyState.CHASE_STATE;
        Follow = true;
        run = true;
        float currentSpeed = v_fast;
      //  attackTimer=0;
        PlayChaseAnimation();
        Vector2 direction = (Target.position - transform.position).normalized;
        Body.linearVelocity = direction * currentSpeed;
        animator.transform.localScale = direction.x < 0 ? Vector3.one : StaticData.ScaleInverse;
        
    }

    private void AttackTarget()
    {
        
        targetNPC = Target.GetComponent<NPC>();
       // State = EnemyState.FIGHT_STATE;
        Follow = true;
        beating = true;
        Vector2 direction = (Target.position - transform.position).normalized;
       
        
       // Body.linearVelocity = Vector2.zero;
        if(targetNPC==null)
        {
            targetNPC = Target.GetComponent<NPC>();
        }
        if (direction.x < 0)
        {
            animator.transform.localScale = Vector3.one ;
            //targetNPC.animator.transform.localScale=0.7f*Vector3.one ;
            //Target.transform.position=pivotTransform.position;
        }else
        {
           animator.transform.localScale=StaticData.ScaleInverse;
          // targetNPC.animator.transform.localScale=0.7f*StaticData.ScaleInverse;
           //Target.transform.position=2*pivotTransform.position-animator.transform.position;
        }
            // Trigger animation attack
        PlayAttackTrigger();
            // Reset attack timer để chuẩn bị cho lần tấn công tiếp theo
    }

    private void HandleTimedAttack()
    {
       

        // Kiểm tra khoảng cách từ Boss đến Target
        //float distanceToTarget = Vector2.Distance(transform.position, Target.position);
        Vector3 direction = (Target.position - transform.position).normalized;
        transform.position = Target.position - direction*2f; 
       
        
      
        // Nếu đạt tới gioi han thời gian tấn công
        if (attackTimer >= durationAttack && targetNPC.IsPositionLocked)
        {
           
            // === KHÓA VỊ TRÍ CỦA TARGET KHI BẮTĐẦU TẤN CÔNG ===
            if (targetNPC == null)
            {
                // Lần đầu bắt đầu tấn công
                targetNPC = Target.GetComponent<NPC>();
                // if (targetNPC != null)
                // {
                //     beating=false;
                //     targetNPC.IsPositionLocked = false;
                // }
            }
            
          
            // Gọi hàm Deal damage sau khi trigger animation
            DealDamage();
            
           
           
         
        }else if (!targetNPC.IsPositionLocked&& attackTimer ==0)
        {
              // Gọi hàm tấn công mục tiêu (trigger animation)

            AttackTarget();
            targetNPC = Target.GetComponent<NPC>();
            if (targetNPC != null)
            {
                targetNPC.IsPositionLocked=true;
            }          
        }else if (attackTimer>5f)
        {
            BossReturn();
            return;
        }
        else
        {
             
            // Tăng attack timer
             attackTimer += Time.deltaTime;
        }
    }

    private void PatrolState()
    {
        SetupPointDetect();
        State = EnemyState.PATROL_STATE;
        Follow = false;
        
        Body.linearVelocity = Vector2.zero;
    }

    private void DealDamage()
    {
        if(targetNPC==null)
        {
            targetNPC = Target.GetComponent<NPC>();
        }
        attackTimer=0;
        targetNPC.IsPositionLocked = false;
        animator.SetTrigger(AttackTrigger);
        if (targetNPC != null)
        {
            targetNPC.Deal(damage);
            if (targetNPC.HP <= 0)
            {
                targetNPC.Death();
                Target=null;               
                BossReturn();
            }
            else
            {
                //PlayStunAnimation();
            }
        }
    }

    public void EatFood()
    {
        State = EnemyState.EAT_STATE;
        Follow = false;
        Body.linearVelocity = Vector2.zero;
        PlayEatAnimation();
        Invoke(nameof(BossReturn), 5f);
    }

    public void enum_emo(string anim_name)
    {
        if (emoAnimation == null) return;
        int rnd = UnityEngine.Random.Range(0, 10);
        if (rnd > 3) return;
        emoAnimation.SetActive(true);
        
        Invoke("hide_emo", 2f);
    }

    void hide_emo()
    {
        emoAnimation.SetActive(false);
    }
}