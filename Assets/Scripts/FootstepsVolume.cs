using UnityEngine;
using UnityEngine.Audio;

public class FootstepVolumeBooster : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    void Start()
    {
        mixer.SetFloat("FootstepVolume", 20f);
    }

    // public void BoostFootsteps()
    // {
    //     mixer.SetFloat("FootstepVolume", 10f);
    // }
}