using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angry_Pig : MonoBehaviour
{
  
    [SerializeField] private Transform Player;
    [SerializeField] private Transform castPoint;
    private Rigidbody2D rb;
    private Animator anim;
    private Rigidbody2D playerRb;
    private BoxCollider2D boxCollider;

    //Sound Design
    [SerializeField] private AudioSource move_Sound;
    [SerializeField] private AudioSource run_Sound;
    [SerializeField] private AudioSource hit_Sound;
    [SerializeField] private AudioSource death_Sound;


    //WayPoint Follower
    [SerializeField] public GameObject[] wayPoints;
    public int currentWWayPointIndex = 0;

    [SerializeField] private float speed = 2f;
    //Chasing
    [SerializeField] private float agroRange;
    [SerializeField] private float chaseSpeed;

    [SerializeField] private int maxHP = 100;
    private int currentHP;

    bool isFacingLeft= false;

    void Start()
    {
        playerRb = Player.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentHP = maxHP;
    }

    void Update()
    {
        // Face the direction of movement
        if(CanSeePlayer(agroRange))
        {
            ChasePlayer();
        }
        else
        {
            StopChasingPlayer();
        }
    }

  void ChasePlayer()
    {
        if(transform.position.x < Player.position.x)
        {
            rb.velocity = new Vector2(chaseSpeed, 0);
            transform.localScale = new Vector3(1, 1 );
        }

        else
        {
            rb.velocity= new Vector2(-chaseSpeed, 0);
            transform.localScale= new Vector2(-1, 1);
        }
        anim.SetBool("is_running", true);

        if (!run_Sound.isPlaying)
        {
            move_Sound.Stop();
            run_Sound.Play();
        }

    }

    void StopChasingPlayer()
    {
        anim.SetBool("is_running",false); 

        //WayPoint Follower

        if (Vector2.Distance(wayPoints[currentWWayPointIndex].transform.position, transform.position) < 0.1f)
        {
            currentWWayPointIndex++;
            if (currentWWayPointIndex >= wayPoints.Length)
            {
                currentWWayPointIndex = 0;
            }
        }
        Vector2 direction = wayPoints[currentWWayPointIndex].transform.position - transform.position;
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, -1);
            isFacingLeft = direction.x < 0;
        }
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWWayPointIndex].transform.position, Time.deltaTime * speed);

        if (!move_Sound.isPlaying)
        {
            run_Sound.Stop();
            move_Sound.Play();
        }
    }

   bool CanSeePlayer(float distance)
{
    bool val = false;
    float castDist = distance;


    if (isFacingLeft)
    {
        castDist = -distance;
    }

    Vector2 castPoint2D = new Vector2(castPoint.position.x, castPoint.position.y);
    Vector2 endPos = castPoint2D + Vector2.right * castDist;

    RaycastHit2D hit = Physics2D.Linecast(castPoint2D, endPos, 1 << LayerMask.NameToLayer("Action"));

    if (hit.collider != null)
    {
        if (hit.collider.gameObject.CompareTag("Player"))
        {
            val = true;
        }
        else
        {
            val = false;
            Debug.DrawLine(castPoint2D, hit.point, Color.yellow);
        }
    }
    else
    {
        Debug.DrawLine(castPoint2D, endPos, Color.blue);
    }

    return val;
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
            hit_Sound.Play();
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
        if (move_Sound.isPlaying)
        move_Sound.Stop();

        if(run_Sound.isPlaying)
            run_Sound.Stop();

        anim.SetBool("is_dead", true);
        death_Sound.Play();
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