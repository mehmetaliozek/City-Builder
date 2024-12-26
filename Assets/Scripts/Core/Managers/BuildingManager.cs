using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    private List<BuildingModel> building = new List<BuildingModel>();
    public void Awake()
    {
        // Singleton
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
        GetBuildings();
    }

    // Yeni bir bina ekler
    public bool AddBuilding(BuildingModel newBuilding)
    {
        try
        {
            Debug.Log($"{newBuilding.BuildingName} binasý baþarýyla eklendi.");
            return DatabaseManager.Instance.AddBuilding(GameManager.Instance.userModel.UserId, newBuilding);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bina eklenemedi: {ex.Message}");
            return false;
        }
    }

    public bool DeleteBuilding(BuildingModel building)
    {
        try
        {
            return DatabaseManager.Instance.DeleteBuilding(building.Id);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bina eklenemedi: {ex.Message}");
            return false;
        }
    }

    private void GetBuildings()
    {
        foreach (var (id, buildingId, position, rotation) in DatabaseManager.Instance.SelectBuilding(PlayerPrefs.GetInt("LoadedSceneUserId")))
        {
            GameObject build = BuildingSystem.Instance.GetBuild(buildingId);
            build.GetComponent<BuildingModel>().Id = id;
            build.GetComponent<BuildingModel>().Position = position;
            build.GetComponent<BuildingModel>().Rotation = rotation;
            BuildingSystem.Instance.SavedBuildTakeArea(build);
        }
    }


}
