using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip startTrack;
    public List<AudioClip> musicTracks = new();
    public Vector2 delayBetweenTracksRange = new Vector2(5f, 15f);

    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Master Volume")]
    [Range(0f, 1f)] public float musicVolume = 1f;

    private AudioSource audioSource;
    private int lastTrackIndex = -1;
    private Coroutine musicCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    void Start()
    {
        if (startTrack != null || (musicTracks != null && musicTracks.Count > 0))
            musicCoroutine = StartCoroutine(PlayMusicSequence());
    }

    void Update()
    {
        // Update live volume in case slider is moved in UI
        if (audioSource != null)
            audioSource.volume = Mathf.Clamp(audioSource.volume, 0f, musicVolume);
    }

    IEnumerator PlayMusicSequence()
    {
        if (startTrack != null)
        {
            audioSource.clip = startTrack;
            audioSource.volume = 0f;
            audioSource.Play();
            yield return StartCoroutine(FadeToVolume(musicVolume));
            yield return new WaitForSeconds(startTrack.length);
        }

        while (true)
        {
            int trackIndex;
            do
            {
                trackIndex = Random.Range(0, musicTracks.Count);
            } while (musicTracks.Count > 1 && trackIndex == lastTrackIndex);

            lastTrackIndex = trackIndex;
            audioSource.clip = musicTracks[trackIndex];
            audioSource.volume = 0f;
            audioSource.Play();

            yield return StartCoroutine(FadeToVolume(musicVolume));

            yield return new WaitForSeconds(audioSource.clip.length - fadeDuration);
            yield return StartCoroutine(FadeToVolume(0f));

            float delay = Random.Range(delayBetweenTracksRange.x, delayBetweenTracksRange.y);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator FadeToVolume(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            float curveVal = fadeCurve.Evaluate(t);
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, curveVal);
            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public void StopMusic()
    {
        if (musicCoroutine != null)
            StopCoroutine(musicCoroutine);

        audioSource.Stop();
    }
}
