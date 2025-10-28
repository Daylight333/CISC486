using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        // Hide and lock the cursor to the center of the screen when gameplay starts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Optional: helper methods if you want to show/hide later
    public static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
