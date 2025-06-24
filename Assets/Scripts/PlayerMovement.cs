using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    public ParticleSystem runParticle;
    public ParticleSystem jumpDustParticle;

    [SerializeField] private LayerMask jumpableGround;
    private enum MovementState { idle, running, jumping, falling };
    public float moveSpeed = 7f;
    public float jumpForce = 14f;
    public float dirX = 0f;

    public AudioSource jumpSoundEffect;

    public bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!canMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
            return;
        }

        HandleMovementInput();

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            jumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (jumpDustParticle != null)
            {
                CreateJumpDust();
            }
        }

        updateAnimationState();
    }

    private void HandleMovementInput()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            dirX = Input.GetAxisRaw("Horizontal");
        }

        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    public void updateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("State", (int)state);
    }

    public bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }



    public void fxPlay()
    {
        if(runParticle!=null)
        {
            runParticle.Play();
        }   
    }

    private void CreateJumpDust()
    {
        Vector3 dustPosition = new Vector3(transform.position.x, transform.position.y - 0.9f, transform.position.z); // Adjust the Y offset as needed
        ParticleSystem dust = Instantiate(jumpDustParticle, dustPosition, Quaternion.identity);
        dust.Play();
        Destroy(dust.gameObject, dust.main.duration);
    }

    public void Teleport(Vector3 position)
    {
        if (rb != null)
        {
            rb.position = position;
            rb.velocity = Vector2.zero;
        }
        else
        {
            transform.position = position;
        }

        dirX = 0; // Reset movement direction

        // Update facing direction
        if (dirX != 0)
        {
            sprite.flipX = dirX < 0;
        }

        // Force update animation state
        updateAnimationState();

        // Debug log to confirm teleportation
        Debug.Log("Player teleported to: " + position);
    }
}
