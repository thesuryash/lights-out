// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using UnityEngine;
using Oculus.Haptics;
using System;

// This scene is a minimal integration example, meant to run on device (f.e. Meta Quest 2, Meta Quest Pro).
// It showcases how events, like button presses, can be hooked up to haptic feedback; and how we can use other input, like
// a controller's thumbstick movements, to modulate haptic effects.
// We gain access to the Haptics SDK's features through an API by importing Oculus.Haptics (see above).
public class HapticManager : MonoBehaviour
{
    // The haptic clips are assignable in the Unity editor.
    // For this example, we are using the two demo clips found in Assets/Haptics.
    // Haptic clips can be designed in Haptics Studio (https://developer.oculus.com/experimental/exp-haptics-studio)
    [SerializeField] private HapticClip clip1;
    private HapticClipPlayer leftClipPlayer1;
    private HapticClipPlayer rightClipPlayer1;

    protected virtual void Start()
    {
        // We create two haptic clip players for each hand.
        leftClipPlayer1 = new HapticClipPlayer(clip1);
        rightClipPlayer1 = new HapticClipPlayer(clip1);
    }

    public void Play()
    {
        rightClipPlayer1.Play(Controller.Right);
        leftClipPlayer1.Play(Controller.Left);
    }
    
    public void Stop()
    {
        rightClipPlayer1.Stop();
        leftClipPlayer1.Stop();
    }

    protected virtual void OnDestroy()
    {
        leftClipPlayer1?.Dispose();
        rightClipPlayer1?.Dispose();
    }

    // Upon exiting the application (or when playmode is stopped) we release the haptic clip players and uninitialize (dispose) the SDK.
    protected virtual void OnApplicationQuit()
    {
        Haptics.Instance.Dispose();
    }
}
