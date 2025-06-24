using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("How many gems the player needs to open this door")]
    public int gemsRequired = 1;

    [Tooltip("How many units down to slide when opening")]
    public float openDistance = 1f;

    private float openSpeed;

    // internal state
    private Vector3 _closedPos;
    private Vector3 _openPos;
    private bool _isOpen = false;
    private Coroutine _openRoutine;

    //animation and sound
    private Animator anim;
    public AudioSource door_open;
    [Header("Camera Shake Settings")]
    [Tooltip("The camera that will shake (if not set, the main camera will be used)")]
    public Camera cameraToShake;
    [Tooltip("Shake magnitude (intensity)")]
    public float shakeMagnitude = 0.1f;

    public GameObject textBubble;

    void Awake()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + Vector3.down * openDistance;

        // Adjust speed so the door takes exactly 4 seconds
        openSpeed = openDistance / 2f;
    }

    void Start()
    {
       anim = GetComponent<Animator>();
    }
        /// <summary>
        /// Called by the player when they hit the trigger.
        /// </summary>
    public void TryOpen(int gemsCollected)
    {
        if (_isOpen) return;                     // already open
        if (gemsCollected < gemsRequired)
        {
            // could play a “locked” sound or UI flicker here
            Debug.Log($"Need {gemsRequired} gems—only have {gemsCollected}.");
            textBubble.SetActive(true);
            return;
        }

        _isOpen = true;                          // prevent re-opening
        if (_openRoutine != null) StopCoroutine(_openRoutine);
        _openRoutine = StartCoroutine(SlideDown());
    }

    private IEnumerator SlideDown()
    {
        anim.SetTrigger("activated");
        door_open.Play();
        float distance = Vector3.Distance(_closedPos, _openPos);
        float duration = distance / openSpeed;

        // Start the camera shake coroutine concurrently.
        StartCoroutine(ShakeCamera(duration));

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(_closedPos, _openPos, t);
            yield return null;
        }

        transform.position = _openPos;
    }

    private IEnumerator ShakeCamera(float shakeDuration)
    {
        // Use the assigned camera, or default to the main camera if none set.
        Camera cam = cameraToShake != null ? cameraToShake : Camera.main;
        if (cam == null)
            yield break; // No camera available to shake!

        // If the camera has the new controller, notify it to stop updating position
        Camera_Controller_Level6 controller = cam.GetComponent<Camera_Controller_Level6>();
        if (controller != null)
        {
            controller.isShaking = true;
        }

        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Shake in x and y
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            cam.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }

        // Restore the original position
        cam.transform.localPosition = originalPos;

        if (controller != null)
        {
            controller.isShaking = false;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        textBubble.SetActive(false);
    }
}
