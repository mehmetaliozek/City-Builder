using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    // Singleton Pattern
    public static RoadManager Instance { get; private set; }

    private List<RoadModel> roads = new List<RoadModel>();

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        GetRoads();
    }

    public bool AddRoad(RoadModel newRoad)
    {
        try
        {
            return DatabaseManager.Instance.AddRoad(GameManager.Instance.userModel.UserId, newRoad);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Yol eklenemedi: {ex.Message}");
            return false;
        }
    }

    public bool DeleteRoad(RoadModel road)
    {
        try
        {
            return DatabaseManager.Instance.DeleteRoad(road.Id);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bina eklenemedi: {ex.Message}");
            return false;
        }
    }

    private void GetRoads()
    {
        foreach (var (id, roadId, position, rotation) in DatabaseManager.Instance.SelectRoad(PlayerPrefs.GetInt("LoadedSceneUserId")))
        {
            GameObject build = BuildingSystem.Instance.GetRoad(roadId);
            build.GetComponent<RoadModel>().Id = id;
            build.GetComponent<RoadModel>().Position = position;
            build.GetComponent<RoadModel>().Rotation = rotation;
            BuildingSystem.Instance.SavedBuildTakeArea(build);
        }
    }
}
