using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RhinoBoss : MonoBehaviour
{
    [SerializeField] private Transform Player;
    private Rigidbody2D rb;
    public ParticleSystem runParticleSystem;

    private Animator anim;
    private Rigidbody2D playerRb;
    private BoxCollider2D boxCollider;
    public GameObject shakeCamera;
    public GameObject playerLife;

    // Sound Design
    [SerializeField] private AudioSource run_Sound;
    [SerializeField] private AudioSource hit_Sound;
    [SerializeField] private AudioSource death_Sound;

    // Health
    [SerializeField] private int maxHP = 500;
    private int currentHP;

    // Patrolling
    public Transform[] patrolPoints;
    public int patrolDestination;

    // Chasing
    private bool isChasing = false;
    private bool readyToChase = false;
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float reducedChaseSpeed = 3f;
    private float chaseTimer = 0f;
    public float chaseDelay = 2f;

    // Invulnerability
    private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 3f;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for blinking effect

    //After Rhino Dies
    public BossBatlleCinematics bossBattleCinematics;
    public BG_Music bgm;
    [SerializeField] private GameObject pathOfPlayer;
    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private GameObject trampolinesToAppear;
    [SerializeField] private GameObject launcherToDisappear;
    public CinemachineVirtualCamera pathCamera;


    void Start()
    {
        playerRb = Player.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        pathCamera.gameObject.SetActive(false);
        trampolinesToAppear.SetActive(false);
    }

    void Update()
    {
        if (!BossBatlleCinematics.isCinematicOver)
        {
            // Do nothing if the cinematic is not over
            return;
        }

        if (playerLife.GetComponent<Player_Life>().isAlive==false)
        {
            ResetEnemyState();
        }

        if (isChasing)
        {
            // Handle chasing behavior
            HandleChasing();
        }
        else
        {
            // Handle patrolling behavior
            HandlePatrol();

            // Check if the player is close enough to start the chasing delay
            if (Vector2.Distance(transform.position, Player.position) < chaseDistance)
            {
                readyToChase = true;
                chaseTimer += Time.deltaTime; // Increment the timer

                // Check if the delay time has passed
                if (chaseTimer >= chaseDelay)
                {
                    isChasing = true; // Start chasing after the delay
                }
            }
            else
            {
                // Reset the chase timer if the player is out of range
                readyToChase = false;
                chaseTimer = 0f;
            }
        }
    }

    private void HandleChasing()
    {
        Debug.Log("Chasing the player");
        if (transform.position.x > Player.position.x)
        {
            Debug.Log("Player is on the left");
            transform.localScale = new Vector3(1, 1, 1);
            transform.position += Vector3.left * chaseSpeed * Time.deltaTime;
        }
        else
        {
            Debug.Log("Player is on the right");
            transform.localScale = new Vector3(-1, 1, 1);
            transform.position += Vector3.right * chaseSpeed * Time.deltaTime;
        }
    }

    private void HandlePatrol()
    {
        // Patrol logic
        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[patrolDestination].position, chaseSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolPoints[patrolDestination].position) < 0.1f)
        {
            // Switch to the next patrol point
            patrolDestination = (patrolDestination + 1) % patrolPoints.Length;
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1); // Flip character sprite
        }
    }

    private void TakeDamage(int damage)
    {
        if (isInvulnerable) return; // Ignore damage if invulnerable

        currentHP -= damage;
        anim.SetBool("is_hit", true);
        hit_Sound.Play();

        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(BecomeInvulnerable()); // Start invulnerability
            StartCoroutine(ResetHitAnimation());
        }
    }

    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the hit animation to play
        anim.SetBool("is_hit", false); // Reset is_hit to false
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true; // Set invulnerable flag
        int playerLayer = LayerMask.NameToLayer("Action");
        int bossLayer = LayerMask.NameToLayer("Boss");

        // Store the original chase speed
        float originalChaseSpeed = chaseSpeed;
        // Reduce the chase speed
        chaseSpeed = reducedChaseSpeed;

        // Disable collision between player and boss
        Physics2D.IgnoreLayerCollision(playerLayer, bossLayer, true);

        StartCoroutine(BlinkEffect()); // Start blinking effect
        yield return new WaitForSeconds(invulnerabilityDuration); // Wait for invulnerability duration

        // Re-enable collision between player and boss
        Physics2D.IgnoreLayerCollision(playerLayer, bossLayer, false);

        chaseSpeed = originalChaseSpeed;
        isInvulnerable = false; // Reset invulnerable flag
    }

    private IEnumerator BlinkEffect()
    {
        Color originalColor = spriteRenderer.color; // Store the original color
        float blinkInterval = 0.1f; // Time between blinks
        float timeElapsed = 0f; // Time tracker for the invulnerability duration

        // Continuously blink until the invulnerability duration has passed
        while (timeElapsed < invulnerabilityDuration)
        {
            // Toggle visibility by changing the alpha value
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.11f); // Set alpha to 0.11
            Debug.Log("Blinking with low alpha");
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = originalColor; // Revert to original color
            yield return new WaitForSeconds(blinkInterval);

            // Increment the elapsed time
            timeElapsed += blinkInterval * 2;
        }
    }

    private IEnumerator Die()
    {
        if (run_Sound.isPlaying)
            run_Sound.Stop();

        anim.SetBool("is_hit", true);
        death_Sound.Play();
        yield return new WaitForSeconds(0.1f);
        boxCollider.enabled = false;
        shakeCamera.GetComponent<BossBatlleCinematics>().StopShake();

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

        bgm.stopBGM(); //stop boss music;
        scatterCollectibles(); //scatter cherries
        launcherToDisappear.SetActive(false); //disappear launcher after rhino dies
        bossBattleCinematics.freezeMovement(); //freeze movemnent of player after he dies

        int actionLayer = LayerMask.NameToLayer("Action");
        int trapLayer = LayerMask.NameToLayer("Trap");

        Physics.IgnoreLayerCollision(actionLayer, trapLayer, true); //disable collision while the cutscene plays


        // Switch to the spikes camera
        pathCamera.gameObject.SetActive(true);
        bossBattleCinematics.playerCamera.gameObject.SetActive(false);

        // Wait for the enemy to fall off the map
        yield return new WaitForSeconds(3f);

        bossBattleCinematics.movementAllow(); //allow movement after cutscene finishes
        Physics.IgnoreLayerCollision(actionLayer, trapLayer, false); //enable collision after the cutscene has ended

        // Deactivate the traps and clear the path of the player
        if (pathOfPlayer != null)
        {
            pathOfPlayer.SetActive(false);
        }
        // Switch back to the player camera
        pathCamera.gameObject.SetActive(false);
        bossBattleCinematics.playerCamera.gameObject.SetActive(true);

        //Delete Rhino GameObject from the Game
        Destroy(gameObject);
    }

    public void scatterCollectibles()
    {
        // Instantiate and scatter cherries
        int cherryCount = 100; // Number of cherries to spawn
        for (int i = 0; i < cherryCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-2f, 50f), Random.Range(-2f, 12f), 0);
            GameObject cherry = Instantiate(cherryPrefab, transform.position + randomOffset, Quaternion.identity);
            Rigidbody2D cherryRb = cherry.GetComponent<Rigidbody2D>();
            if (cherryRb != null)
            {
                float forceX = Random.Range(-5f, 5f);
                float forceY = Random.Range(2f, 5f);
                cherryRb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            }
        }

        trampolinesToAppear.SetActive(true);
    }
     public void PlayRunParticleEffect()
    {
        if (runParticleSystem != null)
        {
            runParticleSystem.Play();
        }
    }

    public void StartChasing()
    {
        isChasing = true;

        if (!anim.GetBool("is_running"))
        {
            anim.SetBool("is_running", true);
            run_Sound.Play(); 
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
                // Launch player in the air after hitting enemy
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

    public void ResetEnemyState()
    {
        isInvulnerable = false;
        chaseSpeed = 8f; // Ensure you have a reference to the original chase speed
        int playerLayer = LayerMask.NameToLayer("Action");
        int bossLayer = LayerMask.NameToLayer("Boss");
        Physics2D.IgnoreLayerCollision(playerLayer, bossLayer, false); // Re-enable collision
        boxCollider.enabled = true;
        isChasing = false;
        readyToChase = false;
   }

}
