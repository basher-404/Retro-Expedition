using System.Collections;
using UnityEngine;

public class Canon : MonoBehaviour
{
    public AudioSource launcherBlast_soundEffect;
    public GameObject objectPrefab; // Assign your prefab in the inspector
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool affectedByGravity = false; // For Canon and trunk shooter
    float offset = 1f;

    void Start()
    {
        // Start the SpawnObject coroutine
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        GameObject launcher = this.gameObject;
        while (true)
        {
            // Calculate the direction based on the launcher's rotation
            float angle = launcher.transform.rotation.eulerAngles.z;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Adjust the spawn position based on the direction
            Vector2 spawnPosition = (Vector2)transform.position + direction * offset;

            // Instantiate the object at the adjusted spawn position
            GameObject instance = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
            launcherBlast_soundEffect.Play();
            Rigidbody2D rb = instance.AddComponent<Rigidbody2D>();

            // Set gravity scale based on the affectedByGravity variable
            rb.gravityScale = affectedByGravity ? 1 : 0;

            // Set the velocity based on the calculated direction
            rb.velocity = direction * speed;

            yield return new WaitForSeconds(2); // Wait for 2 seconds before the next iteration
        }
    }

    }



    


