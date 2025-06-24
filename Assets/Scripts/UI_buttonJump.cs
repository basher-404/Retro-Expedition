using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_buttonJump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Button Pressed");
        if (playerMovement.isGrounded())
        {
            playerMovement.jumpSoundEffect.Play();
            playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, playerMovement.jumpForce);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // You can leave this method empty if you don't need to handle the pointer up event
    }
}
