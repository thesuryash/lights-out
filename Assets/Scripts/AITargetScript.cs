using UnityEngine;
using UnityEngine.AI;

public class AITargetScript : MonoBehaviour
{
    public Transform target;
    public float TargetingDistance;

    private NavMeshAgent m_Agent;
    private float m_Distance;
    // change this to a patrol setting instead of this thing
    private Vector3 m_StartingPoint;
    private bool m_PathCalculate = true;


    void Start() {

        m_Agent = GetComponent<NavMeshAgent>();
        m_StartingPoint = transform.position;

    }

    void Update() {
        m_Distance = Vector3.Distance(m_Agent.transform.position, target.position);

        if (m_Distance > TargetingDistance) 
        {
            m_Agent.isStopped = true;
        }
        else 
        {
            m_Agent.isStopped = false;
            m_Agent.destination = target.position;

            if (!m_Agent.hasPath && m_PathCalculate) 
            {
                m_Agent.destination = m_StartingPoint;
                m_PathCalculate = false;
            }
            else 
            {
                m_Agent.destination = target.position;
                m_PathCalculate = true;
            }
        }
    }
}
