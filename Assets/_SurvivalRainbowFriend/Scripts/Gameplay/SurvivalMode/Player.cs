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
    
    public Light2D LightObject;
    public TextMeshProUGUI nameText;
    public VariableJoystick variableJoystick;
    public PlayerAIM playerAIM;
    public PlayerNPC playerNPC;
  
    public static Vector2 Player_Pos;//player position (x,y)

    public Vector2 minpos, maxpos;

    public static bool go;// for PlayerShooting(stabilization) and touch control - is the character goes?
    public bool IsAlivePlayer = true;
   
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
   
    private bool run=false;
    private bool die=false;
    private bool box=false;
    private bool hide = false;
    private bool run0 = false;
    private bool run02 = false;
    private bool boosterRun = false;
    private int walk = -1;
    private TypeMission typeMission;
    
   
    public void InitPlayer()
    {
        LightObject.pointLightOuterRadius = GameManager.Instance.levelData.radiusLight;
        string box = BoxItemData.Instance.userSkinBoxData.currentBox.BoxObject.nameVariable;
        walk = Random.Range(1, 4);
        
        playerNPC.PlayRndBeginGame();
    }
    private Vector2 direction;
    private Vector2 moveStand;
    void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "TurnOnWin");
        NotificationCenter.DefaultCenter().AddObserver(this, "ChangeName");
        SpeedNormal = Player.speed;
        SpeedBox = Player.speed * 0.4f;
        
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
       
        if (!StaticData.IsPlay)
        {
            return;
        }     
        direction = Vector2.up * variableJoystick.Vertical + Vector2.right * variableJoystick.Horizontal;
        if (direction != Vector2.zero)
        {
            moveStand=direction;
        }
        playerNPC.animator.transform.localScale = moveStand.x > 0 ? Vector3.one*0.7f : StaticData.ScaleInverse*0.7f;
        //divide the vector by its length to get the angle
        run = direction.magnitude > 0;
        playerNPC.run=run;
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
                playerNPC.PlayEmotionRun();
            }
            if (box)
            {
               
            }
           
        }
        playerNPC.run = run;
    }
    private void LateUpdate()
    {
        DrawInstruction();
    }
  
 
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsAlivePlayer) return;    
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
   
    public void Stop()
    {
        
    }
    public void ProccessDie(string bossName)
    {
        if (!IsAlivePlayer) return;
        IsAlivePlayer = false;
           
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        GameManager.Instance.ProcessDeath();
        Invoke("Die", 3f);
    }
   
    public void Die()
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
        playerNPC.HideAndSneek(!hide);
    }
   
    public void TurnOnWin()
    {
        if (!IsAlivePlayer) return;
        
        playerNPC.PlayWinEmotion();
        LightObject.gameObject.SetActive(false);
    }
    public void TurnOnLose()
    {
        if (!StaticData.IsPlay) return;
        if (!IsAlivePlayer) return;
        
        playerNPC.PlayEmotionTimeout();
    }
    public void RecoverPlayer()
    {
        gameObject.tag = "Wall";
        IsAlivePlayer = true;       
        
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;
        playerNPC.RecoverFriend();
        Invoke("Balance", 3f);
    }
    public void RevivePlayer()
    {
        gameObject.tag = "Wall";
        transform.position = transform.position + Vector3.right * 3f;
        IsAlivePlayer = true;   
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;
        
        Invoke("Balance", 3f);
    }
    void Balance()
    {
        playerNPC.Balance();
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
   
    #endregion
}


