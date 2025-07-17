using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delayed_Waypoint_Follower : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float waitTime = 0.5f;

    [Header("Waypoints")]
    public Transform[] waypoints;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        // Initialize position to first waypoint
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        if (waypoints.Length == 0 || waypoints.Length == 1) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (isWaiting)
        {
            // Count down wait timer
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        // Get current target waypoint
        Transform target = waypoints[currentWaypointIndex];

        // Move toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            StartWaiting();
        }
    }

    private void StartWaiting()
    {
        // Start waiting period
        isWaiting = true;
        waitTimer = waitTime;

        // Move to next waypoint index
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    // Optional: Visualize waypoint connections in editor
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw waypoint markers
            Gizmos.DrawSphere(waypoints[i].position, 0.1f);

            // Draw connections between waypoints
            int nextIndex = (i + 1) % waypoints.Length;
            if (waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }
        }
    }
}
