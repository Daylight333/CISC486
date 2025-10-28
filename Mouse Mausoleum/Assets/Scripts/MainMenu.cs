using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // for hover detection

public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // === Button Actions ===

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptions()
    {
        Debug.Log("Options button clicked!");
        // You can show your options panel here later
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game pressed");
        Application.Quit(); // Works only in builds
    }

    // === Cursor Handling ===

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Switch to the system hand cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Keep or reset to default cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
