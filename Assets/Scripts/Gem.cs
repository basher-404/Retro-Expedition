using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public AudioSource collected;
    public CircleCollider2D circleCollider;
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb= GetComponent<Rigidbody2D>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collected.Play();
            rb.velocity = Vector2.zero; // Stop movement
            rb.bodyType = RigidbodyType2D.Static; // Disable physics
            StartCoroutine(destroyObject());
        }
    }

    public IEnumerator destroyObject()
    {
        anim.SetTrigger("is_acquired");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
