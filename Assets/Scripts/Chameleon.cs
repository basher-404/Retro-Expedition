using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chameleon : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private float attackRange = 3f;
    private bool canAttack = true;
    private Rigidbody2D rb;
    private Animator anim;
    private Rigidbody2D playerRb;
    public BoxCollider2D bodyCollider;  // Changed to public

    [SerializeField] private AudioSource tongue_attack;
    [SerializeField] private AudioSource hit_Sound_1;
    [SerializeField] private AudioSource hit_Sound_2;

    //Health System
    [SerializeField] private int maxHP = 150;
    public int currentHP;

    //Visiblity Range (Turn Chameleon visible or invisible)
    [SerializeField] private float visibilityRange = 7f; // Range at which the chameleon sees the player
    private SpriteRenderer spriteRenderer;
    private bool isVisible = false;
    private float timeOutOfSight = 0f;
    private bool isFadingOut = false;

    //Tongue Attack
    public BoxCollider2D tongueCollider_1;
    public BoxCollider2D tongueCollider_2;
    public BoxCollider2D tongueCollider_3;

    //Gem mechanic
    public bool spawn_a_gem= false;
    private Spawn_Gem spawnGem;

    void Start()
    {
        playerRb = Player.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        anim.SetBool("can_attack", false);
        currentHP = maxHP;

        //Make chameleon invisible
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1, 1, 1, 0);

        //Handling Colliders
        bodyCollider = GetComponent<BoxCollider2D>();
        tongueCollider_1.enabled = false;
        tongueCollider_2.enabled = false;
        tongueCollider_3.enabled = false;

        //Gem
        spawnGem = FindObjectOfType<Spawn_Gem>();
    }

    void Update()
    {
        //Becoming visible again
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (distanceToPlayer <= visibilityRange)
        {
            // Player is visible, reset the timer and ensure visibility
            timeOutOfSight = 0f;
            isFadingOut = false;

            if (!isVisible)
            {
                StartCoroutine(FadeIn()); // Start fade-in animation
            }
        }
        else
        {
            // Player is out of sight, start tracking time
            if (!isFadingOut)
            {
                timeOutOfSight += Time.deltaTime;

                if (timeOutOfSight >= 3f) // Wait for 3 seconds
                {
                    StartCoroutine(FadeOut());
                    isFadingOut = true; // Prevent multiple fade-outs
                }
            }
        }


        // Face the player
        Vector2 directionToPlayer = Player.position - transform.position;
        if (directionToPlayer.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(-directionToPlayer.x), 1, 1);
        }

        // Calculate ray direction based on facing
        Vector2 rayDirection = transform.localScale.x < 0 ? Vector2.right : Vector2.left;

        // Cast detection ray
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            rayDirection,
            attackRange
        );

        // Visualize ray in Scene view
        Debug.DrawRay(transform.position, rayDirection * attackRange, Color.red);
        Debug.DrawRay(transform.position+ new Vector3(0, -0.5f, 0), rayDirection * visibilityRange, Color.blue);

        // Check if player is in range and line of sight
        // Attack only if cooldown allows
        if (canAttack)
        {
            StartCoroutine(attack_routine(hit));
        }

    }

    private IEnumerator attack_routine(RaycastHit2D hit)
    {
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            canAttack = false; // Prevent immediate reattacks
            anim.SetBool("can_attack", true);
            tongue_attack.Play();

            yield return new WaitForSeconds(0.5f); // Attack delay

            anim.SetBool("can_attack", false);
            yield return new WaitForSeconds(0.5f); // Cooldown period

            canAttack = true; // Allow new attacks
        }
        else
        {
            anim.SetBool("can_attack", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Player.position.y > transform.position.y + 1f)
            {
                // Player is moving downwards and above the enemy
                TakeDamage(50); // Enemy takes damage
                // Launch player on air after hitting enemy
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

    private void TakeDamage(int damage)
    {
        currentHP -= damage;
        anim.SetBool("is_hit", true);
        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }

        else
        {
            (Random.Range(0, 2) == 0 ? hit_Sound_1 : hit_Sound_2).Play();
            StartCoroutine(ResetHitAnimation());
        }
    }

    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the hit animation to play
        anim.SetBool("is_hit", false); // Reset is_hit to false
    }

    private IEnumerator Die()
    {
        anim.SetBool("is_dead", true);
        (Random.Range(0, 2) == 0 ? hit_Sound_1 : hit_Sound_2).Play();
        yield return new WaitForSeconds(0.1f);
        bodyCollider.enabled = false;

        //Spawn a gem
        if (spawn_a_gem == true)
        {
            if (spawnGem != null)
            {
                spawnGem.SpawnObject(); // Call the function
            }
            else
            {
                Debug.LogWarning("Spawn_Gem script not found!");
            }
        }

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

    public void TongueCollider_1_On()
    {
        //Disable
        tongueCollider_2.enabled = false;
        tongueCollider_3.enabled = false;

        //Enable
        tongueCollider_1.enabled = true;
    }

    public void TongueCollider_2_On()
    {
        //Disable
        tongueCollider_1.enabled = false;
        tongueCollider_3.enabled = false;

        //Enable
        tongueCollider_2.enabled = true;
    }
    public void TongueCollider_3_On()
    {
        //Disable
        tongueCollider_1.enabled = false;
        tongueCollider_2.enabled = false;

        //Enable
        tongueCollider_3.enabled = true;
    }

    public void disable_TongueColliders()
    {
        tongueCollider_1.enabled = false;
        tongueCollider_2.enabled = false;
        tongueCollider_3.enabled = false;
    }

    private IEnumerator FadeIn()
    {
        isVisible = true;
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration; // Gradual increase in opacity
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, 1); // Ensure full visibility
    }

    private IEnumerator FadeOut()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = 1 - (elapsedTime / duration); // Gradual decrease the opacity
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, 0); // Make the chameleon invisibile
        isVisible = false;
    }
}
