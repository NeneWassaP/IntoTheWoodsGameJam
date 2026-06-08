using System.Collections;
using UnityEngine;

public class CutsceneJumpscare : MonoBehaviour
{
    [Header("Target Objects")]
    [SerializeField] private GameObject deerObject;
    [SerializeField] private GameObject gunMarkObject;
    [SerializeField] private GameObject blackFlashObject;
    [SerializeField] private GameObject wendigoObject;

    [Header("Timings")]
    [SerializeField] private float normalDeerDuration = 3.0f;
    [SerializeField] private float targetAimDelay = 0.8f;
    [SerializeField] private float flashDuration = 1.2f;
    [SerializeField] private float scareDisplayDuration = 2.0f;

    [Header("Background Overlay Settings")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Color scareBackgroundColor = new Color(0.5f, 0f, 0f, 1f);

    [Header("Audio Clips")]
    [SerializeField] private AudioClip gunshotSFX;
    [SerializeField] private AudioClip scareScreamSFX;

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    // Changed to private so it safely generates on THIS object, preventing cutoff errors
    private AudioSource localAudioSource;
    private Color originalBackgroundColor = Color.white;

    private void Awake()
    {
        if (backgroundRenderer != null)
        {
            originalBackgroundColor = backgroundRenderer.color;
        }

        if (deerObject != null) deerObject.SetActive(true);
        if (gunMarkObject != null) gunMarkObject.SetActive(true);
        if (blackFlashObject != null) blackFlashObject.SetActive(false);
        if (wendigoObject != null) wendigoObject.SetActive(false);

        // 🌟 AUTOMATIC AUDIO CHANNEL SETUP
        // This attaches the audio player directly to the Cutscene Manager object.
        // Because the Manager stays alive the whole time, the audio can never be cut off!
        localAudioSource = GetComponent<AudioSource>();
        if (localAudioSource == null)
        {
            localAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Force it to 2D sound so distance/camera placement doesn't make it silent
        localAudioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        StartCoroutine(ExecuteCutscene());
    }

    private IEnumerator ExecuteCutscene()
    {
        // PHASE 1: Deer Idling
        yield return new WaitForSeconds(normalDeerDuration);

        // PHASE 2: Tense holding delay
        yield return new WaitForSeconds(targetAimDelay);

        // Play Gunshot
        if (localAudioSource != null && gunshotSFX != null)
        {
            localAudioSource.PlayOneShot(gunshotSFX);
        }

        // PHASE 3: The Black Strobe Flash
        if (blackFlashObject != null)
        {
            float timeSpentFlashing = 0f;
            float flashSpeed = 0.07f;

            while (timeSpentFlashing < flashDuration)
            {
                blackFlashObject.SetActive(!blackFlashObject.activeSelf);
                yield return new WaitForSeconds(flashSpeed);
                timeSpentFlashing += flashSpeed;
            }
            blackFlashObject.SetActive(false);
        }

        // PHASE 4: The Jump Scare Swap!
        if (deerObject != null) deerObject.SetActive(false); // Safe to disable now!
        if (gunMarkObject != null) gunMarkObject.SetActive(false);

        if (wendigoObject != null) wendigoObject.SetActive(true);

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = scareBackgroundColor;
        }

        // 🌟 Play the Scream (Completely safe from being cut off now)
        if (localAudioSource != null && scareScreamSFX != null)
        {
            localAudioSource.PlayOneShot(scareScreamSFX);
        }

        yield return new WaitForSeconds(scareDisplayDuration);

        // PHASE 5: Transition to Dialogue
        if (wendigoObject != null) wendigoObject.SetActive(false);

        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = originalBackgroundColor;
        }

        if (dialogueManager != null)
        {
            dialogueManager.StartCutscene();
        }
    }
}