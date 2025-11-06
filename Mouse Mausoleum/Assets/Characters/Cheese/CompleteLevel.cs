using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    [Header("References")]
    public GameObject mouseObject;       // the player
    public GameObject levelCompleteUI;   // your “Level Complete” screen
    public Renderer rend;                // optional visual, e.g. cheese glow

    private bool levelFinished = false;

    void Start()
    {
        // Safely try to find a renderer, but don't crash if missing
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white;
        }
        else
        {
            Debug.LogWarning("[CompleteLevel] No Renderer found on " + gameObject.name + ". Skipping visual setup.");
        }

        // Hide the "Level Complete" UI at the start
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);
        else
            Debug.LogWarning("[CompleteLevel] Level Complete UI not assigned!");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!levelFinished && collision.gameObject == mouseObject)
        {
            Debug.Log("[CompleteLevel] Mouse reached the goal!");
            FinishLevel();
        }
    }

    private void FinishLevel()
    {
        levelFinished = true;
        Time.timeScale = 0f; // pause gameplay

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);
        else
            Debug.LogError("[CompleteLevel] Missing reference to Level Complete UI!");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // === CONTINUE button ===
    public void ContinueToNextLevel()
    {
        Debug.Log("[CompleteLevel] Continue button clicked — loading next level.");
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("[CompleteLevel] No next scene — returning to Main Menu.");
            SceneManager.LoadScene("MainMenu");
        }
    }

    // === MAIN MENU button ===
    public void ReturnToMainMenu()
    {
        Debug.Log("[CompleteLevel] Main Menu button clicked — returning to Main Menu.");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
