using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Fan_Up : MonoBehaviour
{
   
    public float JumpForce_with__Fan;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LaunchObject(collision.gameObject.transform);
    }

    void LaunchObject(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Player"))
        {
            PlayerMovement playM= objectTransform.GetComponent< PlayerMovement > ();
            playM.jumpForce = 24f;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        inAir(collision.gameObject.transform);
    }

    void inAir(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Player"))
        {
            PlayerMovement playM = objectTransform.GetComponent<PlayerMovement>();
            playM.jumpForce = 14f;
        }
    }
}
  