using UnityEngine;

[CreateAssetMenu(fileName = "DemandData", menuName = "Scriptable Objects/DemandData")]
public class DemandData : ScriptableObject
{
    [Header("Forderung")]
    [InspectorName("Titel")]
    public string name;

    [InspectorName("Beschreibung")]
    public string description;

    [InspectorName("Geforderte Ressource")]
    public ResourceData resource;

    [InspectorName("Geforderte Menge")]
    public int quantity;

    [Header("Runden")]
    [InspectorName("Früheste Startrunde")]
    public int triggerRound;

    [InspectorName("Zeit zum Erfüllen (Runden)")]
    public int deadline;

    [InspectorName("Zusätzliche Runden bei Aufschub")]
    public int deferralPeriod;

    [Header("Barometer-Veränderung")]
    [InspectorName("Vollständig erfüllt")]
    public int changeInValuePositive;

    [InspectorName("Zur Hälfte erfüllt")]
    public int changeInValueHalf;

    [InspectorName("Frist abgelaufen")]
    public int changeInValueNegative;

    [InspectorName("Aufschub genommen")]
    public int changeInDeferalInValue;


}
