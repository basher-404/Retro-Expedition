using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionButtonManager : MonoBehaviour
{
    public Button interactionButton; // Reference to the UI Button
    public PlayerCollision playerCollision;
    public bool isButtonPressed= false;

    void Start()
    {
        // Initially disable the button
        interactionButton.gameObject.SetActive(false);
        interactionButton.onClick.AddListener(OnButtonPressed);
    }

    void Update()
    {
        // Enable or disable the button based on Player_in_Range
        if(playerCollision.isNearLever || playerCollision.isNearShaft)
        {
            interactionButton.gameObject.SetActive(true);
        }

        else
        {
            interactionButton.gameObject.SetActive(false);
        }
       
    }

    void OnButtonPressed() //used by shaft and lever
    {
        isButtonPressed = true;
    }

    public void ButtonLifted()  //disable button after use
    {
        isButtonPressed = false;
    }

}
