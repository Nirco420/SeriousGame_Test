using System.Collections.Generic;
using UnityEngine;

public class SegmentStorage : MonoBehaviour
{
    private BuildingData buildingData;
    public ResourceData storedResource {  get; private set; }
    private List<ResourceProduction> resourceBuildings = new List<ResourceProduction>();

    public int currentAmount {  get; private set; }
    public int maxCapacity { get; private set; }
    public int assignedCarts { get; private set; }

    public void Awake()
    {
        buildingData = GetComponent<BuildingModel>().Data;
        storedResource = buildingData.resource;
        maxCapacity = buildingData.maxStorage;
        currentAmount = 0;
    }

    public void AddResources(int amount) //Berechnet wieviel Speicher im Gebäude noch frei ist und rechnet dann denn niedrigsten wert drauf
    {
        if (amount  < 0)
        {
            return;
        }
        int freeStorage = maxCapacity - currentAmount;
        amount = Mathf.Min(freeStorage, amount);
        currentAmount += amount;

    }

    public int RemoveResources(int amount) //Schaut wieviele Ressourcen entnommen werden können und rechnet dann den niedrigeren raus
    {
        if (amount < 0)
        {
            return 0;
        }

        amount = Mathf.Min(currentAmount, amount);
        currentAmount -= amount;

        return amount;

    }

    public void RegisterProducer(ResourceProduction productionBuilding) //Prüfen ob Produzent schon in der Liste ist wenn nicht, dann hinzufügen
    {
        if(productionBuilding == null)
        {
            return; 
        }

        if (resourceBuildings.Contains(productionBuilding) == true)
        {
            return;
        }

        resourceBuildings.Add(productionBuilding);
    }

    public void RemoveProducer(ResourceProduction productionBuilding) //Producer wieder aus liste entfernen
    {
        resourceBuildings.Remove(productionBuilding);
    }

    public void ProducePerRound() //Durch Producer Liste itterrieren und jeden Produzieren lassen.
    {
        foreach (ResourceProduction producer in resourceBuildings)
        {
            producer.ProduceResource();
            Debug.Log("Resource wurde hinzugefügt: " + currentAmount + "/" + maxCapacity);
        }
    }
    public bool AssignCart() //Cart zuweisen
    {
        if (assignedCarts >= 3)
        {
            return false;
        }

        assignedCarts++;
        return true;
    }

    public bool RemoveCarts() //Cart wieder entfernen
    {
        if (assignedCarts <= 0)
        {
            return false;
        }

        assignedCarts--;
        return true;
    }
}
