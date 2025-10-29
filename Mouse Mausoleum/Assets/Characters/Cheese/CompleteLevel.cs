using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    [Header("References")]
    public GameObject mouseObject;       // the player
    public GameObject levelCompleteUI;   // your “Level Complete” screen
    public Renderer rend;                // optional cheese visual

    private bool levelFinished = false;

    void Start()
    {
        // Make the cheese stand out (optional visual)
        rend = GetComponent<MeshRenderer>();
        rend.material.color = Color.white;

        // Make sure UI starts hidden
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the mouse collided with the cheese
        if (!levelFinished && collision.gameObject == mouseObject)
        {
            Debug.Log("[CompleteLevel] Mouse reached the cheese! Level complete.");
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        levelFinished = true;
        Time.timeScale = 0f; // pause gameplay

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // === Called by the CONTINUE button ===
    public void ContinueToNextLevel()
    {
        Debug.Log("[CompleteLevel] Continue button clicked — loading next level.");

        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Check if a next scene exists in Build Settings
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("[CompleteLevel] No next scene found — returning to Main Menu.");
            SceneManager.LoadScene("MainMenu");
        }
    }

    // === Called by the MAIN MENU button ===
    public void ReturnToMainMenu()
    {
        Debug.Log("[CompleteLevel] Main Menu button clicked — returning to Main Menu.");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
