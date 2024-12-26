using System;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    public int Id;
    public int BuildingId;
    public BuildingType BuildingType;

    // Bina adý (Ev, Banka, Market, Fabrika)
    public string BuildingName;

    // Büyüklük (Small, Large, Skyscraper)
    public BuildingSize buildingSize;
    public int EnergyCost;
    public int FoodCost;
    public int MoneyCost;
    public ResourceType ProductionType; // Üretim türü (örn: "money", "food")
    public int ProductionRate; // Üretim miktarý (örnek: saniyede üretim miktarý)
    public DateTime CreatedAt;

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