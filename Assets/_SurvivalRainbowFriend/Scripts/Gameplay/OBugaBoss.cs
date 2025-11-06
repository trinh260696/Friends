using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class OBugaBoss : BossBase
    {
        public const string RaiseProperties = "raise";
        public const string DownProperties = "down";
        private void Start()
        {
            
            StartCoroutine("RaiseOrDown");
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        public override void Update()
        {
            base.Update();
        }
       
        public override void OnTriggerEnter2D(Collider2D col)
        {
            base.OnTriggerEnter2D(col);
        }
        public void Recover()
        {
            beating = false;
            StartCoroutine("RaiseOrDown");
        }
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
        }
        
        public void Camouflage()
        {

        }
      
      
    }

