using System.Collections;
using UnityEngine;

public class DelayedAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public float delayInSeconds = 6.0f;

    void Start()
    {
        StartCoroutine(PlayAudioAfterDelay());
    }

    IEnumerator PlayAudioAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        Debug.Log("Playing Audio Now");
        audioSource.Play();
    }
}
