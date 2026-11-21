using UnityEngine;

public class BuildingStartPlacement : MonoBehaviour
{
    [SerializeField] private BuildingModel building;
    [SerializeField] private GridManager grid;
    [SerializeField] private SegmentStorage segmentStorage;

#if UNITY_EDITOR
    [ContextMenu("Am Grid ausrichten")]
    private void SnapToGrid() //StartGebäude können hiermit bei der Platzierung dierekt ans GridPlatziert werden
    {
        if (Application.isPlaying || building == null || grid == null)
        {
            return;
        }

        (int x, int z) = grid.WorldToGridPosition(building.transform.position);
        Vector3 cellCenter = grid.GridToWorldCenterPosition(x, z);

        UnityEditor.Undo.RecordObject(building.transform, "Gebäude ausrichten");

        building.transform.position = cellCenter;

        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(building.transform))
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(building.transform);
        }
    }
#endif


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        building.CompletePlacement(grid, segmentStorage);

    }

}
