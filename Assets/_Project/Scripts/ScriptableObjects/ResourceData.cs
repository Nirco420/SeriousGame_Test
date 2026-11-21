using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/ResourceData")]
public class ResourceData : ScriptableObject
{
    public enum ResourceType
    {
        None,
        WOOD,
        WHEED,
        FLACHS
    }

    public ResourceType resourceType;
    public string name;
    public Sprite icon;
}
