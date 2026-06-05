using System.Collections;
using UnityEngine;

public class WendigoBrain : MonoBehaviour
{
    [Header("Attack Setup (Drag in Order: Left-Low, Left-High, Right-High, Right-Low)")]
    [SerializeField] private GameObject[] warningSigns;
    [SerializeField] private GameObject[] lightBeams;

    [Header("Timings")]
    [SerializeField] private float waitBetweenStrikes = 7f;
    [SerializeField] private float strikeDuration = 3f;
    [SerializeField] private float blinkTimeBeforeStrike = 1.5f;

    private int currentTargetIndex = 0; // Starts at 0 (Left-Low)

    private void Start()
    {
        // Ensure all beams and warnings are hidden when the scene starts
        foreach (var sign in warningSigns) sign.SetActive(false);
        foreach (var beam in lightBeams) beam.SetActive(false);

        // Start the infinite attack loop
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true) // This makes the pattern loop forever
        {
            // 1. Wait for 7 seconds
            yield return new WaitForSeconds(waitBetweenStrikes);

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
}