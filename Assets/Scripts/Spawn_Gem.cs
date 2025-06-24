using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Gem : MonoBehaviour
{
    [SerializeField] private GameObject gemPrefab; // Assignable in Inspector

    public void SpawnObject()
    {
        if (gemPrefab != null)
        {
            // Instantiate the gem
            GameObject gem = Instantiate(gemPrefab, transform.position, Quaternion.identity);

            // Apply a small force to push it to the right
            Rigidbody2D gemRb = gem.GetComponent<Rigidbody2D>();
            if (gemRb != null)
            {
                gemRb.AddForce(new Vector2(2.8f, 3.5f), ForceMode2D.Impulse); // Adjust force values as needed
            }
            else
            {
                Debug.LogWarning("Rigidbody2D not found on gemPrefab!");
            }
        }
        else
        {
            Debug.LogWarning("gemPrefab not assigned for " + gameObject.name);
        }
    }
}
