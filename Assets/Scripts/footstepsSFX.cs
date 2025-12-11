using UnityEngine;
using System.Collections.Generic;

public class FootstepController : MonoBehaviour
{
    public AudioSource audioSource;
     
    [Header("Assign Footstep Clips by Tag")]
    public List<FootstepSet> footstepSets = new();
    private Dictionary<string, AudioClip[]> footstepDict;
    private AudioClip[] currentAudioClips;
    public float stepInterval = 0.5f; // seconds between steps
    private float stepTimer;
    private Vector3 prevPos;
    private bool movedXZ;
    public HapticManager hapticManager;
    private string currentTag;
    public bool causesHaptics;

    void Start()
    {
        // Build dictionary on startup
        footstepDict = new Dictionary<string, AudioClip[]>();
        foreach (var set in footstepSets)
        {
            if (!footstepDict.ContainsKey(set.tag))
                footstepDict.Add(set.tag, set.clips);
        }
        prevPos = transform.position;
        currentAudioClips = footstepDict["Wood"]; 
        PlayFootstep();
    }

    void Update()
    {
        Vector3 currentPos = transform.position;

        // Check only X or Z movement
        movedXZ = Mathf.Round(currentPos.x) != Mathf.Round(prevPos.x) || Mathf.Round(currentPos.z) != Mathf.Round(prevPos.z);

        if (movedXZ)
        {
            PlayFootstep();
        }

        // Update previous position
        prevPos = currentPos;
    }

    void PlayFootstep()
    {
        if (currentAudioClips == null || currentAudioClips.Length == 0)
            return;

        int index = Random.Range(0, currentAudioClips.Length);
        //audioSource.PlayOneShot(currentAudioClips[index]);
        audioSource.clip = currentAudioClips[index];
        audioSource.Play();
        if (causesHaptics)
        {
            hapticManager.Play();
        }

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        string tag = hit.gameObject.tag;
        Debug.Log(tag);

        // Check if this tag exists in the dictionary
        if (footstepDict.ContainsKey(tag) && tag != currentTag)
        {
            currentAudioClips = footstepDict[tag]; 
        }
    }
}

[System.Serializable]
public class FootstepSet
{
    public string tag;           // e.g. "wood", "grass", "metal"
    public AudioClip[] clips;    // All footstep sounds for this surface
}
