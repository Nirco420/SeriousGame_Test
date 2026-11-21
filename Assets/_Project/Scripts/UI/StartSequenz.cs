using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSequenz : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject speechBubble;

    [SerializeField] private RectTransform character;
    [SerializeField] private float moveDuration = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start.onClick.AddListener(OnStartClicked);
        panel.SetActive(true);
        StartCoroutine(ShowIntro());
    }

    public void OnStartClicked()
    {
        panel.SetActive(false);
    }

    private IEnumerator ShowIntro()
    {
        speechBubble.SetActive(false);

        Canvas.ForceUpdateCanvases();

        Vector2 targetPosition = character.anchoredPosition;
        RectTransform parent = (RectTransform)character.parent;

        float distance = parent.rect.width + character.rect.width;
        Vector2 startPos = targetPosition - new Vector2(distance, 0f);

        character.anchoredPosition = startPos;

        yield return new WaitForSeconds(0.5f);

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            character.anchoredPosition = Vector2.Lerp(startPos, targetPosition, elapsedTime / moveDuration);
            yield return null;
        }

        character.anchoredPosition = targetPosition;

        yield return new WaitForSeconds(0.5f);

        speechBubble.SetActive(true);
    }

    
 
}
