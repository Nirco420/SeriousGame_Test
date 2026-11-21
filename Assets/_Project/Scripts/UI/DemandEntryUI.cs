using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemandEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI demandResource;
    [SerializeField] private TextMeshProUGUI demandTitle;
    [SerializeField] private TextMeshProUGUI quanatityText;
    [SerializeField] private TextMeshProUGUI remainingRoundsLeft;

    [SerializeField] private Button giveAll;
    [SerializeField] private Button giveHalf;
    [SerializeField] private Button aufschub;

    private DemendManger demandManager;


    public void Setup(DemandData currentDemand, int remainingRounds, DemendManger manager) //Initialisiert die Aktuelle Forderung in der Forderungsübersicht
    {
        demandManager = manager;
        demandTitle.text = currentDemand.name;
        demandResource.text = currentDemand.resource.name;
        quanatityText.text = currentDemand.quantity.ToString();
        remainingRoundsLeft.text = "Fällig in " + remainingRounds;

        giveAll.onClick.AddListener(manager.GiveAll);
        giveHalf.onClick.AddListener(manager.GiveHalf);
        aufschub.onClick.AddListener(manager.MoreTime);
    }

    private void Update()
    {
        if (demandManager == null)
        {
            return;
        }

        remainingRoundsLeft.text = "Verbleibende Runden: " + demandManager.GetRemainingRounds();
        giveAll.interactable = demandManager.CanGiveAll();
        giveHalf.interactable = demandManager.CanGiveHalf();
        aufschub.interactable = demandManager.HasActiveDemand();
    }
}
