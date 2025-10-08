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
            killing = false;
            StartCoroutine("RaiseOrDown");
        }
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
        }
        
        public void Camouflage()
        {

        }
        IEnumerator RaiseOrDown()
        {
            while (StaticData.IsPlay)
            {
                animator.SetTrigger(OBugaBoss.RaiseProperties);
                yield return new WaitForSeconds(5f);
                animator.SetTrigger(OBugaBoss.DownProperties);
                yield return new WaitForSeconds(5f);
            }
        }
        public override void Kill(Transform Destination)
        {
            StopCoroutine("RaiseOrDown");
            base.Kill(Destination);
            ContainAssistant.Instance.GetItem("3D_Hit_05", Destination.transform.position + Vector3.up);
            Invoke("Recover", 5f);
        }
    }

