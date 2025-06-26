using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starfish_Man : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    [Header("References")]
    [SerializeField] private Transform Player;

    [Header("Timings & Speeds")]
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float bounceForce = 15f;

    // --- Internal ---
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private Rigidbody2D playerRb;
    private bool attacking;
    private Transform targetPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerRb = Player.GetComponent<Rigidbody2D>();

        // start at point1
        targetPoint = point1;
        attacking = false;

        // kick off the idle/attack loop
        StartCoroutine(AttackCycle());
    }

    /// <summary>
    /// 1) Idle for idleTime
    /// 2) Set attacking = true, play "is_attacking"
    /// 3) Move towards current target
    /// 4) When reached, stop attack, flip target, loop
    /// </summary>
    private IEnumerator AttackCycle()
    {
        while (true)
        {
            // 1) Idle
            yield return new WaitForSeconds(idleTime);

            // 2) Enter attack
            attacking = true;
            anim.SetBool("is_attacking", true);

            // 3) Slide towards targetPoint
            while (Vector2.Distance(transform.position, targetPoint.position) > 0.05f)
            {
                // Move
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPoint.position,
                    moveSpeed * Time.deltaTime
                );

                // Flip to face  
                float dirX = targetPoint.position.x - transform.position.x;
                if (dirX != 0)
                    transform.localScale = new Vector3(Mathf.Sign(dirX), 1, 1);

                yield return null;
            }

            // 4) Stop attack
            anim.SetBool("is_attacking", false);
            attacking = false;

            // Swap direction
            targetPoint = (targetPoint == point1) ? point2 : point1;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // can only be killed when idle AND jumped on
        bool stomp = Player.position.y > transform.position.y + 1f;
        if (stomp && !attacking)
        {
            StartCoroutine(Die());
            // bounce the player
            playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
        }
        else
        {
            // either side‐hit or attacking: hurt player
            collision.gameObject.GetComponent<Player_Life>().die();
        }
    }

    /// <summary>
    /// Exactly the same death: play death anim, pop up, fall, then Destroy
    /// </summary>
    private IEnumerator Die()
    {
        anim.SetBool("is_dead", true);
        yield return new WaitForSeconds(0.1f);

        boxCollider.enabled = false;

        // pop up a little
        float upwardSpeed = 12f, fallSpeed = 20f, upwardDuration = 0.15f;
        float t = 0f;
        while (t < upwardDuration)
        {
            transform.Translate(Vector3.up * upwardSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        // then fall
        rb.isKinematic = false;
        rb.velocity = new Vector2(0, -fallSpeed);

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
