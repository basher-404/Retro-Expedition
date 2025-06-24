using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossBatlleCinematics : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject player;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera rhinoCamera;

    //Camera Shake Effect
    public CinemachineVirtualCamera CinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin _cbmcp;
    private float ShakeIntensity = 1f;
    private float timer;

    private PlayerMovement playerMovement;
    public GameObject rhino;
    private BoxCollider2D boxCollider;
    public bool playerHasEntered = false;

    //Rhino Cinematics
    public static bool isCinematicOver = false;

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        Debug.Log("Script started and PlayableDirector assigned.");
        rhinoCamera.gameObject.SetActive(false); // Ensure the rhino camera is disabled at the start
        playerMovement = player.GetComponent<PlayerMovement>(); // Get the PlayerMovement component
        boxCollider = GetComponent<BoxCollider2D>(); // Get the BoxCollider2D component
        StopShake();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            freezeMovement();
            playerHasEntered = true;
            rhinoCamera.gameObject.SetActive(true); // Enable the rhino camera
            playableDirector.Play();
            StartCoroutine(SwitchBackToPlayerCamera(playableDirector.duration - 2f)); // Start coroutine to switch back
            playableDirector.stopped += OnCutsceneEnd; // Subscribe to the stopped event
            boxCollider.enabled = false; // Disable the BoxCollider2D
        }
    }

    private IEnumerator SwitchBackToPlayerCamera(double switchTime)
    {
        yield return new WaitForSeconds((float)switchTime);
        playerCamera.gameObject.SetActive(true); // Enable the player camera
        rhinoCamera.gameObject.SetActive(false); // Disable the rhino camera
    }

    private void OnCutsceneEnd(PlayableDirector director)
    {
        movementAllow();
        playerCamera.gameObject.SetActive(true); // Ensure the player camera is enabled
        rhinoCamera.gameObject.SetActive(false); // Ensure the rhino camera is disabled
        playableDirector.stopped -= OnCutsceneEnd; // Unsubscribe from the stopped event

        // Start the rhino chasing the player
        if (rhino != null)
        {
            rhino.GetComponent<RhinoBoss>().StartChasing();
            ShakeCamera();
        }
        isCinematicOver = true;
    }

    public void ShakeCamera()
    {
        if (CinemachineVirtualCamera != null)
        {
            CinemachineBasicMultiChannelPerlin _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _cbmcp.m_AmplitudeGain = ShakeIntensity;
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera is not assigned.");
        }
    }

    public void StopShake()
    {
        if (CinemachineVirtualCamera != null)
        {
            CinemachineBasicMultiChannelPerlin _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _cbmcp.m_AmplitudeGain = 0f;
        }

        else
        {
            Debug.LogError("CinemachineVirtualCamera is not assigned.");
        }
    }

    public void freezeMovement()
    {
        playerMovement.canMove = false; // Disable player movement
    }

    public void movementAllow()
    {
        playerMovement.canMove = true; // Re-enable player movement
    }
}

