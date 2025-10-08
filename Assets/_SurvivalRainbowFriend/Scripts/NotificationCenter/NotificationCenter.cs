using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NotificationCenter : MonoBehaviour
{
	private static NotificationCenter defaultCenter;
	public static NotificationCenter DefaultCenter()
	{
		if (!defaultCenter)
		{
			GameObject notificationObject = new GameObject("Default Notification Center");
			defaultCenter = notificationObject.AddComponent<NotificationCenter>();
			DontDestroyOnLoad(defaultCenter);
		}

		return defaultCenter;
	}

	Hashtable notifications = new Hashtable();

	public void AddObserver(Component observer, String name) { AddObserver(observer, name, null); }
	public void AddObserver(Component observer, String name, object sender)
	{
		if (name == null || name == "") { Debug.Log("Null name specified for notification in AddObserver."); return; }

		if (!notifications.ContainsKey(name))
		{
			notifications[name] = new List<Component>();
		}

		List<Component> notifyList = (List<Component>)notifications[name];

		if (!notifyList.Contains(observer)) { notifyList.Add(observer); }
	}

	public void RemoveObserver(Component observer, String name)
	{
		List<Component> notifyList = (List<Component>)notifications[name];

		if (notifyList != null)
		{
			if (notifyList.Contains(observer)) { notifyList.Remove(observer); }
			if (notifyList.Count == 0) { notifications.Remove(name); }
		}
	}

	public void PostNotification(Component aSender, String aName) { PostNotification(aSender, aName, null); }
	public void PostNotification(Component aSender, String aName, object aData) { PostNotification(new Notification(aSender, aName, aData)); }
	public void PostNotification(Notification aNotification)
	{
		if (aNotification.name == null || aNotification.name == "") { Debug.Log("Null name sent to PostNotification."); return; }

		List<Component> notifyList = (List<Component>)notifications[aNotification.name];
		if (notifyList == null) { Debug.Log("Notify list not found in PostNotification."); return; }

		notifyList = new List<Component>(notifyList);

		List<Component> observersToRemove = new List<Component>();

		foreach (Component observer in notifyList)
		{
			if (!observer)
			{
				observersToRemove.Add(observer);
			}
			else
			{
				observer.SendMessage(aNotification.name, aNotification, SendMessageOptions.DontRequireReceiver);
			}
		}

		foreach (Component observer in observersToRemove)
		{
			notifyList.Remove(observer);
		}
	}
}

public class Notification
{
	public Component sender;
	public String name;
	public object data;

	public Notification(Component aSender, String aName) { sender = aSender; name = aName; data = null; }
	public Notification(Component aSender, String aName, object aData) { sender = aSender; name = aName; data = aData; }
}
