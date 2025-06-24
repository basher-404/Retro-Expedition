using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button exitButton;

    void Start()
    {
        // Add a listener to the button's click event
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

   public  void OnExitButtonClick()
    {
        // Close the application
        Application.Quit();
    }
}
