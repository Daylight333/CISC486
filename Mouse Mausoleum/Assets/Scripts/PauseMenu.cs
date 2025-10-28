using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;        // your brown "PAUSED" image or panel
    public GameObject backgroundDimmer;   // semi-transparent black overlay
    public GameObject displayLives;       // "Lives: X" text UI
    public GameObject optionsPanel;       // optional: assign if you have an options menu

    private bool isPaused = false;

    void Update()
    {
        // Toggle pause with Esc key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // === Called when pressing the RESUME button or Esc ===
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        backgroundDimmer.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (displayLives != null) displayLives.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    // === Called when the player presses Esc or clicks the Pause button ===
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        backgroundDimmer.SetActive(true);
        if (displayLives != null) displayLives.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    // === Called when the player clicks the OPTIONS button ===
    public void OpenOptions()
    {
        Debug.Log("Opening options...");
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            pauseMenuUI.SetActive(false);
        }
    }

    // === Called when the player clicks the BACK button in Options ===
    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
    }

    // === Called when the player clicks the EXIT button ===
    public void QuitToMainMenu()
    {
        Debug.Log("Exiting to main menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Change to your actual menu scene name
    }

}