using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverScreen;
    public GameObject displayLives;   // optional
    public TextMeshProUGUI timerText; // NEW — visible countdown on Canvas

    [Header("Timer")]
    public float timeLimit = 120f; // 2 minutes
    private float timer;
    private bool gameEnded = false;

    void Start()
    {
        timer = timeLimit;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (timerText != null)
            UpdateTimerDisplay();
    }

    void Update()
    {
        if (gameEnded)
            return;

        // Decrease timer
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            Debug.Log("[GameOverManager] Time's up!");
            TriggerGameOver();
        }
        else{
            UpdateTimerDisplay();
        }
        

    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void TriggerGameOver()
    {
        if (gameEnded) return;

        Debug.Log("[GameOverManager] Game over triggered");
        gameEnded = true;
        Time.timeScale = 0f;

        if (displayLives != null)
            displayLives.SetActive(false);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RetryLevel()
    {
        Debug.Log("[GameOverManager] Retry button clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("[GameOverManager] Main Menu button clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
