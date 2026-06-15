using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuBGM : MonoBehaviour
{
    [SerializeField] AudioClip mainTheme;
    [SerializeField, Range(0f, 1f)] float volume = 0.6f;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = volume;

        if (mainTheme != null)
            audioSource.clip = mainTheme;
    }

    void Start()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
            audioSource.Play();
    }
}
