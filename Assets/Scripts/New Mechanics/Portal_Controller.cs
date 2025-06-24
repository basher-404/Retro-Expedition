using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Controller : MonoBehaviour
{
    [Header("References")]
    public Portal_Node entrance;
    public Portal_Node exit;
    public Camera_Controller_Level6 cameraController;

    [Header("First Entry Settings")]
    public float cameraShiftDuration = 1.5f;
    public float portalFadeInDuration = 2f;
    public float postFadeDelay = 0.5f;

    public bool firstEntryCompleted;
    private SpriteRenderer exitRenderer;
    private PlayerMovement player;
    public bool isInSequence = false;

    private void Start()
    {
        // Configure portal pairing
        entrance.pairedPortal = exit;
        exit.pairedPortal = entrance;

        // Get exit renderer and set transparent
        exitRenderer = exit.GetComponent<SpriteRenderer>();
        if (exitRenderer != null)
        {
            Color c = exitRenderer.color;
            c.a = 0f;
            exitRenderer.color = c;
        }

        firstEntryCompleted = false;
        player = FindObjectOfType<PlayerMovement>();
    }

    public void OnFirstEntryTriggered()
    {
        if (firstEntryCompleted || isInSequence || player == null) return;

        StartCoroutine(FirstEntrySequence());
    }

    private IEnumerator FirstEntrySequence()
    {
        isInSequence = true;
        firstEntryCompleted = true;

        // Freeze player
        player.canMove = false;

        // Disable entrance collider to prevent re-triggering
        Collider2D entranceCollider = entrance.GetComponent<Collider2D>();
        bool wasEntranceEnabled = entranceCollider.enabled;
        entranceCollider.enabled = false;

        // Shift camera to exit location
        yield return StartCoroutine(cameraController.ShiftToPosition(
            exit.transform.position,
            cameraShiftDuration
        ));

        // Fade in exit portal
        yield return StartCoroutine(FadePortal(0f, 1f, portalFadeInDuration));

        // Short delay before teleporting
        yield return new WaitForSeconds(postFadeDelay);

        // Actually teleport player to exit position
        TeleportPlayerToExit();

        // Play teleport sound at exit portal
        exit.PlayTeleportSound();

        // Re-enable entrance collider
        entranceCollider.enabled = wasEntranceEnabled;

        // Unfreeze player
        player.canMove = true;

        // Release camera to follow player again
        cameraController.ReleaseCamera();

        isInSequence = false;
    }

    private void TeleportPlayerToExit()
    {
        if (player == null) return;

        // Disable player collider temporarily
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        bool wasPlayerColliderEnabled = playerCollider.enabled;
        playerCollider.enabled = false;

        // Perform teleportation
        player.Teleport(exit.transform.position);

        // Re-enable player collider
        playerCollider.enabled = wasPlayerColliderEnabled;

        // Add teleport cooldown
        Portal_Cooldown cooldown = player.gameObject.AddComponent<Portal_Cooldown>();
        cooldown.SetDuration(1f); // Longer cooldown for first teleport
    }

    private IEnumerator FadePortal(float startAlpha, float endAlpha, float duration)
    {
        if (exitRenderer == null) yield break;

        float elapsed = 0f;
        Color c = exitRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            exitRenderer.color = c;
            yield return null;
        }

        c.a = endAlpha;
        exitRenderer.color = c;
    }
}
