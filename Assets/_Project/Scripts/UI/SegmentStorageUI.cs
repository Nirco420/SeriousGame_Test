using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SegmentStorageUI : MonoBehaviour
{
    [SerializeField] private GameObject segmentStorageUi;
    [SerializeField] private Image resourceImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button closeButton;
    [SerializeField] private CartManager cartManager;
    [SerializeField] private Vector3 panelOffset= new Vector3(80f,0f,0f);
    

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Camera mainCamera;


    [SerializeField] private Button cartSlot1;
    [SerializeField] private Button cartSlot2;
    [SerializeField] private Button cartSlot3;

    [SerializeField] private Sprite slotEmpty;
    [SerializeField] private Sprite slotFull;

    private SegmentStorage segmentStorage;

    public void OnEnable()
    {
        inputManager.OnTap += OpenUI;
    }

    public void Start()
    {
        closeButton.onClick.AddListener(CloseUI);
        cartSlot1.onClick.AddListener(() => ToggleCart(1));
        cartSlot2.onClick.AddListener(() => ToggleCart(2));
        cartSlot3.onClick.AddListener(() => ToggleCart(3));
    }

    public void OpenUI(Vector2 pos) //Öffnet die UI und Instanziert die Werte
    {
        SegmentStorage storage = GetStorageAtPosition(pos);

        if (storage == null)
        {
            return;
        }

        segmentStorage = storage;


        resourceImage.sprite = segmentStorage.storedResource.icon;
        amountText.text = segmentStorage.currentAmount + " / " + segmentStorage.maxCapacity;

        Vector3 position = mainCamera.WorldToScreenPoint(segmentStorage.transform.position);
        float canvasScale = segmentStorageUi.GetComponentInParent<Canvas>().rootCanvas.scaleFactor;
        segmentStorageUi.transform.position = position + new Vector3(panelOffset.x * canvasScale, panelOffset.y * canvasScale, -position.z);

        RefreshCartButtons();
        segmentStorageUi.SetActive(true);
    }

    public void CloseUI() //Schliest die UI wieder
    {
        segmentStorageUi.SetActive(false);
    }

    private SegmentStorage GetStorageAtPosition(Vector2 pos) //prüft ob ein Segmentlager getroffen wurde
    {
        Ray ray = mainCamera.ScreenPointToRay(pos);

        if(Physics.Raycast(ray, out RaycastHit hitInfo) == true)
        {
            SegmentStorage storage = hitInfo.collider.GetComponentInParent<SegmentStorage>();
            return storage;
        }
        return null;
    }

    private void ToggleCart(int slot) 
    {
        if (slot <= segmentStorage.assignedCarts)
        {
            cartManager.RemoveCart(segmentStorage);
        }
        else
        {
            cartManager.AssignCart(segmentStorage);
        }

        RefreshCartButtons();
    }

    private void RefreshCartButtons()
    {
        int count = segmentStorage.assignedCarts;

        cartSlot1.GetComponentInChildren<Image>().sprite = count >= 1 ? slotFull : slotEmpty;
        cartSlot2.GetComponentInChildren<Image>().sprite = count >= 2 ? slotFull : slotEmpty;
        cartSlot3.GetComponentInChildren<Image>().sprite = count >= 3 ? slotFull: slotEmpty;

    }

    public void OnDisable()
    {
        inputManager.OnTap -= OpenUI;
    }


}
