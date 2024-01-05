using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("AudioController.Awake: Multiple AudioController objects in scene.");
            gameObject.SetActive(false);
            return;
        }

        Instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    public static void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        Instance._audioSource.Stop(); // TODO: This causes clicks
        Instance._audioSource.PlayOneShot(clip);
    }
}
