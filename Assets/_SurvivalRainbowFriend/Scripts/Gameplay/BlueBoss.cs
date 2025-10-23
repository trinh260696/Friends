using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CatBoss : BossBase
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
               
            }          
        }
        public override void Kill(Transform Destination)
        {
            base.Kill(Destination);
            ContentAssistant.Instance.GetItem("3D_Hit_03", Destination.transform.position+Vector3.right + Vector3.up);
            AudioManager.instance.Play("PowerPunch");
        }
      
   
}


