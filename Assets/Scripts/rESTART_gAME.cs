using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rESTART_gAME : MonoBehaviour
{
    public void restartGame()
    {
        SceneManager.LoadScene(1); // Assuming the main menu is the first scene (index 0)
    }
}
