using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Fan : MonoBehaviour
{
    public float upwardForce = 10f;

    // This static dictionary maps each trap's Rigidbody2D to the number of wind zones affecting it.
    private static Dictionary<Rigidbody2D, int> trapWindCounters = new Dictionary<Rigidbody2D, int>();

    // Called when a collider enters this wind fan's trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Remove any vertical freeze if previously applied.
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                // Increase the counter for this trap.
                if (trapWindCounters.ContainsKey(rb))
                {
                    trapWindCounters[rb]++;
                }
                else
                {
                    trapWindCounters[rb] = 1;
                }
            }
        }
    }

    // Called every fixed update while the trap is within this fan's trigger.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Apply the upward force, scaled for frame-rate independence.
                rb.AddForce(Vector2.up * upwardForce * Time.deltaTime, ForceMode2D.Force);
            }
        }
    }

    // Called when a collider exits this wind fan's trigger.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Reduce the counter for this trap.
                if (trapWindCounters.ContainsKey(rb))
                {
                    trapWindCounters[rb]--;
                    // Only freeze the trap if it is no longer affected by any wind fans.
                    if (trapWindCounters[rb] <= 0)
                    {
                        trapWindCounters.Remove(rb);

                        // Zero out its vertical velocity.
                        Vector2 newVelocity = rb.velocity;
                        newVelocity.y = 0f;
                        rb.velocity = newVelocity;

                        // Freeze the Y-axis so the trap stays at its current height.
                        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    }
                }
            }
        }
    }
}
