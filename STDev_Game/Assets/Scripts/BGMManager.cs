using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void PlayBGM()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }
}

