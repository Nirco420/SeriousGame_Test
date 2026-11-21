using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public enum BuildingType
    {
        None,
        STORAGE,
        PRODUKTION,
        RESOURECES
    }

    public string description;
    public ResourceData costResource;
    public int cost;

    public BuildingType type;
    public ResourceData resource;

    public int resourceQuantity;
    public int maxStorage;
    public Sprite uiIcon;


}
