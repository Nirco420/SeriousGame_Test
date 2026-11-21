using System.Collections.Generic;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] private GameObject wrapper;
    [SerializeField] private BuildingData buildingData;

    public BuildingData Data
    {
        get
        {
            return buildingData;
        }
    }

    public float rotation;
    private BuildingShapeUnit[] shapeUnits;

    private Renderer[] renderers;

    private Material[][] originalMaterials;
    private Material previewMaterial;
    private ResourceProduction production;
    private bool isPreview;
    private bool isPlaced;

    public void Awake() //Alle wichtigen Compoonents suchen und zuweisen
    {
        shapeUnits = GetComponentsInChildren<BuildingShapeUnit>();
        renderers = GetComponentsInChildren<Renderer>(true);
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        production = GetComponent<ResourceProduction>();

       //Erst nach Registrierung starten
       
        if (production != null)
        {
            production.enabled = false;
        }
    }

    public void BeginPreview() //Kennzeichnet das Preview als Spilervorschau
    {
        if (!isPlaced)
        {
            isPreview = true;
        }
    }

    public void SetPreviewMaterial(Material material) //Setzt Grün oder Rot für die Vorschau. Für jeden Renderer wird ein Array erstellt mit der ursprünglichen Anzahl an Materials. Dann wird jedes Material im Array mit dem Vorschau Material ersetzt. 
    {
        if (isPreview == false || previewMaterial == material)
        {
            return;
        }

        previewMaterial = material;
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = new Material[originalMaterials[i].Length];
            for (int slot = 0; slot < materials.Length; slot++)
            {
                materials[slot] = material;
            }

            // Nur die Zuweisungen wechseln; die Material-Assets bleiben unveraendert.
            renderers[i].sharedMaterials = materials;
        }
    }

    public bool CompletePlacement(GridManager targetGrid, SegmentStorage segmentStorage)//Schließt die Gebäudeplatzierung ab
    {
        // Erst die gesamte Grundfläche prüfen und registrieren, dann Gameplay starten.
        List<Vector3> positions = GetAllBuildingPositions();
        if (targetGrid.TrySetBuilding(this, positions) == false)
        {
            return false;
        }

        isPlaced = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterials = originalMaterials[i];
        }

        isPreview = false;
        previewMaterial = null;

        if (production != null)
        {
            production.Setup(buildingData, segmentStorage);
            production.enabled = true;
        }

        return true;
    }

    public void Rotate(float rotationStep) //Führt rotation durch
    {
        wrapper.transform.Rotate(new Vector3(0, rotationStep, 0));
    }

    public List<Vector3> GetAllBuildingPositions() //sammelt die Weltposiotionen der ShapeUnits
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (BuildingShapeUnit unit in shapeUnits)
        {
            positions.Add(unit.transform.position);
        }
        return positions;
    }
}
