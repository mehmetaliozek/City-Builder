using System;
using UnityEngine;

public class RoadModel : MonoBehaviour
{
    public int Id;
    public int RoadId;
    public int MoneyCost; 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Position
    {
        get
        {
            return JsonUtility.ToJson((Mathf.Round(transform.position.x * 100f) / 100f, 0, Mathf.Round(transform.position.z * 100f) / 100f));
        }
        set
        {
            (float x, float y, float z) = JsonUtility.FromJson<(float x, float y, float z)>(value);
            transform.position = new Vector3(x, 0.01f, z);
        }
    }

    public string Rotation
    {
        get
        {
            return JsonUtility.ToJson((0, transform.rotation.eulerAngles.y, 0));
        }
        set
        {
            (float x, float y, float z) = JsonUtility.FromJson<(float x, float y, float z)>(value);
            transform.rotation = Quaternion.Euler(x, y, z);
        }
    }
}
