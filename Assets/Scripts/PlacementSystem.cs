using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private Transform raftParent;

    [SerializeField] private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField] private ResourcesSO resources;

    [SerializeField] private GameObject gridVisualization;

    internal GridData furnitureData;

    internal List<GameObject> placedGameObjects = new();

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private bool canPlace = false;
    private bool isDragging = false;
    private Vector3 startTouchPosition;

    private void Awake()
    {
        if (furnitureData == null)
            furnitureData = new();
    }
    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        Camera.main.GetComponent<MobileCameraControls>().canPan = false;

        if (!CanAffordBuild(ID)) return;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        canPlace = false;

        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);

        inputManager.OnStartPlacement += HandleStartPlacement;
        inputManager.OnDragPlacement += HandleDragPlacement;
        inputManager.OnEndPlacement += HandleEndPlacement;

        StartCoroutine(EnablePlacementNextFrame());
    }

    private bool CanAffordBuild(int ID)
    {
        //check item cost
        switch (ID)
        {
            case (1):   //basic - 2 wood
                if (resources.wood >= 2)
                    return true;
                break;
            case (2):   //fishing - 2 wood, 1 fish
                if(resources.wood >= 2 && resources.fish >= 1)
                    return true;
                break;
            case (3):   //storage - 3 wood, 1 metal
                if (resources.wood >= 3 && resources.metal >= 1)
                    return true;
                break;
            case (4):   //water - 2 wood, 2 metal
                if (resources.wood >= 2 && resources.metal >= 2)
                    return true;
                break;
        }
        Debug.Log("Cant afford build");
        return false;
    }
    private bool BuyBuild(int ID)
    {
        //check item cost
        switch (ID)
        {
            case (1):   //basic - 2 wood
                if (resources.wood >= 2)
                {
                    resources.wood -= 2;
                    return true;
                }
                break;
            case (2):   //fishing - 2 wood, 1 fish
                if (resources.wood >= 2 && resources.fish >= 1)
                {
                    resources.wood -= 2;
                    resources.fish -= 1;
                    return true;
                }
                break;
            case (3):   //storage - 3 wood, 1 metal
                if (resources.wood >= 3 && resources.metal >= 1)
                {
                    resources.wood -= 3;
                    resources.metal -= 1;
                    return true;
                }
                break;
            case (4):   //water - 2 wood, 2 metal
                if (resources.wood >= 2 && resources.metal >= 2)
                {
                    resources.wood -= 2;
                    resources.metal -= 2;
                    return true;
                }
                break;
        }
        Debug.Log("Cant afford build");
        return false;
    }

    private IEnumerator EnablePlacementNextFrame()
    {
        yield return null;
        canPlace = true;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = furnitureData;  //works for tutorial. ID zero will be abke to be placed on others

        //only allow rafts to be placed next to other rafts
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int (1,0,0),
            new Vector3Int (-1,0,0),
            new Vector3Int (0,0,1),
            new Vector3Int (0,0,-1),
        };

        bool adjacentRaft = false;
        foreach(Vector3Int dir in directions)
        {
            if (!furnitureData.CanPlaceObjectAt(gridPosition + dir, database.objectsData[selectedObjectIndex].Size))
            {
                adjacentRaft = true;
                break;
            }
        }
        if (!adjacentRaft) 
            return false;   

        //check actual placement spot
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    private void StopPlacement()
    {
        selectedObjectIndex = -1;

        gridVisualization.SetActive(false);
        preview.StopShowingPreview();

        inputManager.OnStartPlacement -= HandleStartPlacement;
        inputManager.OnDragPlacement -= HandleDragPlacement;
        inputManager.OnEndPlacement -= HandleEndPlacement;

        lastDetectedPosition = Vector3Int.zero;
        Camera.main.GetComponent<MobileCameraControls>().canPan = true;
    }

    private void HandleStartPlacement(Vector3 worldPos)
    {
        if (!canPlace) return;

        isDragging = false;
        startTouchPosition = worldPos;

        UpdatePreviewPosition(worldPos);

        //Display preview at center
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("PlacementLayer")))
        {
            Vector3Int gridPos = grid.WorldToCell(hit.point);
            preview.UpdatePosition(grid.CellToWorld(gridPos), CheckPlacementValidity(gridPos, selectedObjectIndex));
        }
    }
    private void HandleDragPlacement(Vector3 worldPos)
    {
        if (!canPlace) return;

        //if finger moveed far enough, start dragging
        if (Vector3.Distance(worldPos, startTouchPosition) > 0.1f)
        {
            isDragging = true;
        }
        UpdatePreviewPosition(worldPos);
    }
    private void HandleEndPlacement(Vector3 worldPos)
    {
        if (!canPlace) return;

        if (!isDragging)
        {
            //tap to place (no dragging)
            PlaceBuildingAt(startTouchPosition);
        }
        else
        {
            //drag to place at release
            PlaceBuildingAt(worldPos);
        }

        StopPlacement();
    }
    private void UpdatePreviewPosition(Vector3 worldPos)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPos);
        if (lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            mouseIndicator.transform.position = worldPos;
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
    }
    private void PlaceBuildingAt(Vector3 worldPos)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPos);
        if (!CheckPlacementValidity(gridPosition, selectedObjectIndex)) return;
        if (!BuyBuild(database.objectsData[selectedObjectIndex].ID))
        {
            Debug.Log("Can't Buy Build");
            return;
        }

        ObjectData objData = database.objectsData[selectedObjectIndex];

        GameObject newObject = Instantiate(objData.Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        newObject.transform.SetParent(raftParent);
        placedGameObjects.Add(newObject);

        GridData selectedData = furnitureData;  //works for tutorial. ID zero will be abke to be placed on others
        selectedData.AddObjectAt(gridPosition, objData.Size, objData.ID, placedGameObjects.Count - 1);

        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);

        SaveSystem.Instance.SaveSingleBuilding(objData.ID, newObject.transform.position);
    }

    public void CreateStarterRaft()
    {
        //Start placement
        int ID = 1; //basic tile ID
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        //place at
        Vector3 raftOrigin = new Vector3(0, 0, 0);
        Vector3Int gridPosition = grid.WorldToCell(raftOrigin);

        ObjectData objData = database.objectsData[selectedObjectIndex];

        GameObject newObject = Instantiate(objData.Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        newObject.transform.SetParent(raftParent);
        placedGameObjects.Add(newObject);

        if (furnitureData == null)
        {
            furnitureData = new();
        }
        GridData selectedData = furnitureData;  //works for tutorial. ID zero will be abke to be placed on others
        selectedData.AddObjectAt(gridPosition, objData.Size, objData.ID, placedGameObjects.Count - 1);

        SaveSystem.Instance.SaveSingleBuilding(objData.ID, newObject.transform.position);

        //stop
        StopPlacement();
    }
}
