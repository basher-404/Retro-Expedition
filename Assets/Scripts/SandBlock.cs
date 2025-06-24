using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent <Animator> ();
        if (anim != null)
        {
            anim.speed = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Mr.Stone")
        {
            Effects(collision.gameObject.transform);
        }

    }

    void Effects(Transform objectTransform)
    {
        anim.speed = 1f;
        anim.SetTrigger("Death");

    }

    private void die()
    {
        Destroy(gameObject);

    }

}
