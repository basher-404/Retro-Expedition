using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trunk_enemy : MonoBehaviour
{
    [SerializeField] private Transform Player;
    private Rigidbody2D rb;
    private Animator anim;
    private Rigidbody2D playerRb;
    private BoxCollider2D boxCollider;
    [SerializeField] private AudioSource hit_Sound;
    public bool canShoot = true;
    [SerializeField] private bool isBee = false;

    void Start()
    {
        playerRb = Player.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (isBee)
        {
            // Set up flying behavior for the bee
            rb.gravityScale = 0;
           }
        }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (Player.position.y > transform.position.y + 0.5f)
            {
                // Player is moving downwards and above the enemy
                StartCoroutine(Die()); // Enemy dies
                //Launch player on air after killing enemy
                float boostForce = 15f;
                playerRb.velocity = new Vector2(playerRb.velocity.x, boostForce);
            }
            else
            {
                // Player touches enemy
                Debug.Log("Player touched the enemy from the side");
                Player_Life playerLife = collision.gameObject.GetComponent<Player_Life>();
                playerLife.die();
            }
        }
    }

    private IEnumerator Die()
    {
        anim.SetBool("is_dead", true);
        hit_Sound.Play();
        canShoot = false;
        yield return new WaitForSeconds(0.1f);
        boxCollider.enabled = false;

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
