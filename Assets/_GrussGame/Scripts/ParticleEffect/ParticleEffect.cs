using UnityEngine;
using System.Collections;

public class ParticleEffect : MonoBehaviour {
	
	ParticleSystem ps;
	public string sortingLayer="UI";
	public int sortingOrder=10;
	public bool killAfterLifetime = true;
	
	void  Awake (){
		ps = GetComponent<ParticleSystem>();
	
		ps.GetComponent<Renderer>().sortingLayerName = sortingLayer;
		ps.GetComponent<Renderer>().sortingOrder = sortingOrder;
		if (killAfterLifetime) StartCoroutine("Kill");
	}
	
	IEnumerator Kill (){
		yield return new WaitForSeconds(6f);
		Destroy(gameObject);
	}

}