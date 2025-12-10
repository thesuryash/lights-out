using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Put this on a manager object in your scene
public class SpatialAudioToHaptics_OVR : MonoBehaviour
{
    public enum ControllerSide { Left, Right, Both }

    [Header("Audio Sources")]
    [Tooltip("If empty and Auto Find is on, we will grab all spatial AudioSources in the scene.")]
    public List<AudioSource> spatialAudioSources = new List<AudioSource>();

    [Tooltip("If true and list is empty, auto-collect all spatial AudioSources in the scene.")]
    public bool autoFindSpatialSources = true;

    [Header("Haptics Target")]
    public ControllerSide targetController = ControllerSide.Right;

    [Header("Sampling / Intensity")]
    [Tooltip("Number of samples to read from the audio buffer per update.")]
    public int sampleSize = 256;

    [Tooltip("How often to sample and send haptics (seconds).")]
    public float updateInterval = 0.02f;

    [Tooltip("Overall multiplier on the audio intensity.")]
    [Range(0f, 10f)]
    public float intensityGain = 2f;

    [Tooltip("Minimum intensity before we bother sending haptics.")]
    [Range(0f, 1f)]
    public float intensityThreshold = 0.01f;

    [Tooltip("Maximum haptics amplitude to send (0–1).")]
    [Range(0f, 1f)]
    public float maxHapticAmplitude = 0.7f;

    private float[] _sampleBuffer;

    private void Awake()
    {
        _sampleBuffer = new float[Mathf.Max(32, sampleSize)];
    }

    private void Start()
    {
        if (autoFindSpatialSources && spatialAudioSources.Count == 0)
        {
            spatialAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
    .Where(a => a != null && a.spatialBlend > 0.5f)
    .ToList();

        }

        StartCoroutine(HapticsLoop());
    }

    private IEnumerator HapticsLoop()
    {
        var wait = new WaitForSeconds(updateInterval);

        while (true)
        {
            float intensity = ComputeCombinedIntensity();

            if (intensity > intensityThreshold)
            {
                float amp = Mathf.Clamp01(intensity * intensityGain);
                amp = Mathf.Min(amp, maxHapticAmplitude);
                SendHaptics(amp);
            }
            else
            {
                // stop haptics
                SendHaptics(0f);
            }

            yield return wait;
        }
    }

    private float ComputeCombinedIntensity()
    {
        float peakRms = 0f;

        foreach (var src in spatialAudioSources)
        {
            if (src == null || !src.isPlaying)
                continue;

            src.GetOutputData(_sampleBuffer, 0);

            float sumSq = 0f;
            int len = _sampleBuffer.Length;
            for (int i = 0; i < len; i++)
            {
                float v = _sampleBuffer[i];
                sumSq += v * v;
            }

            float rms = Mathf.Sqrt(sumSq / len);
            if (rms > peakRms)
                peakRms = rms;
        }

        return peakRms;
    }

    private void SendHaptics(float amplitude)
    {
        // Meta / OVR: frequency in [0,1], amplitude in [0,1]
        float freq = (amplitude > 0f) ? 1.0f : 0f;

        if (targetController == ControllerSide.Left || targetController == ControllerSide.Both)
        {
            OVRInput.SetControllerVibration(freq, amplitude, OVRInput.Controller.LTouch);
        }

        if (targetController == ControllerSide.Right || targetController == ControllerSide.Both)
        {
            OVRInput.SetControllerVibration(freq, amplitude, OVRInput.Controller.RTouch);
        }
    }

    private void OnDisable()
    {
        // make sure to stop haptics when this is disabled/destroyed
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
    }
}
