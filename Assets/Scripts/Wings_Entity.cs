using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Wings_Entity : MonoBehaviour
{
    public AudioSource wingsDeeath_soundEffect;
    Animator anim;
    [SerializeField] private GameObject[] wayPoints;
    private int currentWWayPointIndex = 0;
    private SpriteRenderer sprite;
    [SerializeField] private float speed = 2f;

    private void Start()
    {
        anim = GetComponent <Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (Vector2.Distance(wayPoints[currentWWayPointIndex].transform.position, transform.position) < 0.1f)
        {
            currentWWayPointIndex++;
            sprite.flipX = true;
            if (currentWWayPointIndex >= wayPoints.Length)
            {
                currentWWayPointIndex = 0;
                sprite.flipX = true;
            }
        }

        float pointXValue = wayPoints[currentWWayPointIndex].transform.position.x;

        // Get the X Value of the Enemy
        float enemyXValue = gameObject.transform.position.x;

        // PatrolPoint is to the Left
        if (pointXValue < enemyXValue)
        {
            // Flip Sprite on the X Axis
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // PatrolPoint is to the Right
        else if (pointXValue > enemyXValue)
        {
            // Flip Sprite back on the X Axis
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWWayPointIndex].transform.position, Time.deltaTime * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       Death(collision.gameObject.transform);

    }

    void Death(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Trap"))
        {

            anim.SetBool("Dead", true);
            wingsDeeath_soundEffect.Play();
            Destroy(gameObject,2f);
            BoxCollider2D objectBoxCollider = GetComponent<BoxCollider2D>();
            if (objectBoxCollider != null)
            {
                objectBoxCollider.enabled = false;
            }
        }
    }
}
