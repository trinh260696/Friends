using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "OpenDoor");
    }
    public void OpenDoor()
    {
        GetComponent<Animation>().Play("opendoor");
    }
    void SetActive()
    {
        gameObject.SetActive(false);
    }

}

