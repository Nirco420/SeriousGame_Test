using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemendUI : MonoBehaviour
{
    [SerializeField] private DemendManger demendManger;

    [SerializeField] private GameObject demandPanel;
    [SerializeField] private TextMeshProUGUI headlineText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI resourceName;
    [SerializeField] private TextMeshProUGUI demandQuantity;
    [SerializeField] private TextMeshProUGUI demandDeadline;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button openDemandEntry;

    [SerializeField] private DemandEntryUI demandEntryUI;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject redInfo;
    [SerializeField] private GameObject demandOverview;


    private DemandData demandData;
    private ResourceData resourceData;
    private DemandEntryUI demandEntry;
    private bool currentState = false;
    


    private void OnEnable() //Event Abonnieren
    {
        demendManger.OnDemandDisplayRequested += DemandPopUP;
        demendManger.OnDemandCompleted += RemoveDemandEntry;
    }

    public void Start() //Listener für die Buttons
    {
        closeButton.onClick.AddListener(ClosePopUp);
        openDemandEntry.onClick.AddListener(OpenDemandOverview);

        demandOverview.SetActive(currentState);
        
    }

    public void DemandPopUP(DemandData currentDemand, int currentRound) //PopUP, welches aufgerufen wird und über den DemendManager getriggerd wird. 
    {
        demandData = currentDemand;
        demandPanel.SetActive(true);
        resourceData = currentDemand.resource;
        int currentdemandQuantity = currentDemand.quantity;
        int currentdemandDeadline = currentDemand.deadline;


        headlineText.text = currentDemand.name;
        descriptionText.text = currentDemand.description;
        resourceName.text = resourceData.name;
        demandQuantity.text = "Menge: " + currentdemandQuantity.ToString();
        demandDeadline.text = "In Runden: " + currentdemandDeadline.ToString();

    }

    public void ClosePopUp() //Beim schliesen des PopUps, Forderung in die Übersicht und Popup wieder ausschalten. 
    {
        demandPanel.SetActive(false);
        if (demandEntry == null)
        {
            
            demandEntry = Instantiate(demandEntryUI, content);
            demandEntry.Setup(demandData, demandData.deadline, demendManger);
            redInfo.SetActive(true);

        }

    }

    public void OpenDemandOverview()
    {
        currentState = !currentState;

        demandOverview.SetActive(currentState);

    }

    private void RemoveDemandEntry()
    {
        demandPanel.SetActive(false);

        if (demandEntry != null)
        {
            Destroy(demandEntry.gameObject);
            demandEntry = null;
        }

        redInfo.SetActive(false);
    }

    private void OnDisable()
    {
        demendManger.OnDemandDisplayRequested -= DemandPopUP;
        demendManger.OnDemandCompleted -= RemoveDemandEntry;
    }
}
