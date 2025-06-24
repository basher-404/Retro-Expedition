using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever_Activation : MonoBehaviour
{
    private Animator anim;
    public bool Player_in_Range = false;

    public GameObject launcher;
    public float rotationSpeed;
    [SerializeField] private AudioSource leverActivate_soundEffect;
    [SerializeField] private BoxCollider2D boxC;
    private bool isActivated = false; // Flag to check if lever is activated

    //Android UI Button
    public Button yourUIButton; // Reference to your UI Button
    public InteractionButtonManager interact;

    void Start()
    {
        anim = GetComponent<Animator>();
        boxC = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(Player_in_Range && Input.GetKeyDown("x") || Player_in_Range && interact.isButtonPressed && !isActivated)
        {
            ActivateLever();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        checkRange(collision.gameObject.transform);
    }

    void checkRange(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Player") )
        {
            Player_in_Range = true;
        }
    }

    public void ActivateLever()
    {
        anim.SetBool("Contact", true);
        leverActivate_soundEffect.Play();
        launcher.transform.Rotate(new Vector3(0, 0, -rotationSpeed));
        Destroy(boxC);
        isActivated = true;
        interact.ButtonLifted();
    }
}
