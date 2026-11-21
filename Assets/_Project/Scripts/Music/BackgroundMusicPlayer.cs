using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    private static BackgroundMusicPlayer instance;

    private void Awake()
    {
        // Beim Zurückkehren ins Startmenü keine zweite Musik starten.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
