using UnityEngine;

public class CameraCursorLock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make cursor invisible and lock to middle 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
