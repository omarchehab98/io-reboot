using UnityEngine;
public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] audioTracks;
    private int index;
    private float elapsed;

    private float checkInterval = 15.0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        index = Random.Range(0, audioTracks.Length);
        elapsed = checkInterval;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= checkInterval)
        {
            if (PlayNext())
            {
                audioSource.clip = (audioTracks[index]);
                audioSource.Play();
            }
            elapsed -= checkInterval;
        }
    }

    private bool PlayNext()
    {
        if (!audioSource.isPlaying)
        {
            index = (index + 1) % audioTracks.Length;
            return true;
        }
        return false;
    }

    public void Mute(bool pause)
    {
        audioSource.mute = pause;
    }
}