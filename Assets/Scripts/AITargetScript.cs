using UnityEngine;
using UnityEngine.AI;

public class AITargetScript : MonoBehaviour
{
    public Transform target;
    public float TargetingDistance;
    public float waitTimeOnWayPoint = 1f;
    public Path path;

    private NavMeshAgent m_Agent;
    private float m_Distance;
    private bool m_PathCalculate = false;

    float time = 0f;

    private void Awake() {

        m_Agent = GetComponent<NavMeshAgent>();

    }

    void Start() {

        m_Agent.destination = path.GetCurrentWayPoint();

    }

    void FixedUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f; // Eye height
        Vector3 direction = (target.position - origin).normalized;
        float maxDistance = Vector3.Distance(origin, target.position);

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Player"))
            {
                m_PathCalculate = true;
            }
            else
            {
                m_PathCalculate = false;
            }
        }
        else
        {
            m_PathCalculate = false;
        }
    }

    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, target.position);

        if (m_PathCalculate && m_Distance < TargetingDistance) 
        {
            Chase();
        }
        else 
        {
            Patrol();
        }
    }

    void Patrol()
    {
        m_Agent.destination = path.GetCurrentWayPoint();

        if (m_Agent.remainingDistance <= 0.1f)
        {
            time += Time.deltaTime;

            if (time >= waitTimeOnWayPoint)
            {
                time = 0f;
                m_Agent.destination = path.GetNextWayPoint();
            }
        }
    }

    void Chase()
    {
        m_Agent.destination = target.position;
    }




    // void Update() {

    //     m_Distance = Vector3.Distance(m_Agent.transform.position, target.position);
        

    //     if (m_Distance > TargetingDistance) 
    //     {
    //         m_Agent.destination = path.GetCurrentWayPoint();
            
    //         if (m_Agent.remainingDistance <+ 0.1f)
    //         {
    //             time += Time.deltaTime;
    //             if (time >= waitTimeOnWayPoint)
    //             {
    //                 time = 0f;
    //                 m_Agent.destination = path.GetNextWayPoint();
    //             }
    //         }
    //     }
    //     else 
    //     {
    //         m_Agent.destination = target.position;

    //         if (!m_Agent.hasPath && m_PathCalculate) 
    //         {
    //             m_Agent.destination = path.GetCurrentWayPoint();

    //             if (m_Agent.remainingDistance <+ 0.1f)
    //             {
    //                 time += Time.deltaTime;
    //                 if (time >= waitTimeOnWayPoint)
    //                 {
    //                     time = 0f;
    //                     m_Agent.destination = path.GetNextWayPoint();
    //                 }
    //             }
    //         }
    //         else 
    //         {
    //             m_Agent.destination = target.position;
    //             m_PathCalculate = true;
    //         }
    //     }
    // }
}
