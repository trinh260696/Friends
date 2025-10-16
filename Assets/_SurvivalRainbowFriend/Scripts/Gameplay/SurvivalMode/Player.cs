using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Spine;
using Spine.Unity;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public Light2D LightObject;
    public TextMeshProUGUI nameText;
    public SpriteRenderer decoyObj;
    public GameObject emoji;
    public VariableJoystick variableJoystick;
    public PlayerAIM playerAIM;
    public BodyPart bodyPart;
    public StateFriend stateFriend = StateFriend.FRIEND_INIT;
    public static Vector2 Player_Pos;//player position (x,y)

    public Vector2 minpos, maxpos;

    public static bool go;// for PlayerShooting(stabilization) and touch control - is the character goes?
    public bool IsAlivePlayer = true;
    public Animator animator;
    public Rigidbody2D Body;//Player body
    public GameObject Blood1;//for enemy bullet ("EnBul 1")
    public GameObject BloodParticle;
    public Transform[] ObjectInstructions;
    public Transform ObjectInstructionsWin;
  
    public static float Move_Angle;//used in PlayerAIM, if player has not target      
    public Transform Move_Arrow;//arrow on the joystick
    public static float speed = 6.0f;//move speed
    public float SpeedNormal;
    public float SpeedBox;
    public float SpeedReal;
    private const string RunProperties = "run_normal";
    private const string ReturnProperties = "run_return";
    private const string WinProperties = "WinTrigger";
    private const string LoseProperties1 = "HitTrigger1";
    private const string LoseProperties2 = "HitTrigger2";
    private const string FailTrigger = "FailTrigger";
    private const string DieProperties = "die";
    private const string HideProperties = "hide";
    private const string ReviveProperties = "ReviveTrigger";
    private const string RunBoosterProperties = "run_booster_return";
    
    private const string HideTriggerProperties = "HideTrigger";
    private const string WalkProperties = "walk";
    private bool run=false;
    private bool die=false;
    private bool box=false;
    private bool hide = false;
    private bool run0 = false;
    private bool run02 = false;
    private bool boosterRun = false;
    private int walk = -1;
    private TypeMission typeMission;
    public SpriteRenderer boxSpriteRenderer;
    private Dictionary<string, bool> BoxDictionary = new Dictionary<string, bool>
    {
        {"carton",false },{"drum_box",false},{"shove",false},{"snow",false},{ "tree",false},{"tnt_box",false},{"gift_box",false},{"toe",false}
    };

    private void SetKeyAnimation()
    {
       
        run0 = run & boosterRun;
        animator.SetBool(RunProperties, run);
        animator.SetBool(ReturnProperties, run && box);
        animator.SetBool(HideProperties, hide);
       
        animator.SetBool(RunBoosterProperties, run0);
        animator.SetBool(RunBoosterProperties, run0 && box);
       
        animator.SetInteger(WalkProperties, walk);
        animator.SetBool(DieProperties, die);
    }
    void enum_emo(string anim_name)
    {
        int rnd = UnityEngine.Random.Range(0, 10);
        if (rnd > 3) return;
        emoji.SetActive(true);
      
        Invoke("hide_emo", 2f);
    }
    void hide_emo()
    {
        emoji.SetActive(false);
    }
    public void InitPlayer()
    {
        LightObject.pointLightOuterRadius = GameManager.Instance.levelData.radiusLight;
        string box = BoxItemData.Instance.userSkinBoxData.currentBox.BoxObject.nameVariable;
        walk = Random.Range(1, 4);
        BoxDictionary[box] = true;
        PlayRndBeginGame();
    }
    private Vector2 direction;
    private Vector2 moveStand;
    void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "TurnOnWin");
        NotificationCenter.DefaultCenter().AddObserver(this, "ChangeName");
        SpeedNormal = Player.speed;
        SpeedBox = Player.speed * 0.4f;
        boxSpriteRenderer.gameObject.SetActive(false);
        nameText.text = UserData.Instance.GameData.name;
        minpos = UserData.Instance.GameData.vip == 1 ? new Vector2(-7.5f, -5) : new Vector2(-7.5f, -4);
    }
    void Update()
    {       
        go = false;//zero out to a stationary position
        Player_Pos = new Vector2(transform.position.x, transform.position.y);//player position (x,y)
        if (StaticData.IsPlay) {
        }                                                                    
    }
    void FixedUpdate()
    {
        SetKeyAnimation();
        if (!StaticData.IsPlay)
        {
            return;
        }     
        direction = Vector2.up * variableJoystick.Vertical + Vector2.right * variableJoystick.Horizontal;
        if (direction != Vector2.zero)
        {
            moveStand=direction;
        }
        animator.transform.localScale = moveStand.x > 0 ? Vector3.one*0.7f : StaticData.ScaleInverse*0.7f;
        //divide the vector by its length to get the angle
        run = direction.magnitude > 0;
        if (run)
        {
            SpeedReal = hide ? SpeedBox : SpeedNormal;
            Body.AddForce(direction / direction.magnitude * SpeedReal);
            if (Time.frameCount % 11 == 0)
            {
                AudioManager.instance.Play(string.Format("run{0}", Random.Range(1, 5)));               
            }
            if (Time.frameCount % 100 == 0)
            {
                PlayEmotionRun();
            }
            if (box)
            {
               
            }
           
        }      
    }
    private void LateUpdate()
    {
        DrawInstruction();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsAlivePlayer) return;
      
        if (col.gameObject.name.Contains("GreenBoss"))
        {
            AudioManager.instance.Play("GreenWarning");
        }
        
        if (box)
        {
            if (col.gameObject.name == "WinArea")
            {
              //  GameManager.Instance.AddMission(typeMission);
                boxSpriteRenderer.gameObject.SetActive(false);
                box = false;
            }
            return;
        }
        if (col.gameObject.tag.Equals("Box"))//collision with Box
        {
            PlayEmotionGetItem();
            var boxScript = col.gameObject.GetComponent<BodyPart>();
            if (boxScript.Free)
            {
                AudioManager.instance.Play("Pick");
                boxScript.Hide();
                boxScript.gameObject.SetActive(false);
                box = true;
               // typeMission = boxScript.typeMission;

                boxSpriteRenderer.gameObject.SetActive(true);
                boxSpriteRenderer.sprite = Resources.Load<Sprite>("Avatar/" + typeMission.ToString());
                if (hide)
                {
                    boxSpriteRenderer.gameObject.SetActive(false);
                }
            }
            boxScript.gameObject.SetActive(false);
        }
       

    }
 
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsAlivePlayer) return;
        if (box)
        {
            if (col.gameObject.name == "WinArea")
            {
                PlayWinEmotion();
               // GameManager.Instance.AddMission(typeMission);
                boxSpriteRenderer.gameObject.SetActive(false);
                box = false;
            }
            return;
        }
        if (col.collider.CompareTag("Wall"))
        {
            var dir = (col.collider.transform.position - transform.position).normalized;
            Body.AddForce(dir * 1f);
        }
    }
    public void ChangeName()
    {
        nameText.text = UserData.Instance.GameData.name;
    }
    public void DrawInstruction()
    {
        if (box)
        {
            for (int i = 0; i < ObjectInstructions.Length; i++)
            {
                //ObjectInstructions[i].position = Vector3.up * 1000;
                ObjectInstructions[i].gameObject.SetActive(false);
            }
            ObjectInstructionsWin.gameObject.SetActive(true);
            //huong
            Vector2 moveDir = (GameManager.Win_Pos - (Vector2)transform.position);
            //khoang cach
            int distance = (int)Vector2.Distance((Vector2)GameManager.Win_Pos, (Vector2)transform.position);
            ///Goc
            RectageDirect();
            float angle = Vector2.Angle( moveDir,Vector2.up);
            Vector3 cross = Vector3.Cross(Vector3.up, (Vector3)moveDir).normalized;

            ObjectInstructionsWin.GetChild(0).GetChild(0).transform.eulerAngles = cross * angle;
            ObjectInstructionsWin.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0} m", distance);
            ObjectInstructionsWin.GetChild(0).GetChild(0).GetChild(0).transform.localEulerAngles = cross * angle * -1;
            return;
        }
        else
        {
            ObjectInstructionsWin.gameObject.SetActive(false);
            if (GameManager.Instance.currentTime > 20) return;
            var colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, 60, 1 << 11);
            if (colliders.Length == 0)
            {
                for (int i = 0; i < ObjectInstructions.Length; i++)
                {
                    ObjectInstructions[i].gameObject.SetActive(false);
                }
                return;
            }
            colliders = colliders.OrderBy(c => Vector2.Distance((Vector2)transform.position, (Vector2)c.transform.position)).ToArray();
            for (int i = 0; i < ObjectInstructions.Length; i++)
            {

                if (i < colliders.Length)
                {
                    float x = Vector2.Distance(colliders[i].transform.position, ObjectInstructions[i].transform.position);
                    if (x <= 2)
                    {
                        ObjectInstructions[i].gameObject.SetActive(false);
                    }
                    else
                        ObjectInstructions[i].gameObject.SetActive(true);
                    Vector2 dir = (colliders[i].transform.position - transform.position);
                    Vector2 dirNormal = dir.normalized;
                    int distance = (int)Vector2.Distance((Vector2)colliders[i].transform.position , (Vector2)transform.position);
                    //RectageDirect
                    float xPos, yPos;
                    if(dir.y>maxpos.y || dir.y < minpos.y)
                    {
                        yPos = dir.y > maxpos.y ? maxpos.y : minpos.y;
                        xPos = dir.x * yPos / dir.y;
                    }
                    else if(dir.x>maxpos.x || dir.x < minpos.x)
                    {
                        xPos = dir.x > maxpos.x ? maxpos.x : minpos.x;
                        yPos = dir.y * xPos / dir.x;
                    }
                    else
                    {
                        xPos = dir.x; yPos = dir.y;
                    }
                    ObjectInstructions[i].transform.position = transform.position + new Vector3(xPos, yPos, 0);
                    //ObjectInstructions[i].transform.position = transform.position + new Vector3(
                    //                                            Mathf.Clamp(dir.x, minpos.x, maxpos.x),
                    //                                            Mathf.Clamp(dir.y, minpos.y, maxpos.y),
                    //                                            1
                    //                                            );
                    Vector3 cross = Vector3.Cross(Vector3.up,(Vector3)dir).normalized;
                    float angle = Vector2.Angle( dir,Vector2.up);
                    ObjectInstructions[i].GetChild(0).GetChild(0).transform.eulerAngles = cross * angle;
                    ObjectInstructions[i].GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0} m", distance);
                    ObjectInstructions[i].GetChild(0).GetChild(0).GetChild(0).transform.localEulerAngles = cross * angle * -1;
                    
                }
                else
                {
                    ObjectInstructions[i].gameObject.SetActive(false);
                }

            }
        }
       
    }

    void RectageDirect()
    {
        Vector2 moveDir = (GameManager.Win_Pos - (Vector2)transform.position);
        ObjectInstructionsWin.position = transform.position + new Vector3(
            Mathf.Clamp(moveDir.x, minpos.x, maxpos.x),
            Mathf.Clamp(moveDir.y, minpos.y, maxpos.y),
            1
            );
    }
    public void PlayRndBeginGame()
    {
        string[] arr = new string[] { StaticParam.poke_emo, StaticParam.emo2, StaticParam.tamgiac_emo_begin, StaticParam.tamgiac_emo_loop, StaticParam.smile_emo };
        int rnd = UnityEngine.Random.Range(0, 5);
        enum_emo(arr[rnd]);
    }
    public void PlayEmoRun()
    {
        string[] arr = new string[] { StaticParam.run_emo, StaticParam.hurry_up, StaticParam.tamgiac_emo_begin, StaticParam.tamgiac_emo_loop, StaticParam.smile_emo };
        int rnd = UnityEngine.Random.Range(0, 5);
        enum_emo(arr[rnd]);
    }
    void PlayEmotionGetItem()
    {
        string[] arr = new string[] { StaticParam.here_emo, StaticParam.smile_emo, StaticParam.poke_emo };
        int rnd = UnityEngine.Random.Range(0, 3);
        enum_emo(arr[rnd]);
    }
    public void Stop()
    {
        
    }
    public void Damage(string bossName,int damage)
    {
        if (!IsAlivePlayer) return;
        StaticData.ENEMY_STRING = bossName;
       
        if (box)
        {
            boxSpriteRenderer.gameObject.SetActive(false);
            GameManager.Instance.RegenerateBox(transform.position,0);
        }
        if (HP <= 0) {
            IsAlivePlayer = false;
            animator.SetTrigger(Player.LoseProperties1);
            var colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = false;
            GameManager.Instance.ProcessDeath();
            Invoke("Die", 3f);
        }

    }
    public void DeathGreen()
    {
        //if (!IsAlivePlayer) return;
        //StaticData.ENEMY_STRING = "Green";
        //GameManager.Instance.ProcessDeath();
        //enum_emo(StaticParam.tired_emo);
        //animator.transform.localScale = Vector3.one * 0.7f;      
        //animator.SetTrigger(Player.LoseProperties2);
        //IsAlivePlayer = false;
        //if (box)
        //{
        //    boxSpriteRenderer.gameObject.SetActive(false);
        //     GameManager.Instance.RegenerateBox(transform.position,0);
        //}
        
        //var colliders = GetComponents<Collider2D>();
        //for (int i = 0; i < colliders.Length; i++)
        //    colliders[i].enabled = false;
        
    }
    void Die()
    {        
        die = true;
    }
    void ChangeTag()
    {
        if (hide)
        {
           
            gameObject.tag = "Hide";
            
        }
        else
        {
            gameObject.tag = "Player";          
        }
    }
    public void HideAndSneek()
    {
        hide = !hide;
        if (hide)
        {
            //animator.SetTrigger(Player.HideTriggerProperties);
            decoyObj.gameObject.SetActive(true);
            animator.gameObject.SetActive(false);
            boxSpriteRenderer.gameObject.SetActive(false);
        }
        else
        {
            decoyObj.gameObject.SetActive(false);
            animator.gameObject.SetActive(true);
            gameObject.tag = "Player";
            if (box)
            {
                Invoke("Show", 0.5f);
            }

        }
        Invoke("ChangeTag", 0.4f);
    }
    void Show()
    {
        boxSpriteRenderer.gameObject.SetActive(true);
    }
    public void TurnOnWin()
    {
        if (!IsAlivePlayer) return;
        foreach (var param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
        animator.SetTrigger(Player.WinProperties);
        PlayWinEmotion();
        LightObject.gameObject.SetActive(false);
    }
    public void TurnOnLose()
    {
        if (!StaticData.IsPlay) return;
        if (!IsAlivePlayer) return;
        foreach(var param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
        animator.SetTrigger(Player.FailTrigger);
        PlayEmotionTimeout();
    }
    public void RecoverPlayer()
    {
        gameObject.tag = "Wall";
        IsAlivePlayer = true;       
        foreach (var param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
        animator.SetTrigger(Player.ReviveProperties);
        if (box && !hide)
        {
            boxSpriteRenderer.gameObject.SetActive(true);
        }
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;
        Invoke("Balance", 3f);
    }
    public void RevivePlayer()
    {
        gameObject.tag = "Wall";
        transform.position = transform.position + Vector3.right * 3f;
        IsAlivePlayer = true;
        animator.SetTrigger(Player.ReviveProperties);
        if (box)
        {
            boxSpriteRenderer.gameObject.SetActive(false);
            box = false;
        }
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;
        if (hide)
        {
            decoyObj.gameObject.SetActive(true);
            animator.gameObject.SetActive(false);
        }
        Invoke("Balance", 3f);
    }
    void Balance()
    {
        if (hide)
        {
            gameObject.tag = "Hide";
            decoyObj.gameObject.SetActive(true);
            animator.gameObject.SetActive(false);
        }
        else
        {
            decoyObj.gameObject.SetActive(false);
            animator.gameObject.SetActive(true);
            gameObject.tag = "Player";
        }
           
        die = false;
    }
    #region booster
    public void BoostSpeed()
    {
        this.boosterRun = true;
        this.SpeedNormal +=   Player.speed*0.5f;
        this.SpeedBox = this.SpeedNormal * 0.4f;      
    }
    public void LightUp()
    {
        LightObject.gameObject.SetActive(false);
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
        string[] arr = new string[] { StaticParam.tired_emo, StaticParam.hurry_up, StaticParam.scare_emo, StaticParam.run_emo };
        int rnd = UnityEngine.Random.Range(0, 4);
        enum_emo(arr[rnd]);
    }
    public void PlayEmotionRun()
    {
        string[] arr = new string[] { StaticParam.run_emo, StaticParam.hurry_up, StaticParam.poke_emo, StaticParam.smile_emo };
        int rnd = UnityEngine.Random.Range(0, 4);
        enum_emo(arr[rnd]);
    }
    #endregion
}


