using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public int id;
    public Vector3 position;
}

[System.Serializable]
public class SaveData
{
    public List<BuildingData> buildings = new List<BuildingData>();
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    [Header("Reference to building data")]
    public ObjectsDatabaseSO buildingDatabase;

    public List<GameObject> placedBuildings = new();

    [SerializeField] private Grid grid;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private Transform raftParent;

    private GridData raftTileData = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGame();
    }

    //Call when placing a new building
    public void SaveSingleBuilding(int id, Vector3 position)
    {
        SaveData data = new SaveData();

        if (PlayerPrefs.HasKey("SaveData"))
        {
            string json = PlayerPrefs.GetString("SaveData");
            data = JsonUtility.FromJson<SaveData>(json);
        }

        data.buildings.Add(new BuildingData { id = id, position = position });

        string newJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", newJson);
        PlayerPrefs.Save();
    }

    //Call once the at the start of the game
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SaveData"))
        {
            Debug.Log("No saved data found");
            //Add basic raft tile
            placementSystem.CreateStarterRaft();
            return;
        }

        //clear old tiles off
        foreach(var obj in placedBuildings)
        {
            Destroy(obj);
        }
        placedBuildings.Clear();
        raftTileData = new GridData();

        //load data
        string json = PlayerPrefs.GetString("SaveData");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (BuildingData building in data.buildings)
        {
            GameObject prefab = buildingDatabase.objectsData[building.id - 1].Prefab;
            ObjectData objectData = buildingDatabase.objectsData[building.id - 1];

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, building.position, Quaternion.identity, raftParent);
                placedBuildings.Add(obj);

                Vector3Int gridPosition = grid.WorldToCell(building.position);

                //raftTileData.AddObjectAt(gridPosition, objectData.Size, objectData.ID, placedBuildings.Count - 1);
                placementSystem.furnitureData.AddObjectAt(gridPosition, objectData.Size, objectData.ID, placedBuildings.Count - 1);
            }
            else { Debug.LogWarning($"No prefab found with ID {building.id}"); }
        }

        Debug.Log($"Loaded {data.buildings.Count} buildings");
    }

    public SaveData GetSaveData()
    {
        string json = PlayerPrefs.GetString("SaveData", string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return new SaveData();
        }

        return JsonUtility.FromJson<SaveData>(json);
    }

    public void ClearSaveData()
    {
        foreach(GameObject obj in placedBuildings)
        {
            if (obj != null) Destroy(obj);
        }
        foreach(GameObject obj in placementSystem.placedGameObjects)
        {
            if (obj != null) Destroy(obj);
        }

        placedBuildings.Clear();
        placementSystem.furnitureData.Clear();

        PlayerPrefs.DeleteKey("SaveData");
        PlayerPrefs.DeleteKey("BattlesWon");
        PlayerPrefs.Save();
        Debug.Log("Save data cleared");

        //Add basic raft tile
        placementSystem.CreateStarterRaft();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
        if (Input.GetKeyDown(KeyCode.K)) ClearSaveData();
    }
}
