using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushing_Hammer : MonoBehaviour
{
    private Animator anim;
    public BoxCollider2D trap_setter;
    public bool is_attacking = false;
    public bool is_retracting = false;

    [Header("Box Colliders for Hammer while attacking")]
    public BoxCollider2D frame1;
    public BoxCollider2D frame2;

    private void Start()
    {
        anim = GetComponent<Animator>();
        frame1.enabled = false;
        frame2.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (is_attacking || is_retracting)
        {
            return;
        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            is_attacking=true;
            reset_animations();
            anim.SetBool("is_attacking", true);
        }
    }

    public void setup_collider1()
    {
        disable_allcolliders();
        frame1.enabled = true;
    }

    public void setup_collider2()
    {
        disable_allcolliders();
        frame2.enabled = true;
    }

    public void disable_allcolliders()
    {
        frame1.enabled = false;
        frame2.enabled = false;
    }

    public void reset_to_idle()
    {
        reset_animations();
        disable_allcolliders();
        anim.SetBool("is_idle", true);
        is_retracting = false;
        is_attacking = false;
    }

    public void retract()
    {
        reset_animations();
        is_retracting = true;
        anim.SetBool("is_retracting", true);
    }

    public void reset_animations()
    {
        anim.SetBool("is_attacking", false);
        anim.SetBool("is_retracting", false);
        anim.SetBool("is_idle", false);
    }
}
