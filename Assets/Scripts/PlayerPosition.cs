using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The Collider to associate with the XR device.")]
    Transform m_Collider;
    
    public Transform Collider
    {
        get => m_Collider;
        set => m_Collider = value;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.y = m_Collider.position.y + 0.1f;
        transform.position = newPos;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Wall"))
        {
            Debug.Log("CharacterController hit a wall!");
        }
    }
}
