using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Automatically ensures a SpriteRenderer is attached
public class WendigoBrain : MonoBehaviour
{
    [Header("Attack Setup (Drag in Order: Left-Low, Left-High, Right-High, Right-Low)")]
    [SerializeField] private GameObject[] warningSigns;
    [SerializeField] private GameObject[] lightBeams;

    // 🌟 NEW: Array to hold your 4 custom direction sprites
    [SerializeField] private Sprite[] wendigoSprites;

    [Header("Timings")]
    [SerializeField] private float waitBetweenStrikes = 7f;
    [SerializeField] private float strikeDuration = 3f;
    [SerializeField] private float blinkTimeBeforeStrike = 1.5f;

    private SpriteRenderer spriteRenderer; // 🌟 Cache component
    private int currentTargetIndex = 0; // Starts at 0 (Left-Low)

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure all beams and warnings are hidden when the scene starts
        foreach (var sign in warningSigns) sign.SetActive(false);
        foreach (var beam in lightBeams) beam.SetActive(false);

        // Set the initial sprite based on the starting target index (Left-Low)
        UpdateWendigoSprite();

        // Start the infinite attack loop
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true) // This makes the pattern loop forever
        {
            // 1. Wait for 7 seconds
            yield return new WaitForSeconds(waitBetweenStrikes);

            // 🌟 UPDATE: Change the Wendigo's stance/pose right as the warning starts
            // This gives players a visual cue from the monster itself before the beam appears!
            UpdateWendigoSprite();

            // 2. Blinking Warning Phase
            GameObject activeWarning = warningSigns[currentTargetIndex];
            float timeSpentBlinking = 0f;

            // Rapidly toggle the warning object on and off
            while (timeSpentBlinking < blinkTimeBeforeStrike)
            {
                activeWarning.SetActive(!activeWarning.activeSelf); // Flip state
                yield return new WaitForSeconds(0.15f); // Fast blink speed
                timeSpentBlinking += 0.15f;
            }
            activeWarning.SetActive(false); // Make sure it ends completely off

            // 3. Strike Phase! (Beam is active for 3 seconds)
            GameObject activeBeam = lightBeams[currentTargetIndex];
            activeBeam.SetActive(true);

            yield return new WaitForSeconds(strikeDuration);

            activeBeam.SetActive(false);

            // 4. Move to the next position in the list
            currentTargetIndex++;

            // If we reached the end of our 4 positions, loop back to the first one (0)
            if (currentTargetIndex >= 4)
            {
                currentTargetIndex = 0;
            }
        }
    }

    // 🌟 NEW: Helper method to safely change the sprite based on the current index
    private void UpdateWendigoSprite()
    {
        if (spriteRenderer != null && wendigoSprites != null && currentTargetIndex < wendigoSprites.Length)
        {
            if (wendigoSprites[currentTargetIndex] != null)
            {
                spriteRenderer.sprite = wendigoSprites[currentTargetIndex];
            }
        }
    }
}