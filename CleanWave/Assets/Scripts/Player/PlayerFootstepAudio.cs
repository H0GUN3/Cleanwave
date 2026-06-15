using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [SerializeField] AudioClip footstepClip;
    [SerializeField, Range(0f, 1f)] float volume = 0.35f;
    [SerializeField] float movementThreshold = 0.05f;
    [SerializeField] float referenceMoveSpeed = 3f;
    [SerializeField] float minPitch = 0.85f;
    [SerializeField] float maxPitch = 1.45f;

    PlayerMovement movement;
    AudioSource audioSource;

    public void Configure(AudioClip clip)
    {
        footstepClip = clip;
        ApplyClip();
    }

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        ApplyClip();
    }

    void Update()
    {
        if (movement == null || footstepClip == null)
        {
            StopIfPlaying();
            return;
        }

        bool isMoving = movement.MoveInput.sqrMagnitude > movementThreshold * movementThreshold;
        if (isMoving)
        {
            audioSource.pitch = GetSpeedPitch();

            if (audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Unloaded)
                audioSource.clip.LoadAudioData();

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            StopIfPlaying();
        }
    }

    void ApplyClip()
    {
        if (audioSource == null)
            return;

        audioSource.clip = footstepClip;
    }

    float GetSpeedPitch()
    {
        if (referenceMoveSpeed <= 0f)
            return 1f;

        return Mathf.Clamp(movement.EffectiveMoveSpeed / referenceMoveSpeed, minPitch, maxPitch);
    }

    void StopIfPlaying()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}
