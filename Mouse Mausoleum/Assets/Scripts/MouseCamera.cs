using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamera : MonoBehaviour {
    public Vector2 turn;

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        
        turn.x += Input.GetAxis("Mouse X");
        transform.localRotation = Quaternion.Euler(0,turn.x,0);
    }
}
