using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Portal_Node : MonoBehaviour
{
    [HideInInspector] public Portal_Node pairedPortal;
    private Portal_Controller portalController;
    private Collider2D portalCollider;

    [Header("Sound Effects")]
    public AudioSource teleportSound;
    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        portalCollider.isTrigger = true;
        portalController = GetComponentInParent<Portal_Controller>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Skip if we're in the middle of a sequence
        if (portalController.isInSequence) return;

        // Get root object of the collider
        GameObject teleportingObject = other.attachedRigidbody != null ?
            other.attachedRigidbody.gameObject :
            other.gameObject;

        // Skip if recently teleported
        if (teleportingObject.GetComponent<Portal_Cooldown>() != null) return;
        if (pairedPortal == null) return;

        // Handle first entry special case
        if (this == portalController.entrance &&
            teleportingObject.CompareTag("Player") &&
            !portalController.firstEntryCompleted)
        {
            portalController.OnFirstEntryTriggered();
            return;
        }

        // Apply cooldown and teleport
        teleportingObject.AddComponent<Portal_Cooldown>();
        TeleportObject(teleportingObject);
    }

    private void TeleportObject(GameObject obj)
    {
        PlayTeleportSound();

        Vector3 exitPosition = pairedPortal.transform.position;
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = PreserveVelocity(rb.velocity);
            rb.position = exitPosition;
        }
        else
        {
            obj.transform.position = exitPosition;
        }

        // If it's the player, ensure animation updates
        PlayerMovement player = obj.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.updateAnimationState();
        }
    }

    private Vector2 PreserveVelocity(Vector2 originalVelocity)
    {
        // Adjust velocity direction relative to portal exit rotation
        float angleDiff = pairedPortal.transform.eulerAngles.z - transform.eulerAngles.z;
        return Quaternion.Euler(0, 0, angleDiff) * originalVelocity;
    }

    public void PlayTeleportSound()
    {
        if (teleportSound != null)
        {
            teleportSound.Play();
        }
    }
}
