using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CutsceneJumpscare : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float normalDeerDuration = 3.0f; // How long it acts like a normal deer
    [SerializeField] private float glitchBlinkDuration = 1.2f; // How long it flashes before the scare
    [SerializeField] private float scareDisplayDuration = 2.0f; // How long the Wendigo screams on screen

    [Header("Jumpscare Assets")]
    [SerializeField] private Sprite wendigoScareSprite; // The terrifying Wendigo face/pose sprite
    [SerializeField] private AudioClip scareScreamSFX;  // The heavy audio spike track

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager; // Your level dialogue canvas manager

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Setup a local audio source dynamically if one doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // Kick off the horror sequence immediately when the scene/trigger activates
        StartCoroutine(ExecuteCutscene());
    }

    private IEnumerator ExecuteCutscene()
    {
        // PHASE 1: Normal Behavior
        // The default animation state in your Animator should be the deer idle/look around loop.
        yield return new WaitForSeconds(normalDeerDuration);

        // PHASE 2: The Glitch Blink
        float timeSpentBlinking = 0f;
        float blinkSpeed = 0.08f; // Rapid fire flashing rate

        while (timeSpentBlinking < glitchBlinkDuration)
        {
            // Toggle the visibility off and on to create a creepy visual breakdown
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkSpeed);
            timeSpentBlinking += blinkSpeed;
        }
        spriteRenderer.enabled = true; // Force it visible when entering the scare phase

        // PHASE 3: The Jump Scare!
        // Disable the normal deer animator completely so it doesn't try to overwrite our scare sprite
        if (animator != null) animator.enabled = false;

        // Instantly switch to the Wendigo artwork
        if (wendigoScareSprite != null)
        {
            spriteRenderer.sprite = wendigoScareSprite;
        }

        // Make the sprite huge or flash red for maximum visual impact
        transform.localScale *= 1.3f;
        spriteRenderer.color = Color.red;

        // Blast the jumpscare scream audio
        if (scareScreamSFX != null)
        {
            audioSource.PlayOneShot(scareScreamSFX);
        }

        // Camera Shake Hook (Optional)
        // If you have a camera shake script in your project, trigger it here!

        yield return new WaitForSeconds(scareDisplayDuration);

        // PHASE 4: Transition to Dialogue
        // Clean up the creature (hide it or return it to a passive state)
        spriteRenderer.color = Color.white;
        gameObject.SetActive(false); // Remove the creature from the active screen view

        // Launch the dialogue panel!
        if (dialogueManager != null)
        {
            dialogueManager.StartCutscene();
        }
    }
}