using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class FriendManager : MonoBehaviour
{
    public List<NPC> Friends;
    private float xPos;
    private float yPos;
    private float f = 180f;
    private float radius = 3f;
    private Vector2 centerPos;
    public void GenerateFriend(List<NPCObj> friends, Vector2 center)
    {
        centerPos = center;
        int number = friends.Count;
        Friends = new List<NPC>();
        // The corner that is used to start the polygon (parallel to the X axis).
        Vector2 startCorner = new Vector2(radius, 0) + center;

        // The "previous" corner point, initialised to the starting corner.
        Vector2 previousCorner = startCorner;
        for (int i=0; i<number; i++)
        {
            // Calculate the angle of the corner in radians.
            float cornerAngle = 2f * Mathf.PI / (float)number * i;

            // Get the X and Y coordinates of the corner point.
            Vector2 currentCorner = new Vector2(Mathf.Cos(cornerAngle) * radius, Mathf.Sin(cornerAngle) * radius) + center;

            // Draw a side of the polygon by connecting the current corner to the previous one.
            Debug.DrawLine(currentCorner, previousCorner);

            // Having used the current corner, it now becomes the previous corner.
            previousCorner = currentCorner;
            f = UnityEngine.Random.Range(0f, 2f);
            Vector2 pos1, pos2;
            pos1 = center; pos2 = center;
            Vector2 dir1 = (center - currentCorner).normalized;
            var ray1 = Physics2D.Raycast(currentCorner, dir1, 40, 1 << 13);
            if (ray1.collider != null)
            {
                pos1 = ray1.point - dir1;
            }
            var ray2 = Physics2D.Raycast(currentCorner, -dir1, 40, 1 << 13);
            if (ray2.collider != null)
            {
                pos2 = ray2.point + dir1;
            }
            int rnd = UnityEngine.Random.Range(0, 2);
            currentCorner = rnd == 1 ? pos1 : pos2;
            var go = ContentAssistant.main.GetItem<FriendNPC>("Friend");
            go.transform.position = currentCorner;
            go.transform.SetParent(transform,true);
            var animator = ContentAssistant.main.GetItem(friends[i].friendType.ToString());
            animator.transform.SetParent(go.transform,false);
            animator.transform.localPosition = Vector3.zero;
            var friend= go.GetComponent<NPC>();
            friend.Init(friends[i],animator,i);
            
            Friends.Add(friend);         
        }

    } 
    public void NotifyMakeNoise()
    {
        for(int i=0; i<Friends.Count; i++)
        {
            Friends[i].MakeNoise(centerPos);
        }
    }

    //public void ReviveFriend()
    //{
    //    for (int i = 0; i < Friends.Count; i++)
    //    {
    //        if (Friends[i].gameObject.activeSelf)
    //            Friends[i].ReviveFriend();

    //    }
    //}

    public void Stop()
    {
        for (int i = 0; i < Friends.Count; i++)
        {
            if (Friends[i].gameObject.activeSelf)
                Friends[i].StopGame();

        }
    }

    public void Recover()
    {
        for (int i = 0; i < Friends.Count; i++)
        {
            if (Friends[i].gameObject.activeSelf)
                Friends[i].RecoverFriend();

        }
    }
    public void PlayEmotionWin()
    {
        for (int i = 0; i < Friends.Count; i++)
        {
            if (Friends[i].gameObject.activeSelf)
                Friends[i].PlayWinEmotion();

        }
    }
    public void PlayEmotionLose()
    {
        for (int i = 0; i < Friends.Count; i++)
        {
            if (Friends[i].gameObject.activeSelf)
                Friends[i].PlayLoseEmotion();

        }
    }
    public void PlayEmotionTimeout()
    {
        for (int i = 0; i < Friends.Count; i++)
        {
            if (Friends[i].gameObject.activeSelf)
                Friends[i].PlayEmotionTimeout();

        }
    }
}
