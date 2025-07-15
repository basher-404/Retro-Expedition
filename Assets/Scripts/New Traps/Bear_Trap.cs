using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear_Trap : MonoBehaviour
{
    private Animator anim;
    private bool has_been_activated_before = false;
    public BoxCollider2D trap_setter;
    public BoxCollider2D killer_collider;

    private void Start()
    {
        anim = GetComponent<Animator>();
        killer_collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (has_been_activated_before)
        {
            return;
        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetBool("activated", true);
            killer_collider.enabled = true;
            has_been_activated_before = true;
        }
    }

    public void disable_trap_collider()
    {
        killer_collider.enabled = false;
    }
}
