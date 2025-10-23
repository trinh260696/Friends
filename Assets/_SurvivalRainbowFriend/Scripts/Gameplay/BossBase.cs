using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Unity.VisualScripting;
public class BossBase : MonoBehaviour
{
    public string nameBoss = "Boss";
    public int IQ = 1;
    public float attackSpeed = 1f;
    public int damage = 10;
    
    private EnemyState _state;
    public EnemyState State { get => _state; set => _state = value; }
    
    public Animator animator;
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
    public float Speed=5.0f;     
    //blood sprites (GameObjects)
   
   
    public bool isChase = false;
    public bool isMoveWayPoint = false;
    

    private RaycastHit2D hit;
    public bool killing = false;
    private List<Vector2> Waypoints;
    private List<Vector2> DetectPoints;
    private const string BossRunProperties = "bossrun";
    private const string BossChaseProperties = "bosschase";
    public const string BossKillProperties = "bosskill";
    private const string BossDamageProperties = "bosshit";
    private const string TriggerDie = "dietrigger";
    private const string BossEatProperties = "bosseat";
    private const string ReviveTrigger = "revivetrigger";
    private bool run = false;
    
    public float RANGE_FAR = 10f;
    public float RANGE_NEAR = 2f;
    public float v_slow = 2f;
    public float v_fast = 6f;
    private float stunDuration = 0f;
    private float stunTimer = 0f;
    private Vector2 lastTargetPos = Vector2.zero;
    private float velocityMultiplier = 1f;
       
    public void SetKeyAnimation()
    {
        animator.SetBool(BossBase.BossRunProperties, run);
        animator.SetBool(BossBase.BossChaseProperties, Follow);
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
                run = true;
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
            DetectPoints.Clear();
            for (int i = 0; i < resultSecond.Length; i++)
            {
                DetectPoints.Add(Waypoints[resultSecond[i]]);
            }
        }
        else
        {
            DetectPoints.Clear();
        }
           
    }
    public void BossReturn()
    {
        killing = false;
        Follow = false;
        run = true;
        _travelPointIndex = 0; // Reset về điểm đầu tiên
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
        
        if (!isMoveWayPoint) return;
        SetKeyAnimation();

        if (!StaticData.IsPlay) return;
        if (Target == null) return;
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
            State = EnemyState.CHASE_STATE;
        }else if (col.CompareTag("Finish"))
        {
            BossReturn();
        }
    }
    public virtual void OnTriggerStay2D(Collider2D col)
    {

    }
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
       
              
    }
    public virtual void Kill(Transform Destination)
    {
        killing = true;
        _follow = false;
        // Stop TravelPoint

        State = EnemyState.FIGHT_STATE;
        animator.SetTrigger(BossBase.BossKillProperties);
        Vector3 dir = Destination.position - transform.position;

        ContentAssistant.Instance.PlayEffectDeath(Destination, Destination.position+Vector3.right*Mathf.Sign(dir.x)+Vector3.up);
    }
    void OnTravelPoint()
    {
        State = EnemyState.PATROL_STATE;
    }
    void TravelPoint()
    {
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

        // Cập nhật hướng nhìn của boss
        animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
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
        animator.SetTrigger(BossDamageProperties);
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
        if (nearestFood != null && IQ <= 3)
        {
            BaitManager.Instance.MoveEnemyTowardFood(this, nearestFood);
            State = EnemyState.EAT_STATE;
            return;
        }

        if (Target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, Target.position);

            if (distanceToTarget < RANGE_NEAR)
            {
                AttackTarget();
            }
            else if (distanceToTarget < RANGE_FAR)
            {
                ChaseTarget();
            }
            else
            {
                PatrolState();
            }

            lastTargetPos = Target.position;
        }
        else
        {
            PatrolState();
        }
    }

    private void ChaseTarget()
    {
        State = EnemyState.CHASE_STATE;
        Follow = true;
        run = true;
        float currentSpeed = v_fast;
        
        Vector2 direction = (Target.position - transform.position).normalized;
        Body.linearVelocity = direction * currentSpeed;
        animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : new Vector3(-0.7f, 0.7f, 1f);
    }

    private void AttackTarget()
    {
        State = EnemyState.FIGHT_STATE;
        Follow = true;
        run = false;
        
        Vector2 direction = (Target.position - transform.position).normalized;
        animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : new Vector3(-0.7f, 0.7f, 1f);
        
        float distanceToTarget = Vector2.Distance(transform.position, Target.position);
        if (distanceToTarget > RANGE_NEAR * 0.8f)
        {
            Body.linearVelocity = direction * v_slow;
        }
        else
        {
            Body.linearVelocity = Vector2.zero;
            DealDamage();
        }
    }

    private void PatrolState()
    {
        State = EnemyState.PATROL_STATE;
        Follow = false;
        run = false;
        Body.linearVelocity = Vector2.zero;
    }

    private void DealDamage()
    {
        if (Target == null) return;

        NPC npc = Target.GetComponent<NPC>();
        if (npc != null)
        {
            npc.HP -= damage;
            if (npc.HP <= 0)
            {
                npc.Death();
                BossReturn();
            }
            else
            {
                animator.SetTrigger(BossDamageProperties);
            }
        }
    }

    public void EatFood()
    {
        State = EnemyState.EAT_STATE;
        Follow = false;
        Body.linearVelocity = Vector2.zero;
        animator.SetTrigger("eat");
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