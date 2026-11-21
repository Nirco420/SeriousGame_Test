using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class BarometerManager : MonoBehaviour
{
    [SerializeField] private float cellSize;
    [SerializeField] private RectTransform circle;

    private Vector2 middlePosition;
    [SerializeField] private float duration = 5f;
    [SerializeField] private Image panelImage;
    [SerializeField] private AudioSource drums;
    [SerializeField, Range(0f, 1f)] private float maxDangerVolume = 0.5f;

    private bool isGameOver;
    private int currentValue;
 
    public void Awake()
    {
        middlePosition = circle.anchoredPosition;
        SetTransparency(0f);
        drums.volume = 0f;
    }

    public void UpdateBarometer()
    {
        StartCoroutine(MoveCircle());
    }

    public IEnumerator MoveCircle() //Kreis per Lerp bewegen 
    {
        float time = 0;
        Vector2 startPos = circle.anchoredPosition;
        Vector2 targetPos = middlePosition + new Vector2(currentValue * cellSize, 0f);

        float startAlpha = panelImage.color.a;
        float targetAlpha = Mathf.Abs(currentValue) * 0.209f;

        float startVolume = drums.volume;
        float targetVolume = 0f;

        if (Mathf.Abs(currentValue) >= 2)
        {
            targetVolume = maxDangerVolume;
        }


        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            circle.anchoredPosition = Vector2.Lerp(startPos, targetPos, progress);

            SetTransparency(Mathf.Lerp(startAlpha, targetAlpha, progress));

            drums.volume = Mathf.Lerp(startVolume, targetVolume, progress);

            yield return null;
        }

        circle.anchoredPosition = targetPos;
        SetTransparency(targetAlpha);
        drums.volume = targetVolume;

    }

    public void SetTransparency(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Color c = panelImage.color;
        c.a = alpha;
        panelImage.color = c;
    }

    public void ChangeValue(int amount) //Value von den Forderungen erhalten und dementsprechend die Richtung vorgeben
    {
        currentValue = Mathf.Clamp(currentValue + amount, -4, 4);
        UpdateBarometer();

        if (currentValue == -4 || currentValue == 4)
        {
            isGameOver = true;
            SceneManager.LoadScene("LoseScreen");
        }
    }
}
