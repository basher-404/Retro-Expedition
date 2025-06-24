using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public bool isNearShaft = false;
    public bool isNearLever = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Shaft"))
        {
            isNearShaft = true;
        }
        else if (collision.collider.CompareTag("Lever"))
        {
            isNearLever = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Shaft"))
        {
            isNearShaft = false;
        }
        else if (collision.collider.CompareTag("Lever"))
        {
            isNearLever = false;
        }
    }
}
