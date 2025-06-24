using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    //Cherries
    private int cherries_collected = 0;
    [SerializeField] private TMP_Text cherries_text;
    [SerializeField] private AudioSource item_collection_SoundEffect;

    //Gems
    private int gems_collected = 0;
    [SerializeField] private TMP_Text gems_text;
    public GameObject gem_UI;

    private void Start()
    {
        gem_UI.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Cherries
        if (collision.gameObject.CompareTag("Cherry_Tag"))
        {
            Destroy(collision.gameObject);
            cherries_collected++;
            cherries_text.text = "x: " + cherries_collected;
            item_collection_SoundEffect.Play();
        }

        //Door Mechanic
        else if (collision.gameObject.CompareTag("Door"))
        {
            var door = collision.GetComponentInParent<Door>();
            if (door != null)
            {
                door.TryOpen(gems_collected);
            }
            gems_collected -= door.gemsRequired;
            decrement_Gems();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Gems
        if (collision.gameObject.CompareTag("Gem_Tag"))
        {
            CircleCollider2D gemCollider = collision.gameObject.GetComponent<CircleCollider2D>();
            if (gemCollider != null)
            {
                Destroy(gemCollider);
            }

            gems_collected++;

            if (gems_collected >= 1) // Show gem text when first gem is collected
            {
                gem_UI.SetActive(true);
            }
            gems_text.text = "x " + gems_collected;
        }
    }

    private void decrement_Gems()
    {
        gems_text.text = "x " + gems_collected;
        if (gems_collected <= 0)
        {
            gem_UI.SetActive(false);
        }
    }
}
