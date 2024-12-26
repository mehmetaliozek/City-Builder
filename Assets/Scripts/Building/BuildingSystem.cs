using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;

    [Header("Map Layout")]
    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase whiteTile;

    [Space(10)]
    [Header("Building Objects")]
    public GameObject buildParent;
    public GameObject roadParent;
    public GameObject[] largeBuilds;
    public GameObject[] smallBuilds;
    public GameObject[] skyscrappers;
    public GameObject[] roads;
    private List<GameObject[]> buildCategories;
    private List<GameObject> buildings = new List<GameObject>();
    private PlaceableObject objectToPlace = null;

    public bool IsDestroyMode { get; private set; }

    #region Unity methods
    public void Awake()
    {
        Instance = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
        buildCategories = new List<GameObject[]>
        {
            largeBuilds,
            smallBuilds,
            skyscrappers,
            roads
        };
        buildings.AddRange(largeBuilds);
        buildings.AddRange(smallBuilds);
        buildings.AddRange(skyscrappers);
    }

    public void Update()
    {
        if (GameManager.Instance.userModel.UserId != PlayerPrefs.GetInt("LoadedSceneUserId")) return;
        SetMode();
        if (IsDestroyMode)
        {
            if (Input.GetKeyDown(KeyCode.Return) && objectToPlace != null)
            {
                bool isDeleted = false;
                switch (objectToPlace.gameObject.tag)
                {
                    case Tags.Build:
                        isDeleted = BuildingManager.Instance.DeleteBuilding(objectToPlace.gameObject.GetComponent<BuildingModel>());
                        break;
                    case Tags.Road:
                        isDeleted = RoadManager.Instance.DeleteRoad(objectToPlace.gameObject.GetComponent<RoadModel>());
                        break;
                }

                if (isDeleted)
                {
                    Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                    TakeArea(start, objectToPlace.Size);
                    Destroy(objectToPlace.gameObject);
                    ResourceManager.Instance.Initialize();
                }
                else
                {

                }
            }
            return;
        }

        if (objectToPlace == null)
        {
            return;
        }

        HandleInput();
    }
    #endregion

    #region Utils
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    public (int startIndex, int count) GetSelectedCategoryItemCount(int categoryIndex)
    {

        if (categoryIndex < 0 || categoryIndex >= buildCategories.Count)
            return (0, 0);

        int startIndex = 0;

        for (int i = 0; i < categoryIndex; i++)
        {
            startIndex += buildCategories[i].Length;
        }

        return (startIndex, buildCategories[categoryIndex].Length);
    }

    public GameObject GetSelectedItemCategory(int index)
    {
        int currentIndex = 0;

        foreach (var category in buildCategories)
        {
            if (index < currentIndex + category.Length)
            {
                return category[index - currentIndex];
            }

            currentIndex += category.Length;
        }

        return null;
    }

    public GameObject GetBuild(int id)
    {
        foreach (var build in buildings)
        {
            if (build.GetComponent<BuildingModel>().BuildingId == id)
            {
                return build;
            }
        }
        return null;
    }

    public GameObject GetRoad(int id)
    {
        foreach (var road in roads)
        {
            if (road.GetComponent<RoadModel>().RoadId == id)
            {
                return road;
            }
        }
        return null;
    }

    public int GetItemPrice(int index)
    {
        int currentIndex = 0;

        foreach (var category in buildCategories)
        {
            if (index < currentIndex + category.Length)
            {
                if (category[index - currentIndex].TryGetComponent(out BuildingModel buildingModel))
                    return buildingModel.MoneyCost;
                if (category[index - currentIndex].TryGetComponent(out RoadModel roadModel))
                    return roadModel.MoneyCost;
            }

            currentIndex += category.Length;
        }

        return 0;
    }

    private void SetParent()
    {
        switch (objectToPlace.gameObject.tag)
        {
            case Tags.Build:
                objectToPlace.transform.parent = buildParent.transform;
                break;
            case Tags.Road:
                objectToPlace.transform.parent = roadParent.transform;
                break;
        }
    }

    public void DisableAllColliders()
    {
        foreach (Collider col in FindObjectsByType<BoxCollider>(FindObjectsSortMode.None))
        {
            col.enabled = false;
        }
    }

    public void EnableAllColliders()
    {
        foreach (Collider col in FindObjectsByType<BoxCollider>(FindObjectsSortMode.None))
        {
            col.enabled = true;
        }
    }
    #endregion

    #region Building Placement
    public void InitializeWithObject(GameObject prefab, Vector3 pos)
    {
        if (!objectToPlace.IsUnityNull())
        {
            Destroy(objectToPlace.gameObject);
        }
        DisableAllColliders();
        Vector3 position = SnapCoordinateToGrid(pos);
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();
        SetParent();
    }

    public void SavedBuildTakeArea(GameObject prefab)
    {
        Quaternion rotation = prefab.transform.rotation;
        DisableAllColliders();
        GameObject obj = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        while (objectToPlace.transform.rotation.eulerAngles.y != rotation.eulerAngles.y)
        {
            objectToPlace.Rotate();
        }
        SetParent();
        objectToPlace.Place();
        Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        TakeArea(start, objectToPlace.Size);
    }

    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        Vector3Int size = objectToPlace.Size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(start.x + x, start.y + y, start.z);
                TileBase tileAtPosition = MainTilemap.GetTile(tilePosition);

                if (tileAtPosition == whiteTile)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(start.x + x, start.y + y, start.z);
                MainTilemap.SetTile(tilePosition, IsDestroyMode ? null : whiteTile);
            }
        }
        if (!IsDestroyMode)
        {
            EnableAllColliders();
            objectToPlace = null;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            objectToPlace.Rotate();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanBePlaced(objectToPlace))
            {
                bool isAdded = false;
                switch (objectToPlace.gameObject.tag)
                {
                    case Tags.Build:
                        isAdded = BuildingManager.Instance.AddBuilding(objectToPlace.gameObject.GetComponent<BuildingModel>());
                        break;
                    case Tags.Road:
                        isAdded = RoadManager.Instance.AddRoad(objectToPlace.gameObject.GetComponent<RoadModel>());
                        break;
                }


                if (isAdded)
                {
                    objectToPlace.Place();
                    Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                    TakeArea(start, objectToPlace.Size);
                    ResourceManager.Instance.Initialize();
                }
                else
                {

                }
            }
            else
            {
                Destroy(objectToPlace.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject);
        }
    }

    private void SetMode()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Delete))
        {
            objectToPlace = null;
            IsDestroyMode = !IsDestroyMode;
        }
    }

    public void SetPlaceableObject(PlaceableObject placeableObject)
    {
        objectToPlace = placeableObject;
    }
    #endregion
}
//TODO: uia kaynak menüsünü ekle (üst taraf , energy,money,food) => MAÖ
//TODO: prefablarýmýza eklenmiþ olan modellerin gelir,gidr daðýlýmlarýný dengeli bir þekilde yap => MAÖ,BÇ
//TODO: 3 tane sahne ekle 1) signin signup sahnesi , 2) main menu sahnesi ,3) oyun sahnesi =>  MAÖ