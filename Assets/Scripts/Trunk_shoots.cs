using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk_shoots : MonoBehaviour
{
    public AudioSource launcherBlast_soundEffect;
    public GameObject objectPrefab; // Assign your prefab in the inspector
    public Transform spawnPoint; // Assign your empty GameObject in the inspector
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool affectedByGravity = false; // Add this variable
    public Animator animator;
    public trunk_enemy trunk;

    void Start()
    {
        // Start the SpawnObject coroutine
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {

        while (true)
        {
         
                // Set the animator to the shooting state
                animator.SetBool("is_shooting", true);

                yield return new WaitForSeconds(0.8f);

                bulletspawn();

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
            // Instantiate the object at the position of the spawnPoint
            GameObject instance = Instantiate(objectPrefab, spawnPoint.position, Quaternion.identity);
            launcherBlast_soundEffect.Play();
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

            // Set gravity scale based on the affectedByGravity variable
            rb.gravityScale = affectedByGravity ? 1 : 0;

            // Set the velocity of the bullet
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
