using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clip;
    public bool playOnStart = false;
    public bool oneShot = true;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Pitch Settings")]
    [Range(0.1f, 3f)] public float minPitch = 0.9f;
    [Range(0.1f, 3f)] public float maxPitch = 1.1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = !oneShot;
    }

    void Start()
    {
        if (playOnStart && clip != null)
            Play();
    }

    /// <summary>
    /// Play the assigned sound effect with random pitch.
    /// </summary>
    public void Play()
    {
        if (clip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        if (oneShot)
            audioSource.PlayOneShot(clip, volume);
        else
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Plays a custom clip with optional volume override and random pitch.
    /// </summary>
    public void PlayClip(AudioClip customClip, float overrideVolume = 1f)
    {
        if (customClip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(customClip, overrideVolume);
    }

    /// <summary>
    /// Stops playback.
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
    }
}
