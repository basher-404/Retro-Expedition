using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike__Ball : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    public bool trapWorks = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetBool("Touch", true);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Destroy this object
            destroyComponents();
            Destroy(gameObject,2f); //Delays destruction for 2 seconds
        }
        Kill(collision.gameObject.transform);
    }

    void  Kill(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Destroyable Enemy"))
        {
            destroyComponents();
            Destroy(objectTransform.gameObject, 2f); // Destroy the GameObject, not the Transform
            trapWorks = false;
        }
        trapWorks = false;
    }

    void destroyComponents()
    {
        Rigidbody2D objectRigidbody = GetComponent<Rigidbody2D>();
        if (objectRigidbody != null)
        {
            objectRigidbody.simulated = false;
        }

        BoxCollider2D objectBoxCollider = GetComponent<BoxCollider2D>();
        if (objectBoxCollider != null)
        {
            objectBoxCollider.enabled = false;
        }
    }
}
