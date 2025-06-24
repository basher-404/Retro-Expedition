using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private AudioSource box_push;
    private Rigidbody2D rb;
    private Vector3 lastPosition;

    // The position of the object in the previous frame


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = rb.position;  
    }

    private float speed=0f;
    // Start is called before the first frame update
    void FixedUpdate()
    {
       speed= (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
     

        if (speed> 0.01)
        {
            if (!box_push.isPlaying)
            {
                box_push.volume = 1f;

                box_push.Play();

            }

        }
        else
            box_push.Pause();
    }
}
