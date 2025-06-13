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

        //Debug.Log($"Saved building ID {id} at position {position}");
    }

    //Call once the at the start of the game
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SaveData"))
        {
            Debug.Log("No saved data found");
            return;
        }

        string json = PlayerPrefs.GetString("SaveData");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (BuildingData building in data.buildings)
        {
            GameObject prefab = buildingDatabase.objectsData[building.id - 1].Prefab;
            if(prefab != null)
            {
                GameObject obj = Instantiate(prefab, building.position, default);
                placedBuildings.Add(obj);
            }
            else { Debug.LogWarning($"No prefab found with ID {building.id}"); }
        }

        Debug.Log($"Loaded {data.buildings.Count} buildings");
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteKey("SaveData");
        PlayerPrefs.Save();
        Debug.Log("Save data cleared");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
        if (Input.GetKeyDown(KeyCode.K)) ClearSaveData();
    }
}
