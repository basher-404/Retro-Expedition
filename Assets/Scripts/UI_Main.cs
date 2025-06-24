using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Main : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject mobileControlsUI;
    public AudioSource bgm; 
    private bool isPaused = false;
    private float originalVolume;

    void Start()
    {
        if (!pauseMenuUI.activeSelf)
        {
            pauseMenuUI.SetActive(false);
        }

        // Store the original volume of the BGM
        if (bgm != null)
        {
            originalVolume = bgm.volume;
        }
    }

    void Update()
    {
        // Check for Escape key press (for PC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Check for Back button press (for Android)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        pauseMenuUI.gameObject.SetActive(true);
        mobileControlsUI.gameObject.SetActive(false);

        if (isPaused)
        {
            Time.timeScale = 1.0f;
            isPaused = false;
        }

        if (!isPaused)
        {
            Time.timeScale = 0.0f;
            isPaused = true;
        }

        // Adjust the BGM volume
        if (bgm != null)
        {
            bgm.volume = isPaused ? originalVolume * 0.1f : originalVolume;
        }
    }

    // Function to continue the game
    public void ContinueGame()
    {
        isPaused = false;
        pauseMenuUI.gameObject.SetActive(false);
        mobileControlsUI.gameObject.SetActive(true);
        Time.timeScale = 1;

        // Restore the BGM volume
        if (bgm != null)
        {
            bgm.volume = originalVolume;
        }
    }

    // Function to load the main menu
    public void LoadMainMenu()
    {
        Time.timeScale = 1; // Ensure the game is not paused
        SceneManager.LoadScene(0); // Assuming the main menu is the first scene (index 0)
    }

    // Function to exit the game
    public void ExitGame()
    {
        Application.Quit();
    }
}
