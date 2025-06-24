using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointFollower : MonoBehaviour
{
    [SerializeField] public GameObject[] wayPoints;
    public int currentWWayPointIndex = 0;

    [SerializeField] private float speed = 2f;
    void Update()
    {
        if (Vector2.Distance(wayPoints[currentWWayPointIndex].transform.position, transform.position) < 0.1f)
        {
            currentWWayPointIndex++;
            if(currentWWayPointIndex >= wayPoints.Length)
            {
                currentWWayPointIndex = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWWayPointIndex].transform.position, Time.deltaTime * speed);
    }
}
