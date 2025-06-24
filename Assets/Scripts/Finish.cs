using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] private AudioSource finishSound;
    private Animator anim;
    private bool levelCompleted= false;
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            // You can replace "YourAnimationName" with the name of your animation
            anim.speed = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            {
                levelCompleted = true;
                finishSound.Play();
                anim.speed = 1f;
            }
        }
    }

    private void completeLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ 1);
        AdsManager.Instance.interstitialAds.ShowInterstitialAd();
    }
}
