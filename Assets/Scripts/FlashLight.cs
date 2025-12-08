using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class Torch : MonoBehaviour
{
    [SerializeField] private Light lightSource;
    [SerializeField] private AudioClip flickeringAudio;
    [SerializeField] private float flickerDuration = 2f;
    [SerializeField] private Vector2 flickerIntervalRange = new Vector2(0.05f, 0.2f);
    [SerializeField] private Vector2 intensityRange = new Vector2(0.3f, 1.2f);

    private bool hasBurnedOut = false;
    private AudioSource _audioSource;
    private HapticImpulsePlayer _haptics;  

    void Awake()
    {
        if (lightSource != null)
            lightSource.enabled = false;

        _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    // called when grabbed; we pass in the hand's HapticImpulsePlayer
    public void OnPickedUp(HapticImpulsePlayer haptics)
    {
        if (hasBurnedOut) return;

        _haptics = haptics;
        StartCoroutine(FlickerSequence());
    }

    private System.Collections.IEnumerator FlickerSequence()
    {
        float startTime = Time.time;

        if (lightSource != null)
            lightSource.enabled = true;

        if (flickeringAudio != null)
        {
            _audioSource.clip = flickeringAudio;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        while (Time.time - startTime < flickerDuration)
        {
            // visual flicker
            if (lightSource != null)
            {
                lightSource.intensity = Random.Range(intensityRange.x, intensityRange.y);
                lightSource.enabled = Random.value > 0.2f;
            }

            // haptic flicker via HapticImpulsePlayer
            if (_haptics != null)
            {
                float amp = Random.Range(0.2f, 0.9f);
                float dur = Random.Range(0.04f, 0.12f);
                _haptics.SendHapticImpulse(amp, dur);
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
