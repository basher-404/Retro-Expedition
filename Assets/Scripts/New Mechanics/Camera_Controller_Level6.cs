using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller_Level6 : MonoBehaviour
{
    private Transform player;
    [HideInInspector]
    public bool isShaking = false;  // New flag
    private Vector3? customPosition = null;
    private bool isShifting = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        // Prevent overriding shake or shift adjustments.
        if (isShifting || isShaking) return;

        if (customPosition.HasValue)
        {
            // Maintain custom position until released
            transform.position = customPosition.Value;
        }
        else
        {
            // Normal camera follow
            transform.position = new Vector3(
                player.position.x,
                player.position.y,
                transform.position.z
            );
        }
    }

    public IEnumerator ShiftToPosition(Vector3 targetPosition, float duration)
    {
        isShifting = true;
        Vector3 startPosition = transform.position;
        targetPosition.z = startPosition.z; // Maintain camera Z

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        customPosition = targetPosition; // Lock camera at this position
        isShifting = false;
    }

    public void ReleaseCamera()
    {
        customPosition = null;
    }
}
