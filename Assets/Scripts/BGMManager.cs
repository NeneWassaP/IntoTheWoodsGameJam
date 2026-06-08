using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;

    [System.Serializable]
    public struct SceneTrack
    {
        public string sceneName; // Exact name of your scene file
        public AudioClip audioClip; // The music file for this scene
    }

    [Header("Tracks Configuration")]
    [SerializeField] private SceneTrack[] tracks;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern: Ensures only ONE music manager ever exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 🌟 Tells Unity never to destroy this object when changing scenes
            audioSource = GetComponent<AudioSource>();

            // Force audio source to loop background tracks
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject); // Delete duplicates that spawn when re-entering the intro scene
            return;
        }
    }

    private void OnEnable()
    {
        // Tell Unity to notify this script whenever a scene finishes loading
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateBackgroundMusic(scene.name);
    }

    private void UpdateBackgroundMusic(string sceneName)
    {
        foreach (var track in tracks)
        {
            if (track.sceneName == sceneName)
            {
                // Safety: If this track is already playing, let it keep playing without resetting!
                if (audioSource.clip == track.audioClip) return;

                audioSource.clip = track.audioClip;
                audioSource.Play();
                return;
            }
        }

        // If a scene isn't listed in the array (like a secret room), stop the music
        audioSource.Stop();
    }
}