using UnityEngine;

public class Animation : MonoBehaviour
{
    public Animator animator;
    public Transform transformMouse;
    private float moveVal = 0.7f;
    private bool hasMoved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        animator.Play("mixamo_com");
    }

    void Update(){
        if (Input.GetKey(KeyCode.W) || 
            Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || 
            Input.GetKey(KeyCode.D)){
            if (!hasMoved){
                transformMouse.position = new Vector3(transformMouse.position.x,0,transformMouse.position.z);
                hasMoved = true;
            }
            
            animator.Play("JerryRun");
        }
        else {
            if (hasMoved){
                transformMouse.position = new Vector3(transformMouse.position.x,-moveVal,transformMouse.position.z);
                hasMoved = false;
            }
            
            animator.Play("mixamo_com");
        }  
    }
}
