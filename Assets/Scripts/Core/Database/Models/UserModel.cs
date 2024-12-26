using System;
using UnityEngine;

[Serializable]
public class UserModel
{
    public int UserId = -1;
    public string Username  = string.Empty;
    public string PasswordHash = string.Empty;
    public string Email = string.Empty;
    public bool IsBanned = false;
    public DateTime CreatedAt  = DateTime.Now;
    public int UserRoleID = -1;
    public int Energy = 0;
    public int Money = 0;
    public int Food = 0; 

    public static UserModel FromJson(string json)
    {
        return JsonUtility.FromJson<UserModel>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
