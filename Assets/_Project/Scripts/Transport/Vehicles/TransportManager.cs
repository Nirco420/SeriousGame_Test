using System.Collections;
using UnityEngine;
using UnityEngine.Splines;


public class TransportManager : MonoBehaviour
{
    [SerializeField] private SegmentStorage segmentStorage;
    [SerializeField] private MainStorrage mainStorage;
    [SerializeField] private Transform startStoragPos;
    [SerializeField] private Transform mainStoragePos;
    [SerializeField] private GameObject cart;
    [SerializeField] private SplineAnimate splineAnimate;
    [SerializeField] private ResourceDisplayUI resourceDisplayUI;


    [SerializeField] private int transportCapacity;
    public int currentLoad {  get; private set; }
    public float moveSpeed;

    private bool isDriving = false;

    public ResourceData resourceData {  get; private set; }

    private void Update()
    {
        if(isDriving == false)
        {
            cart.SetActive(segmentStorage.assignedCarts > 0);
        }
    }

    public void StartTransport() //Startet den Transport, 
    {
        if(isDriving == true || segmentStorage.assignedCarts == 0)
        {
            return;
        }

        currentLoad = segmentStorage.RemoveResources(transportCapacity * segmentStorage.assignedCarts); //Aus dem Segmentlager soviele Ressourcen holen wie der wagen platz hat. Wenn zwei Wagen zugewiesen sind, die Doppelte Menge nehmen

        if (currentLoad == 0)
        {
            return;
        }

        resourceData = segmentStorage.storedResource; //Resource Data übergeben
        isDriving = true;
        resourceDisplayUI.SetResource(resourceData);
        resourceDisplayUI.SetQuantity(currentLoad);

        StartCoroutine(TranportLifeCycle(startStoragPos, mainStoragePos, moveSpeed)); //Coroutine für Wagen Animation starten
    }

    public IEnumerator MoveToMainStorage(float kmh) //Animation von StartPosition zum ende des Paths
    {
        resourceDisplayUI.gameObject.SetActive(true);
        splineAnimate.MaxSpeed = kmh;
        splineAnimate.Restart(false);

        while(splineAnimate.NormalizedTime < 1)
        {
            splineAnimate.NormalizedTime = Mathf.MoveTowards(splineAnimate.NormalizedTime, 1f, Time.deltaTime / splineAnimate.Duration);
            yield return null;
        }
    }

    public IEnumerator MoveBackToSegmentStorage(float kmh) //Animation von Stadt wieder zurück zum Lager
    {
        resourceDisplayUI.gameObject.SetActive(false);
        splineAnimate.MaxSpeed = kmh;

        while(splineAnimate.NormalizedTime > 0)
        {
            splineAnimate.NormalizedTime = Mathf.MoveTowards(splineAnimate.NormalizedTime, 0f, Time.deltaTime / splineAnimate.Duration);
            yield return null;
        }
    }

    public IEnumerator TranportLifeCycle(Transform startPos, Transform targetPos, float kmh) //Corountineablauf
    {
        if (isDriving == true)
        {
            yield return StartCoroutine(MoveToMainStorage(kmh));

            cart.SetActive(false);
            mainStorage.AddResources(currentLoad, resourceData);
            currentLoad = 0;

            yield return new WaitForSeconds(1f);

            cart.SetActive(true);
           
            yield return StartCoroutine(MoveBackToSegmentStorage(kmh));
            isDriving = false;
        }
    }
    }
