using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public int Energy
    {
        get => Energy;
        private set => resourceItem[0].SetAmount(value);
    }
    public int Food
    {
        get => Food;
        private set => resourceItem[1].SetAmount(value);
    }
    public int Money
    {
        get => Money;
        private set => resourceItem[2].SetAmount(value);
    }

    private float ResourceUpdateTime = 10f;
    private float GameTime = 0f;

    private List<ResourceItem> resourceItem = new List<ResourceItem>();

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            resourceItem = GameObject.FindGameObjectWithTag(Tags.Resource).GetComponentsInChildren<ResourceItem>().ToList();
        }

        Initialize();
    }

    public void Initialize()
    {
        var resources = DatabaseManager.Instance.GetResources(PlayerPrefs.GetInt("LoadedSceneUserId"));

        Energy = resources.Energy;
        Food = resources.Food;
        Money = resources.Money;
    }

    public void FixedUpdate() {
        if (GameManager.Instance.userModel.UserId != PlayerPrefs.GetInt("LoadedSceneUserId")) return;
        GameTime += Time.fixedDeltaTime;
        if (GameTime >= ResourceUpdateTime)
        {
            DatabaseManager.Instance.CallProcessBuildingProduction();
            DatabaseManager.Instance.CallReduceBuildingCost();
            Initialize();
            GameTime = 0;
        }
    }
}
