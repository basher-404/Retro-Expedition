using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blow_Torch : MonoBehaviour
{
    [Header("Assign the Spike Ball Prefab (its tag is used for collision detection!)")]
    public GameObject spikeBallPrefab;

    [Header("Particle Effects")]
    [Tooltip("This should be the continuously playing fire effect.")]
    public ParticleSystem fireEffect;
    [Tooltip("This is the electricity effect that plays for 2 seconds upon collision.")]
    public ParticleSystem electricityEffect;
    [Tooltip("This is the implosion effect that plays after electricity.")]
    public ParticleSystem implosionEffect;
    [Tooltip("This is the impact effect that plays after electricity.")]
    public ParticleSystem explosionEffect;

    [Header("Timings")]
    [Tooltip("Duration over which the fire effect shrinks after hit until blowtorch is destroyed.")]
    public float fireShrinkDuration = 2f;

    [Header("Components of Blow Torch")]
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    [Header("Sound Effects")]
    public AudioSource electricitySound;
    public AudioSource implosionSound;
    public AudioSource explosionSound;

    // Flag to ensure the destruction sequence only triggers once.
    private bool isDestroyed = false;

    void Start()
    {
        // Start the fire particle effect if it is not already looping.
        if (fireEffect && !fireEffect.isPlaying)
        {
            fireEffect.Play();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();    
    }

    // Either use OnCollisionEnter2D or OnTriggerEnter2D depending on your collider setup.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ensure this sequence only happens once and that the colliding object's tag matches
        // the spike ball prefab's tag.
        if (!isDestroyed && spikeBallPrefab && collision.gameObject.CompareTag(spikeBallPrefab.tag))
        {
            isDestroyed = true;
            StartCoroutine(BlowtorchDestroySequence());
        }
    }


    IEnumerator BlowtorchDestroySequence()
    {
        // Start simultaneously shrinking the fire effect.
        StartCoroutine(ShrinkFireEffect(fireShrinkDuration));

        // Play the electricity effect to indicate destruction    
        electricityEffect.gameObject.SetActive(true);
        electricityEffect.Play();
        electricitySound.Play();
        
        //Wait to play implosion effect
        yield return new WaitForSeconds(1f);
        implosionEffect.gameObject.SetActive(true);
        implosionEffect.Play();
        implosionSound.Play();
       
        yield return new WaitForSeconds(0.5f);
        explosionSound.Play();
        yield return new WaitForSeconds(0.5f);

        // Stop the continuous fire effect (if desired)
        if (fireEffect)
        {
            fireEffect.Stop();
        }
        
        // Play explosion effect
        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();
       

        // Set the alpha value to 0 (fully transparent) and turn off the boxcollider along with the effects
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        boxCollider.enabled = false;
        fireEffect.Stop();
        electricityEffect.Stop();
        implosionEffect.Stop();

        yield return new WaitForSeconds(2f);
        Destroy(transform.parent.gameObject); 
    }

    /// <summary>
    /// Gradually scales down the fire effect's transform over the given duration.
    /// </summary>
    /// <param name="duration">How long the scaling takes until the fire effect is effectively invisible.</param>
    IEnumerator ShrinkFireEffect(float duration)
    {
        if (fireEffect == null)
            yield break;

        Vector3 startScale = fireEffect.transform.localScale;
        float elapsed = 0f;

        // Continuously reduce the scale over the duration.
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fireEffect.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        // Ensure the scale is exactly zero when finished.
        fireEffect.transform.localScale = Vector3.zero;
    }
}
