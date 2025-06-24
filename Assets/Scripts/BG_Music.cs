using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Music : MonoBehaviour
{
    public AudioSource initialMusic;
    public AudioSource bossBattleMusic;
    public AudioSource victoryMuisc;
    private BossBatlleCinematics bossBattleCinematics;

    void Start()
    {
        // Find the BossBattleCinematics script in the scene
        bossBattleCinematics = FindObjectOfType<BossBatlleCinematics>();

        // Start playing the initial music
        initialMusic.Play();
    }

    void Update()
    {
        // Check if playerHasEntered is true
        if (bossBattleCinematics.playerHasEntered)
        {
            // Stop the initial music and play the boss battle music
            if (initialMusic.isPlaying)
            {
                initialMusic.Stop();
                bossBattleMusic.Play();
            }
        }
    }

    public void stopBGM()
    {
        bossBattleMusic.Stop();
        victoryMuisc.Play();
    }
}
