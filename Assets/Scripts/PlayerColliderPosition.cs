using UnityEngine;

public class PlayerColliderPosition : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The Camera to associate with the XR device.")]
    Transform m_Camera;
    
    /// <summary>
    /// The <see cref="Camera"/> used to render the scene from the point of view of the XR device. Must be a child of
    /// the <see cref="GameObject"/> containing this <c>XROrigin</c> component.
    /// </summary>
    /// <remarks>
    /// You can add a <see cref="UnityEngine.InputSystem.XR.TrackedPoseDriver"/> component to the <see cref="Camera"/>
    /// GameObject to update its position and rotation using tracking data from the XR device.
    /// You must update the <see cref="Camera"/> position and rotation using tracking data from the XR device.
    /// </remarks>
    public Transform Camera
    {
        get => m_Camera;
        set => m_Camera = value;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.x = m_Camera.position.x;
        newPos.z = m_Camera.position.z;
        transform.position = newPos;
    }


}
