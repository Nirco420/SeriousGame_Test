using UnityEngine;

public class ResourceProduction : MonoBehaviour
{

    private SegmentStorage producerForStorage;
    private int productionQuantity;
    private ResourceData producedResource;
    [SerializeField] private ResourceDisplayUI displayUI;
    

    public void Setup(BuildingData buildingData, SegmentStorage segmentStorage) //Inizialisiert die aktuelle BuildingData und meldet sie als Producer im Lager
    {
        productionQuantity = buildingData.resourceQuantity;
        producedResource = buildingData.resource;
        producerForStorage = segmentStorage;
        producerForStorage.RegisterProducer(this);
        displayUI.SetResource(producedResource);
        displayUI.gameObject.SetActive(true);
        
    }
    public void ProduceResource() //Lagert pro Runde die ProduktionsMenge des Gebäudes ein 
    {
        producerForStorage.AddResources(productionQuantity);
    }

    public void OnDestroy() //Gebäude wieder abmelden wenn es nicht mehr existiert
    {
        if (producerForStorage != null)
        {
            producerForStorage.RemoveProducer(this);
        }
    }


}
