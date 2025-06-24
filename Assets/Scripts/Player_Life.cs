using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Life : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;
    private Rigidbody2D rb;
    [SerializeField] private float height=-23.0f;
    public bool isAlive = true;
    [SerializeField] private bool isfalling= false;
    public GameObject[] traps;
    [SerializeField] private AudioSource deathSoundEffect;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
      if(transform.position.y <= height)
        {
            isfalling = true;
        }

        if (isfalling)
        {
            die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") || 
            collision.gameObject.CompareTag("Destroyable Enemy"))
        {
            foreach (GameObject trap in traps)
            {
                Spike__Ball spikeBallScript = trap.GetComponent<Spike__Ball>();
                if (spikeBallScript != null && spikeBallScript.trapWorks)
                {
                    die();
                }
            }
        }
    }

    public void die()
    {
        isAlive = false;
        isfalling = false;
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("Death");
        deathSoundEffect.Play();
        StartCoroutine(Restart_Level());
    }

    private IEnumerator Restart_Level()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
