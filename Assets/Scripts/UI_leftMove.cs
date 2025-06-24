using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_leftMove : MonoBehaviour
{
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script

    public void MoveLeft()
    {

        if (playerMovement.canMove) // Check if the player can move
        {
            Debug.Log("Button Pressed");
            playerMovement.dirX = -1f; // Set direction to left
            playerMovement.rb.velocity = new Vector2(playerMovement.dirX * playerMovement.moveSpeed, playerMovement.rb.velocity.y);
        }
    }

    public void StopMoving()
    {
        if (playerMovement.canMove) // Check if the player can move
        {
            Debug.Log("Button Released");
            playerMovement.dirX = 0f; // Stop movement
        }
    }
}
