using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTraps : MonoBehaviour
{
    public GameObject objectToAppear;

    private void Start()
    {
        objectToAppear.SetActive(false);    
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            objectToAppear.SetActive(true);
        }
    }
}
