using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private Material positive;
    [SerializeField] private Material negative;
    [SerializeField] private MainStorrage mainStorrage;
    [SerializeField] private SegmentRotation segmentRotation;

    [SerializeField] private GameObject placementPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button rotateButton; 

    private BuildingModel currentPreview;
    private ResourceData buildingResource;
    private bool isDragging;
    private GridManager grid;
    private MeshCollider meshCollider;
    private SegmentStorage segmentStorage;

    public bool isPlacing => currentPreview != null;


    public void Start()
    {
        confirmButton.onClick.AddListener(PlaceBuilding);
        cancelButton.onClick.AddListener(CanclePlacement);
        rotateButton.onClick.AddListener(RotatePreview);
    }

    private void OnEnable() //BuildingPlacement als Empfänger hinzufügen
    {
        inputManager.OnFingerMoved += OnTouchMoved;
        inputManager.OnFingerBegan += OnTouchBegan;
        inputManager.OnFingerEnded += OnTouchEnded;
        buildingUI.OnBuildingSelected += GetSelectedBuilding;
    }

    private void Update() //Pro Frame wird geprüft, ob eine Preview existiert, ob sie mind eine ShepUnit hat und alle Zellen darunter bebaubar sind
    {
        if (currentPreview == null)
        {
            return;
        }

        //UI an der Gebäudeposition platzieren
        Vector3 position = mainCamera.WorldToScreenPoint(currentPreview.transform.position);
        placementPanel.transform.position = new Vector3(position.x, position.y, 0f);

        bool canBuild = grid.CanBuild(currentPreview.GetAllBuildingPositions()); //Bool speichern um Buttons darüber zu steuern
        
        BuildingData data = currentPreview.Data;

        canBuild = canBuild && mainStorrage.HasResources(data.cost, data.costResource);

        confirmButton.interactable = canBuild;

        if (canBuild == true)
        {
            currentPreview.SetPreviewMaterial(positive);
        }
        else
        {
            currentPreview.SetPreviewMaterial(negative);
        }
    }

    private void CanclePlacement() //Bricht den Bau vorgang ab und schliest die UI
    {
        ClearPreview();
        buildingUI.BuildingFinished();
    }

    private void RotatePreview() //Dreht das Parent-Objekt 
    {
        if(currentPreview == null)
        {
            return;
        }

        currentPreview.Rotate(90f); 
    }

    public void OnTouchBegan(Vector2 pos) //Erzeugt einen Ray von der Camera zum Objekt. Wenn einer der Objektcollider getroffen wurde, dann treffer position übergeben
    {
        isDragging = false;

        if (currentPreview == null)
        {
            return;
        }

        //Da collider hinter Button geprüft werden und die Platzierung dann nicht gescheid Funktioniert diese Prüfung. 
        if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)confirmButton.transform, pos, null) || RectTransformUtility.RectangleContainsScreenPoint((RectTransform)cancelButton.transform, pos, null) ||RectTransformUtility.RectangleContainsScreenPoint((RectTransform)rotateButton.transform, pos, null))
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(pos);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(currentPreview.transform))
            {
                isDragging = true;
                break;
            }
        }
    }

    public void OnTouchMoved(Vector2 pos) //Ein Ray von der camera zur Fingerposition erzeugen, wenn der MeshCollider der Baufläche getroffen wird, den Trefferpunkt in GridKoordinaten umrechen. Das Objekt dann auf diese GridKoordianten setzen
    {
        if (currentPreview == null || isDragging == false)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(pos);
        if (meshCollider.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            (int x, int z) = grid.WorldToGridPosition(hitInfo.point);
            currentPreview.transform.position = grid.GridToWorldCenterPosition(x, z);
        }
    }

    public void OnTouchEnded(Vector2 pos) //Beendet den ToucInput
    {
        isDragging = false;
    }

    public void GetSelectedBuilding(BuildingModel buildingPrefab)//Erzeugt das Gebäude mit den richtigen Gebäudedaten, erstmal nur als Vorschau
    {
        if (segmentRotation.isRotating == true)
        {
            return;
        }

        ClearPreview();
        Segment segment = segmentRotation.currentSegment;

        grid = segment.grid;
        meshCollider = segment.surface;
        segmentStorage = segment.segmentStorage;

        Vector3 spawnPosition = grid.GridToWorldCenterPosition(8, 9);

        currentPreview = Instantiate(buildingPrefab, spawnPosition, grid.transform.rotation);

        currentPreview.transform.SetParent(segment.buildingParents, true);
        currentPreview.BeginPreview();
        placementPanel.SetActive(true);
    }

    public void PlaceBuilding()//Hier wird das gebäude platziert, es werden nochmal alle Zellen geprüft und wenn diese frei sind, durch das Building belegt. Alle werte werden dann wieder zurückgesetzt
    {
        if(currentPreview == null)
        {
            return;
        }

        BuildingData data = currentPreview.Data;


        if (mainStorrage.HasResources(data.cost, data.costResource) == false)
        {
            return;

        }

        // Das Modell registriert sich und verwendet seine eigenen Gebäudedaten.
        if (!currentPreview.CompletePlacement(grid, segmentStorage))
        {
            return;
        }

        mainStorrage.RemoveResource(data.cost, data.costResource);

        // Dieselbe Instanz bleibt bestehen.
        currentPreview = null;
        isDragging = false;
        buildingUI.BuildingFinished();
        placementPanel.SetActive(false);
    }

    private void ClearPreview() //Entfernt die Vorschau bzw. das Modell bei einem Auswahlswechsel
    {
        if (currentPreview != null)
        {
            // Sofort ausblenden, da Destroy erst am Frame Ende ausgeführt wird.
            currentPreview.gameObject.SetActive(false);
            Destroy(currentPreview.gameObject);
        }

        currentPreview = null;
        isDragging = false;

        if (placementPanel != null)
        {
            placementPanel.SetActive(false);
        }
    }

    private void OnDisable() //Events wieder abmelden und alle werte resetten
    {
        inputManager.OnFingerMoved -= OnTouchMoved;
        inputManager.OnFingerBegan -= OnTouchBegan;
        inputManager.OnFingerEnded -= OnTouchEnded;
        buildingUI.OnBuildingSelected -= GetSelectedBuilding;
        ClearPreview();
    }
}
