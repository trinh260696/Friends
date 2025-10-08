using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class BlueBoss : BossBase
    {
        
        private void Start()
        {
            NotificationCenter.DefaultCenter().AddObserver(this, "PopulatePos");
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        public override void Update()
        {
            base.Update();
        }
       
        public override void OnInit()
        {
            base.OnInit();
           
        }
        public override void OnTriggerEnter2D(Collider2D col)
        {
            if (!StaticData.IsPlay) return;
            base.OnTriggerEnter2D(col);
            if (killing) return;
            if (col.gameObject.CompareTag("Player"))
            {
                if (!Follow&&!killing)
                {
                    Target = col.transform;
                    Follow = true;
                    StopDetect();                
                }
            }          
        }
        public override void Kill(Transform Destination)
        {
            base.Kill(Destination);
            ContainAssistant.Instance.GetItem("3D_Hit_03", Destination.transform.position+Vector3.right + Vector3.up);
            AudioManager.instance.Play("PowerPunch");
        }
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
        }
        public override void OnTriggerStay2D(Collider2D col)
        {
            if (!StaticData.IsPlay) return;
            
            base.OnTriggerStay2D(col);
            if (!StaticData.IsPlay) return;
            if (!StaticData.ISREADY) return;
            if (killing) return;
            if (col.gameObject.CompareTag("Player"))
            {             
                if (!Follow &&!killing)
                {
                    Target = col.transform;
                    Follow = true;
                    StopDetect();
                }
            }else if (Target)
            {
                if (Target.name == col.name && col.gameObject.CompareTag("Hide"))
                {
                    Target = null;
                    Follow = false;
                    StartDetect();
                }
            }
        }
   
}


