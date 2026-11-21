using System.Collections;
using UnityEngine;

public class SegmentRotation : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float rotationStep = 120f;
    [SerializeField] private float rotationDuration = 0.4f;
    [SerializeField] private BuildingPlacement buildingPlacement;
    [SerializeField] private GameObject rotationPivot;
    [SerializeField] private BuildingUI buildingUI;

    [SerializeField] private Segment[] segments;
    private int currentSegmentIndex;


    public Segment currentSegment => segments[currentSegmentIndex];


    public bool isRotating;

    private void OnEnable()
    {
        inputManager.OnSwipe += RotateOnSwipe;
    }

    private void RotateOnSwipe (int amount) //Empfängt vom InputManager in welche Richtung gedreht wird und gibt diesen wert an den Lerp weiter
    {
        if (isRotating == true || buildingPlacement.isPlacing == true)
        {
            return;
        }
        buildingUI.BuildingFinished();
        StartCoroutine(Rotate(amount));
    }

    private IEnumerator Rotate(int amount) //Führt die Rotation durch 
    {
        isRotating = true;

        Quaternion startRotation = rotationPivot.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, -rotationStep * amount, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
           rotationPivot.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationDuration);

            yield return null;
        }

        rotationPivot.transform.rotation = targetRotation;

        rotationPivot.transform.rotation = targetRotation;
        currentSegmentIndex += amount;

        if (currentSegmentIndex >= segments.Length)
        {
            currentSegmentIndex = 0;
            Debug.Log("Aktives Segment: " + currentSegment.name);
        }
        else if(currentSegmentIndex < 0)
        {
            currentSegmentIndex = segments.Length - 1;
            Debug.Log("Aktives Segment: " + currentSegment.name);
        }
        Debug.Log("Aktives Segment: " + currentSegment.name);
        isRotating = false;
    
    }

    private void OnDisable()
    {
        inputManager.OnSwipe -= RotateOnSwipe;
    }
}
