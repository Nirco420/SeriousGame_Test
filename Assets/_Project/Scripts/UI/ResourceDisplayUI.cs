using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplayUI : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI quantity;

    public void SetResource(ResourceData resource) //Icon aus ResourceData holen 
    {
        resourceIcon.sprite = resource.icon; 
    }

    public void SetQuantity(int amount) //Menge setzen
    {
       quantity.text = amount.ToString();
    }
}
