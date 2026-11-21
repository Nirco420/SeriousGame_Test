using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public event Action<BuildingModel> OnBuildingSelected;
    public event Action<bool> OnNextRoundPressed;

    [SerializeField] private Button buildingButton;
    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private SegmentRotation segmentRotation;

    [SerializeField] private Button building1;
    [SerializeField] private Button building2;
    [SerializeField] private Button building3;
    [SerializeField] private Button nextRound;


    [SerializeField] private BuildingModel building1x1;
    [SerializeField] private BuildingModel building2x2;
    [SerializeField] private BuildingModel building3x3;

    [SerializeField] private TextMeshProUGUI text1x1Building;
    [SerializeField] private TextMeshProUGUI text2x2Building;
    [SerializeField] private TextMeshProUGUI text3x3Building;

    [SerializeField] private Image resourceImage1x1;
    [SerializeField] private Image resourceImage2x2;
    [SerializeField] private Image resourceImage3x3;


    [SerializeField] private BuildingModel wheedBuilding;
    [SerializeField] private BuildingModel woodBuilding;
    [SerializeField] private BuildingModel flachsBuilding;

    public void Start() //Listener für die Buttons erstellen
    {
        buildingButton.onClick.AddListener(OnBuildingButtonClicked);

        building1.onClick.AddListener(OnBuilding1Clicked);
        building2.onClick.AddListener(OnBuilding2Clicked);
        building3.onClick.AddListener(OnBuilding3Clicked);

        nextRound.onClick.AddListener(OnEndRoundWasClicked);

    }

    public void OnBuildingButtonClicked() //RundesPanel wo die Gebäude Buttons drinne sind sichtbar schalten
    {
        if(segmentRotation.isRotating == true)
        {
            return;
        }

        RefreshUiButtons();
        buildingPanel.SetActive(true);
    }

    public void BuildingFinished() //Methode das Panel wir aus zu schalten
    {
        buildingPanel.SetActive(false);
    }

    public void OnBuilding1Clicked() //Event wenn das einer Gebäude gewählt wurde
    {
        OnBuildingSelected?.Invoke(building1x1);
    }

    public void OnBuilding2Clicked()//Event wenn das zweir Gebäude gewählt wurde
    {
        OnBuildingSelected?.Invoke(building2x2);
    }

    public void OnBuilding3Clicked()//Event wenn das dreier Gebäude gewählt wurde
    {
        OnBuildingSelected?.Invoke(building3x3);
    }

    public void OnEndRoundWasClicked() //Event Triggern, dass die Runde beendet worden ist
    {
        OnNextRoundPressed?.Invoke(true);
    }

    public void ShowCostBuilding(BuildingData buildingData, TextMeshProUGUI text, Image image) 
    { 
        BuildingData data = buildingData;

        text.text = "Kosten" + buildingData.cost;
        image.sprite = data.costResource.icon;
    }

    public void RefreshUiButtons() //Building in der UI anpassen, je nach Resource des Segments
    {
        ResourceData resource = segmentRotation.currentSegment.allowedResources;

        if (resource == wheedBuilding.Data.resource)
        {
            building2x2 = wheedBuilding;
        }
        else if (resource == woodBuilding.Data.resource)
        {
            building2x2 = woodBuilding;
        }
        else if (resource == flachsBuilding.Data.resource)
        {
            building2x2 = flachsBuilding;
        }

        building2.gameObject.SetActive(building2x2.Data.resource == resource);

        ShowCostBuilding(building1x1.Data, text1x1Building, resourceImage1x1);
        ShowCostBuilding(building2x2.Data, text2x2Building, resourceImage2x2);
        ShowCostBuilding(building3x3.Data, text3x3Building, resourceImage3x3);
    }
}
