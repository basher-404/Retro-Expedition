using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone_Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private AudioSource deathSoundEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        BoxCollider2D objectBoxCollider = GetComponent<BoxCollider2D>();
        if (collision.gameObject.name == "Sand Block")
        {
            StartCoroutine(Die());
            if (objectBoxCollider != null)
            {
                objectBoxCollider.enabled = false;
            }
        }
    }
   

    private IEnumerator Die()
    {
        anim.SetTrigger("Death");
        deathSoundEffect.Play();
        yield return new WaitForSeconds(0.1f);

        // Move the enemy upwards
        float upwardSpeed = 12f;
        float fallSpeed = 20f;
        float upwardDuration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < upwardDuration)
        {
            transform.Translate(Vector3.up * upwardSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fall off the map
        rb.isKinematic = false;
        rb.velocity = new Vector2(0, -fallSpeed);

        // Wait for the enemy to fall off the map
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
