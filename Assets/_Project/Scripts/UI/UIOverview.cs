using TMPro;
using UnityEngine;

public class UIOverview : MonoBehaviour
{
    [SerializeField] private MainStorrage mainStorrage;
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private CartManager cartManager;
    [SerializeField] private TextMeshProUGUI flachsText;
    [SerializeField] private TextMeshProUGUI wheedText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI currentRound;
    [SerializeField] private TextMeshProUGUI availabelCarts;

    public void Update()
    {
        woodText.text = mainStorrage.wood.ToString();
        wheedText.text = mainStorrage.wheed.ToString();
        flachsText.text = mainStorrage.flachs.ToString();

        currentRound.text = roundManager.currentRound.ToString();

        availabelCarts.text = cartManager.availableCarts + " / " + cartManager.TotalCarts;
    }

}
