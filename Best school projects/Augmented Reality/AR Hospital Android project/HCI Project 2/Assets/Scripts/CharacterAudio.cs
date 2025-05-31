using UnityEngine;
using System.Collections;

public class CharacterAudio : MonoBehaviour
{
    public AudioSource audioSource; // The AudioSource component attached to your character
    public AudioClip secondClip; // The second audio clip you want to play
    public float delayBeforeSwitch = 5f; // The delay in seconds before the second audio plays

    // Start is called before the first frame update
    void Start()
    {
        // If you want to play the initial audio immediately, uncomment the line below
        // audioSource.Play();

        // Start the coroutine to change the audio after a delay
        StartCoroutine(PlaySecondAudioAfterDelay(delayBeforeSwitch));
    }

    private IEnumerator PlaySecondAudioAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Change the audio clip
        audioSource.clip = secondClip;

        // Play the new audio clip
        audioSource.Play();
    }
}
