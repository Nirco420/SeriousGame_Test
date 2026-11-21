using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize = 1f;

    [SerializeField] private MeshCollider meshCollider;


    private BuildingGridCell[,] grid;
    public bool IsReady { get; private set; }

    private void Awake() //Grid erzeugen und je nach größe der Widht und Height neue Felder erzeugen und ins 2D-Array aufnehmen
    {
        if (width <= 0 || height <= 0 || cellSize <= 0f || meshCollider == null)
        {
            return;
        }

        grid = new BuildingGridCell[width, height];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                grid[x, z] = new BuildingGridCell();
                grid[x, z].SetBuildable(IsCellOnCollider(x, z));
            }
        }
        IsReady = true;
    }

    public Vector3 GridToWorldPosition(int x, int z) //Berechnet die Grid koordinaten in Worldkoordinaten um 
    {
        return transform.TransformPoint(new Vector3(x * cellSize, 0f , z * cellSize));
    }

    public (int x, int z) WorldToGridPosition(Vector3 worldPos) //Berechnet die Worldposition in GridPositionen um 
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int z = Mathf.FloorToInt(localPos.z / cellSize);
        return (x,z);
    }

    public bool TrySetBuilding(BuildingModel building, List<Vector3> allBuildingPositions) //Trägt das Gebäude auf die Zellen ein, wenn diese frei sind
    {
        if (building == null || CanBuild(allBuildingPositions) == false)
        {
            return false;
        }

        foreach (Vector3 buildingPos in allBuildingPositions)
        {
            (int x, int z) = WorldToGridPosition(buildingPos);
            grid[x, z].SetBuilding(building);
        }
        return true;
    }

    public bool CanBuild(List<Vector3> allBuildingPositions) //Geht alle ZellPositionen durch und prüft, ob sie im Grid liegen, ob sie frei sind und ob sie auf dem meshCollider liegen
    {

        if (IsReady == false || allBuildingPositions == null || allBuildingPositions.Count == 0)
        {
            return false;
        }
        foreach (Vector3 buildingPos in allBuildingPositions)
        {
            (int x, int z) = WorldToGridPosition(buildingPos);
            
            if(x < 0 || x >= width || z < 0 || z >= height)
            
            {
                return false;
            }

            if (!grid[x,z].IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

    public void OnDrawGizmos() //Per Gizmos das Grid zeichnen
    {
        if (cellSize <= 0f || width <= 0 || height <= 0 || meshCollider == null)
        {
            return;
        }

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (IsCellOnCollider(x, z))
                {
                    Vector3 center = new Vector3((x + 0.5f) * cellSize, 0f, (z + 0.5f) * cellSize);

                    Gizmos.DrawWireCube(center, new Vector3(cellSize, 0f, cellSize));
                }
            }
        }

        Gizmos.matrix = previousMatrix;
    }

    public bool IsCellOnCollider(int x, int z) // prüft per Ray jede Ecke einer Zelle. Wenn der Ray auf den MeshCollider trifft, dann return true sonst false
    {
        float left = x * cellSize + 0.3f;
        float right = (x + 1) * cellSize - 0.3f;
        float top = z * cellSize + 0.3f;
        float bottom = (z + 1) * cellSize - 0.3f;

        Ray leftUpperRay = new Ray(transform.TransformPoint(new Vector3(left, 10f, top)), -transform.up);

        Ray rightUpperRay = new Ray(transform.TransformPoint(new Vector3(right, 10f, top)), -transform.up);

        Ray leftBottomRay = new Ray(transform.TransformPoint(new Vector3(left, 10f, bottom)), -transform.up);

        Ray rightBottomRay = new Ray(transform.TransformPoint(new Vector3(right, 10f, bottom)), -transform.up);

        return meshCollider.Raycast(leftUpperRay, out _, Mathf.Infinity)
            && meshCollider.Raycast(rightUpperRay, out _, Mathf.Infinity)
            && meshCollider.Raycast(leftBottomRay, out _, Mathf.Infinity)
            && meshCollider.Raycast(rightBottomRay, out _, Mathf.Infinity);
    }

    public Vector3 GridToWorldCenterPosition(int x, int z) //Hilfsmethode für das GridSnapping um den Mittelpunkt für jede Zelle bestimmen zu können.
    {
        return transform.TransformPoint(new Vector3((x + 0.5f) * cellSize, 0f, (z + 0.5f) * cellSize));
    }
}

public class BuildingGridCell
{

    private BuildingModel building;

    private bool isBuildable;

    public void SetBuilding(BuildingModel building) //Setzt das aktuell zugewiesene BuildingModell als aktuelles Modell
    {
        this.building = building;
    }

    public void SetBuildable(bool isBuild) //Wird im GridManager aufgerufen um isBuildable zu setzen
    {
        isBuildable = isBuild;
    }

    public bool GetBuildable() //gibt zurück ob eine Zelle bebaubar ist oder nicht
    {
        return isBuildable;
    }

    public bool IsEmpty() // prüft ob eine Zelle bebaubar ist, also auf dem MeshCollider liegt und ob sie leer ist also kein Gebäude drauf ist
    {
        if(isBuildable == true && building == null)
        {
            return true;
        }
        return false;
    }
}
