using UnityEngine;
using UnityEngine.AI;

public class AITargetScript : MonoBehaviour
{
    public Transform Target;
    public float TargetingDistance;

    private NavMeshAgent m_Agent;
    private float m_Distance;


    void Start() {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        m_Distance = Vector3.Distance(m_Agent.transform.position, Target.position);
        if (m_Distance > TargetingDistance) {
            m_Agent.isStopped = true;
        }
        else {
            m_Agent.isStopped = false;
            m_Agent.destination = Target.position;
        }
    }
}
