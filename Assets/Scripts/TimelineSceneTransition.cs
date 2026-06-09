using UnityEngine;
using UnityEngine.Playables; // 🌟 Required to talk to the Timeline system
using UnityEngine.SceneManagement;

public class TimelineSceneTransition : MonoBehaviour
{
    [Header("Timeline Setup")]
    [SerializeField] private PlayableDirector playableDirector; // Drag your object with the Timeline here

    [Header("Scene Transition Setup")]
    [SerializeField] private string nextSceneName; // Exact case-sensitive name of the scene to load

    private void OnEnable()
    {
        if (playableDirector != null)
        {
            // 🌟 Tells the script to wait until the timeline finishes playing
            playableDirector.stopped += OnTimelineFinished;
        }
    }

    private void OnDisable()
    {
        if (playableDirector != null)
        {
            // Unsubscribe when disabled to keep memory clean
            playableDirector.stopped -= OnTimelineFinished;
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        // Safety check to ensure the matching timeline asset is the one that just ended
        if (director == playableDirector)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("You forgot to type the Next Scene Name in the Inspector!");
            }
        }
    }
}