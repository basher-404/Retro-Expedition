using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_Shoots : MonoBehaviour
{
    public AudioSource launcherBlast_soundEffect;
    public GameObject objectPrefab; // Assign your prefab in the inspector
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool affectedByGravity = false; // Add this variable
    public Animator animator;
    public trunk_enemy trunk;
    [SerializeField] private bool isBee = false; // Add this variable
    [SerializeField] AudioSource buzz;

    void Start()
    {
        // Start the SpawnObject coroutine
        StartCoroutine(SpawnObject());
    }

    private void Update()
    {
        if (!trunk.canShoot)
        {
            buzz.Stop();
        }
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            // Set the animator to the shooting state
            animator.SetBool("is_shooting", true);

            yield return new WaitForSeconds(0.8f);

            bulletspawn();

            // Set the animator back to the idle state
            animator.SetBool("is_shooting", false);

            // Wait for the idle period before the next shot
            yield return new WaitForSeconds(2); // Adjust this value to match the desired break time
        }
    }

    private void bulletspawn()
    {
        if (trunk.canShoot)
        {
            // Calculate the spawn position
            Vector3 spawnPosition = transform.position + new Vector3(0, -1f, 0);

            // Instantiate the object at the calculated position
            GameObject instance = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
            launcherBlast_soundEffect.Play();
            Rigidbody2D rb = instance.AddComponent<Rigidbody2D>();

            // Set gravity scale based on the affectedByGravity variable
            rb.gravityScale = affectedByGravity ? 1 : 0;

            // Set the velocity of the bullet
            if (isBee)
            {
                rb.velocity = new Vector2(0, -speed); // Shoot downwards
            }
            else
            {
                if (Mathf.Approximately(transform.rotation.eulerAngles.z, 0))
                {
                    rb.velocity = new Vector2(speed, 0);
                }
                else
                {
                    rb.velocity = new Vector2(25, -speed);
                }
            }
        }
    }
}
