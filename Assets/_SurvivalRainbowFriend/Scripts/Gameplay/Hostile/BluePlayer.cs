using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BluePlayer : MonoBehaviour
{
    public GameObject LightObject;
    public TextMeshProUGUI nameText;
    public VariableJoystick variableJoystick;
    public static Vector2 Player_Pos;//player position (x,y)
    public static bool go;// for PlayerShooting(stabilization) and touch control - is the character goes?
    
    public Animator animator;
    public Rigidbody2D Body;//Player body
    private Transform Target;
    public static float Move_Angle;//used in PlayerAIM, if player has not target      
    public Transform Move_Arrow;//arrow on the joystick
    public static float speed = 6.0f;//move speed
    public float SpeedNormal;
    public float SpeedBooster;
    public float SpeedReal;
    public bool notKilling = true;
    private const string WalkProperties = "walk";
    private const string RunProperties = "run";
   
    private const string WinProperties = "WinTrigger";  
    private const string FailTrigger = "FailTrigger";   
    private const string ReviveTrigger = "ReviveTrigger";   
    private const string AttackTriggerProperties = "AttackTrigger";
    private bool walk = false;
    private bool attack = false;
    private bool boosterRun = false;
    private bool run = false;
    private bool follow = false;
    private void SetKeyAnimation()
    {      
        run = walk & attack;     
        animator.SetBool(WalkProperties, walk);     
        animator.SetBool(RunProperties, run);       
    } 
    private Vector2 direction;
    private Vector2 moveStand;
    void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "TurnOnWin");
        NotificationCenter.DefaultCenter().AddObserver(this, "ChangeName");
        SpeedNormal = BluePlayer.speed;
        SpeedBooster = BluePlayer.speed * 2f;  
        
        nameText.text = UserData.Instance.GameData.name;
    }
    void Update()
    {
        go = false;//zero out to a stationary position
        Player_Pos = new Vector2(transform.position.x, transform.position.y);//player position (x,y)
        if (StaticData.IsPlay)
        {
        }
    }
    void FixedUpdate()
    {
        SetKeyAnimation();
        if (!StaticData.IsPlay || !notKilling)
        {
            return;
        }
        direction = Vector2.up * variableJoystick.Vertical + Vector2.right * variableJoystick.Horizontal;
        if (direction != Vector2.zero)
        {
            moveStand = direction;
        }
        animator.transform.localScale = moveStand.x > 0 ? Vector3.one*0.7f : StaticData.ScaleInverse*0.7f;
        //divide the vector by its length to get the angle
        walk = direction.magnitude > 0;
        if (walk)
        {
            SpeedReal = boosterRun ? SpeedBooster : SpeedNormal;
            Body.AddForce(direction / direction.magnitude * SpeedReal);
            if (Time.frameCount % 11 == 0)
            {
                AudioManager.instance.Play(string.Format("run{0}", Random.Range(1, 5)));
            }          
        }
        // DrawInstruction();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attack = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!StaticData.IsPlay) return;
        if (!notKilling) return;
        if (collision.collider.CompareTag("Wall"))
        {
            var dir = (collision.collider.transform.position - transform.position).normalized;
            Body.AddForce(dir);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("Kill Player");
            Target = collision.transform;
            collision.gameObject.GetComponent<NPC>().Death();
            Kill(collision.transform);          
        }
    }
    public  void Kill(Transform Destination)
    {
        notKilling = false;          
        LeanTween.cancel(gameObject);
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        Target = null;
        animator.SetTrigger(BluePlayer.AttackTriggerProperties);
        Vector3 dir = Destination.position - transform.position;
        if (dir.x > 0)
        {
            Destination.position = transform.position + Vector3.right  + Vector3.up * 0.25f;
            Destination.transform.GetChild(0).localScale = StaticData.ScaleInverse * 0.7f;
            animator.transform.localScale = Vector3.one * 0.7f;
        }
        else
        {
            Destination.position = transform.position - Vector3.right - Vector3.up * 0.25f;
            Destination.transform.GetChild(0).localScale = Vector3.one * 0.7f;
            animator.transform.localScale = StaticData.ScaleInverse * 0.7f;
        }
        Invoke("BlueReturn", 5f);
        ContainAssistant.Instance.PlayEffectDeath(Destination, Destination.position + Vector3.right * Mathf.Sign(dir.x) + Vector3.up);
    }
    public void BlueReturn()
    {
        attack = false;
        notKilling = true;         
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;       
    }
    public void ChangeName()
    {
        nameText.text = UserData.Instance.GameData.name;
    }
   
    public void Stop()
    {
        animator.SetTrigger(BluePlayer.ReviveTrigger);
    }
   
    public void TurnOnWin()
    {
        animator.SetTrigger(BluePlayer.WinProperties);
    }
    public void TurnOnLose()
    {
        animator.SetTrigger(BluePlayer.FailTrigger);
    }
    public void RevivePlayer()
    {

    }
   
    #region booster
    public void BoostSpeed()
    {
        this.boosterRun = true;
        this.SpeedReal = this.SpeedBooster;
    }
    public void LightUp()
    {
        LightObject.SetActive(false);
    }
    #endregion
}
