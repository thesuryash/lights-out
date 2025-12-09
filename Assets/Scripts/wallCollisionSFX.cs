using UnityEngine;
using System.Collections.Generic;

public class WallHitController : MonoBehaviour
{
    public CharacterController characterController;
    public AudioSource audioSource;
     
    [Header("Assign Wall Clips by Tag")]
    public List<WallHitSet> wallHits = new();
    private Dictionary<string, AudioClip[]> wallHitDict;
    public AudioClip[] wallClips;
    //public float stepInterval = 0.5f; // seconds between steps
    //private float stepTimer;
    private Vector3 prevPos;
    void Start()
    {
        // Build dictionary on startup
        wallHitDict = new Dictionary<string, AudioClip[]>();
        foreach (var set in wallHits)
        {
            if (!wallHitDict.ContainsKey(set.tag))
                wallHitDict.Add(set.tag, set.clips);
        }
        prevPos = transform.position;
        PlayWallHit();
    }
    void Update()
    {
        Vector3 currentPos = transform.position;

        // Check only X or Z movement
        bool movedXZ = Mathf.Round(currentPos.x) != Mathf.Round(prevPos.x) || Mathf.Round(currentPos.z) != Mathf.Round(prevPos.z);

        if (movedXZ)
        {
            PlayWallHit();
        }

        // Update previous position
        prevPos = currentPos;
    }

    void PlayWallHit()
    {
        if (wallClips == null || wallClips.Length == 0)
            return;

        int index = Random.Range(0, wallClips.Length);
        //audioSource.PlayOneShot(wallClips[index]);
        audioSource.clip = wallClips[index];
        audioSource.Play();

    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Wall")
        {
            wallClips = wallHitDict["Wall"];
        }
        else
        {
            wallClips = null;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        audioSource.Stop();
    }
}

[System.Serializable]
public class WallHitSet
{
    public string tag;           // e.g. "wood", "grass", "metal"
    public AudioClip[] clips;    // All footstep sounds for this surface
}
