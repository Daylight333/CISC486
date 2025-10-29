using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;        // your brown "PAUSED" panel
    public GameObject backgroundDimmer;   // black transparent overlay
    public GameObject displayLives;       // "Lives: X" text UI
    public GameObject optionsPanel;       // optional: assign if you have one

    private bool isPaused = false;

    void Start()
    {
        Debug.Log("[PauseMenu] Start() called — initializing menu state");

        // Ensure the menu starts hidden and game unpaused
        HideAllMenus();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;

        Debug.Log("[PauseMenu] Game started unpaused, cursor hidden");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[PauseMenu] ESC pressed — toggling pause");
            TogglePause();
        }

#if UNITY_EDITOR
        // Safety: prevent the Unity editor itself from being paused accidentally
        if (UnityEditor.EditorApplication.isPaused)
        {
            Debug.LogWarning("[PauseMenu] Unity Editor was paused — resuming Editor");
            UnityEditor.EditorApplication.isPaused = false;
        }
#endif
    }

    // === Resume button ===
    public void ResumeGame()
    {
        Debug.Log("[PauseMenu] Resume button clicked");
        ResumeGameplayInternal();
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            Debug.Log("[PauseMenu] Game is paused — resuming");
            ResumeGameplayInternal();
        }
        else
        {
            Debug.Log("[PauseMenu] Game is running — pausing");
            PauseGameplayInternal();
        }
    }

    private void ResumeGameplayInternal()
    {
        Debug.Log("[PauseMenu] Resuming gameplay");

        isPaused = false;
        Time.timeScale = 1f;

        HideAllMenus();

        if (displayLives != null)
        {
            displayLives.SetActive(true);
            Debug.Log("[PauseMenu] Lives display enabled");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#endif

        Debug.Log("[PauseMenu] Game resumed successfully");
    }

    private void PauseGameplayInternal()
    {
        Debug.Log("[PauseMenu] Pausing gameplay");

        isPaused = true;
        Time.timeScale = 0f;

        if (backgroundDimmer != null)
        {
            backgroundDimmer.SetActive(true);
            Debug.Log("[PauseMenu] Background dimmer shown");
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            Debug.Log("[PauseMenu] Pause menu UI shown");
        }

        if (displayLives != null)
        {
            displayLives.SetActive(false);
            Debug.Log("[PauseMenu] Lives display hidden");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[PauseMenu] Game paused successfully");
    }

    // === Utility: hides all menu elements safely ===
    private void HideAllMenus()
    {
        Debug.Log("[PauseMenu] Hiding all menus");

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Debug.Log("[PauseMenu] Pause menu UI hidden");
        }

        if (backgroundDimmer != null)
        {
            backgroundDimmer.SetActive(false);
            Debug.Log("[PauseMenu] Background dimmer hidden");
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            Debug.Log("[PauseMenu] Options panel hidden");
        }
    }

    // === Buttons ===
    public void OpenOptions()
    {
        Debug.Log("[PauseMenu] Options button clicked");

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(false);

            Debug.Log("[PauseMenu] Options panel opened");
        }
        else
        {
            Debug.LogWarning("[PauseMenu] No options panel assigned");
        }
    }

    public void CloseOptions()
    {
        Debug.Log("[PauseMenu] Close Options button clicked");

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Debug.Log("[PauseMenu] Returned from options to pause menu");
    }

    public void QuitToMainMenu()
    {   
        Debug.Log("[PauseMenu] Exit to Main Menu button clicked");

        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
        {
            Debug.Log("[PauseMenu] Loading scene: MainMenu");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogError("[PauseMenu] ERROR — Scene 'MainMenu' not found in Build Settings!");
        }
    }

    public void QuitGame()
    {
        Debug.Log("[PauseMenu] Quit Game button clicked — exiting application");

        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("[PauseMenu] Application.Quit() called — won't close Editor");
#endif
    }
}
