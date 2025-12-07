using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] public Light lightSource;
    [SerializeField] public AudioClip flickeringAudio;
    [SerializeField] private float flickerDuration = 2f;
    [SerializeField] private Vector2 flickerIntervalRange = new Vector2(0.05f, 0.2f);
    [SerializeField] private Vector2 intensityRange = new Vector2(0.3f, 1.2f);

    private bool isPickedUp = false;
    private bool hasBurnedOut = false;
    private AudioSource _audioSource;

    void Awake()
    {
        if (lightSource != null)
            lightSource.enabled = false;

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Call this when the player picks up the torch
    public void OnPickedUp()
    {
        if (hasBurnedOut || isPickedUp)
            return;

        isPickedUp = true;
        StartCoroutine(LightSourceSequence());
    }

    private System.Collections.IEnumerator LightSourceSequence()
    {
        float startTime = Time.time;

        if (lightSource != null)
            lightSource.enabled = true;

        if (flickeringAudio != null && _audioSource != null)
        {
            _audioSource.clip = flickeringAudio;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        while (Time.time - startTime < flickerDuration)
        {
            if (lightSource != null)
            {
                // random intensity
                lightSource.intensity = Random.Range(intensityRange.x, intensityRange.y);
                // randomly turn off/on for harsher flicker
                lightSource.enabled = Random.value > 0.2f;
            }

            yield return new WaitForSeconds(Random.Range(flickerIntervalRange.x, flickerIntervalRange.y));
        }

        // final state: off forever
        if (lightSource != null)
        {
            lightSource.enabled = false;
            lightSource.intensity = 0f;
        }

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.loop = false;
        }

        hasBurnedOut = true;
    }
}
