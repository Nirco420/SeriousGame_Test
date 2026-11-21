using NUnit.Framework.Constraints;
using UnityEngine;

public class MainStorrage : MonoBehaviour
{
    public int wood {  get; private set; }
    public int wheed { get; private set; }
    public int flachs { get; private set; }
    public int gold {  get; private set; }

    [SerializeField] private int startWood;
    [SerializeField] private int startWheed;
    [SerializeField] private int startFlachs;
    [SerializeField] private int startGold;


    public void Start()
    {
        wood = startWood;
        wheed = startWheed;
        flachs = startFlachs;
        gold = startGold;
    }

    public void AddResources(int amount, ResourceData resource) //Prüft, welche Ressource akommt und verändert dementsprechend den Lagerwert
    {
        switch (resource.resourceType)
        {
            case ResourceData.ResourceType.WHEED:
                wheed += amount;
                break;

            case ResourceData.ResourceType.FLACHS:
                flachs += amount; 
                break;

            case ResourceData.ResourceType.WOOD:
                wood += amount;
                break;
        }
    }

    public void RemoveResource(int amount, ResourceData resource) //Resourcen wieder entnehmen 
    {
        switch (resource.resourceType)
        {
            case ResourceData.ResourceType.WHEED:
                if(amount > wheed)
                {
                    return;
                }
                wheed -= amount;
                break;
                

            case ResourceData.ResourceType.FLACHS:
                if (amount > flachs)
                {
                    return;
                }
                flachs -= amount;
                break;
                

            case ResourceData.ResourceType.WOOD:
                if (amount > wood)
                {
                    return;
                }
                wood -= amount;
                break;
        }
    }

    public bool HasResources(int amount, ResourceData resource)
    {
        switch (resource.resourceType)
        {
            case ResourceData.ResourceType.WHEED:
                if(amount > wheed)
                {
                    return false;
                }
                return true;

            case ResourceData.ResourceType.FLACHS:
                if (amount > flachs)
                {
                    return false;
                }
                return true;

            case ResourceData.ResourceType.WOOD:
                if( amount > wood)
                {
                    return false;
                }

                return true;
        }
        return false;
    }

}
