using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        // Hide and lock the cursor to the center of the screen when gameplay starts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
