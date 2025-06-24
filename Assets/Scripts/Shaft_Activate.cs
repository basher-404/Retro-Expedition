using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shaft_Activate : MonoBehaviour
{
    public GameObject fallingPlatform;
    public GameObject waypoint;
    public float speed = 10f;
    private Animator anim;
    public bool Player_in_Range = false;
    public string animationName = "Falling"; // Name of the animation
    private bool isActivated = false;
    private Animator animator;
    [SerializeField] private AudioSource shaftActivate_soundEffect;
    [SerializeField] private AudioSource platform_stop__soundEffect;
    [SerializeField] private BoxCollider2D boxC;

    //Android UI Button
    public Button yourUIButton; // Reference to your UI Button
    public InteractionButtonManager interact;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        boxC = GetComponent<BoxCollider2D>();
        animator = fallingPlatform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player_in_Range && Input.GetKeyDown("x") || Player_in_Range && interact.isButtonPressed)
        {
            isActivated = true;
            shaftActivate_soundEffect.Play();
        }

        if (isActivated == true && Vector2.Distance(fallingPlatform.transform.position, waypoint.transform.position) > 0.01f)
        {
            anim.SetBool("Activate", true);
            animator.SetBool("Activate", true);
            fallingPlatform.transform.position = Vector2.MoveTowards(fallingPlatform.transform.position, waypoint.transform.position, speed * Time.deltaTime);
            Destroy(boxC);
            interact.ButtonLifted();
        }
        else
        {
            animator.SetBool("Activate", false);
            platform_stop__soundEffect.Play();
        }


    }

 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        checkRange(collision.gameObject.transform);
    }

    void checkRange(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Player"))
        {
            Player_in_Range = true;
        }
    }

}
