using UnityEngine;

public class FBFriend
{
    public string name;
    public string first_name;
    public string last_name;
    public string id;

    private Texture avatar;

    public void SetAvatar(Texture avatar)
    {
        this.avatar = avatar;
    }

    public Texture GetAvatar()
    {
        return avatar;
    }
}