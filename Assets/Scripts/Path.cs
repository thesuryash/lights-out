using UnityEngine;

public class Path : MonoBehaviour
{   

    public Transform[] waypoints;

    private int direction = 1;
    int index;

    private int GetNextWayPointIndex()
    {
        index += direction;

        index %= waypoints.Length;

        return index;
    }

    public Vector3 GetCurrentWayPoint()
    {
        return waypoints[index].position;
    }

    public Vector3 GetNextWayPoint()
    {
        if (waypoints.Length == 0) return transform.position;

        index = GetNextWayPointIndex();
        Vector3 nextWayPoint = waypoints[index].position;

        return nextWayPoint;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
