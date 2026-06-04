using UnityEngine;
using UnityEngine.SceneManagement; // REQUIRED: Allows us to load and reload scenes
using UnityEngine.InputSystem;    // REQUIRED: For checking the modern keyboard inputs

public class StageResetter : MonoBehaviour
{
    private void Update()
    {
        // Listen for the R key being pressed down
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetCurrentStage();
        }
    }

    public void ResetCurrentStage()
    {
        // 1. Find out exactly which scene is currently open right now
        Scene activeScene = SceneManager.GetActiveScene();

        // 2. Tell Unity to reload it instantly
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}