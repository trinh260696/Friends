using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ContainAssistant : MonoBehaviour {

	public static ContainAssistant Instance;
	public GameObject[] items;
	public GameObject[] effects;

	public GameObject blueBoss;

	private Dictionary<string, GameObject> content = new Dictionary<string, GameObject>();	
	private GameObject zObj;

	void Awake()
	{
		Instance = this;
		content.Clear ();
		foreach (GameObject item in  items)
			content.Add (item.name, item);
		foreach (GameObject effect in effects)
			content.Add (effect.name, effect);
		content.Add(blueBoss.name, blueBoss);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public T GetItem<T> (string key) where T : Component {
		return ((GameObject) Instantiate (content [key])).GetComponent<T>();
	}

	public GameObject GetItem (string key) {
		return (GameObject) Instantiate (content [key]);
	}

	public T GetItem<T> (string key, Vector3 position) where T : Component {
		zObj = GetItem (key);
		zObj.transform.position = position;
		return zObj.GetComponent<T>();
	}

	public GameObject GetItem(string key, Vector3 position) {
		zObj = GetItem (key);
		zObj.transform.position = position;
		return zObj;
	}

	public GameObject GetItem(string key, Vector3 position, Quaternion rotation) {
		zObj = GetItem (key, position);
		zObj.transform.rotation = rotation;
		zObj.name = key;
		return zObj;
	}
	IEnumerator play_effect(Transform tr,Vector3 pos)
	{
		for (int i = 0; i < 3; i++)
		{
			ContainAssistant.Instance.GetItem("BloodPunch", pos, Quaternion.identity);
			yield return new WaitForSeconds(1f);
		}
		GetItem("Blood 1", pos, Quaternion.Euler(0, 0, tr.rotation.eulerAngles.z + Random.Range(-15, 15) + 180));
		yield return new WaitForSeconds(1f);
		GetItem("Blood 2", pos, Quaternion.Euler(0, 0, tr.rotation.eulerAngles.z + Random.Range(-15, 15) + 180));
		yield return new WaitForSeconds(1f);
		GetItem("BloodDead", pos, Quaternion.Euler(0, 0, tr.rotation.eulerAngles.z + Random.Range(-15, 15) + 180));
	}
	public void PlayEffectDeath(Transform tr, Vector3 pos)
    {
		StartCoroutine(play_effect(tr, pos));
    }
}
public struct ContainAssistantItem {
	public GameObject item;
	public string category;
}
