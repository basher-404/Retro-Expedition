using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline_Trigger : MonoBehaviour
{
    public float launchForce = 10f; // Adjust this value to control the launch force
    public float box_launchForceX;
    public float box_launchForceY;

    private Animator anim;
    [SerializeField] private AudioSource activateTrampoline;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LaunchObject(collision.gameObject.transform);
    }

    void LaunchObject(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Box"))
        {
            Vector2 launchVelocity = new Vector2(box_launchForceX, box_launchForceY);
            objectTransform.GetComponent<Rigidbody2D>().velocity = launchVelocity;
            anim.SetTrigger("Activate");
            activateTrampoline.Play();
        }
        
       else if (objectTransform.CompareTag("Player"))
        {
            // Apply upward force to simulate a jump-like effect
            Vector2 launchVelocity = new Vector2(0f, launchForce);
            objectTransform.GetComponent<Rigidbody2D>().velocity = launchVelocity;
            anim.SetTrigger("Activate");
            activateTrampoline.Play();
        }
    }
}