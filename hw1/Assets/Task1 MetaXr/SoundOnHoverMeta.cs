using UnityEngine;

public class SoundOnHoverMeta : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("no AudioSource found " + gameObject.name);
        else
            Debug.Log("AudioSource ready " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered " + other.gameObject.name);
        audioSource?.Play();
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited " + other.gameObject.name);
        audioSource?.Stop();
    }
}