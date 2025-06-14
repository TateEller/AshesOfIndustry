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

    [SerializeField] private GameObject gridVisualization;

    internal GridData floorData, furnitureData;

    private List<GameObject> placedGameObjects = new();

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private bool canPlace = false;
    private bool isDragging = false;
    private Vector3 startTouchPosition;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();

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

    private IEnumerator EnablePlacementNextFrame()
    {
        yield return null;
        canPlace = true;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;  //works for tutorial. ID zero will be abke to be placed on others

        //only allow rafts to be placed next to other rafts

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

        ObjectData objData = database.objectsData[selectedObjectIndex];

        GameObject newObject = Instantiate(objData.Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        newObject.transform.SetParent(raftParent);
        placedGameObjects.Add(newObject);

        GridData selectedData = objData.ID == 0 ? floorData : furnitureData;  //works for tutorial. ID zero will be abke to be placed on others
        selectedData.AddObjectAt(gridPosition, objData.Size, objData.ID, placedGameObjects.Count - 1);

        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);

        SaveSystem.Instance.SaveSingleBuilding(objData.ID, newObject.transform.position);
    }
}
