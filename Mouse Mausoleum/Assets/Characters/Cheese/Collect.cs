using UnityEngine;

public class Collect : MonoBehaviour {
    
    public GameObject targetObject;

    // If the Mouse collides with the cheese make the cheese dissapear
    void OnCollisionEnter(Collision cheeseCollide){
        
        if (cheeseCollide.gameObject == targetObject){
            gameObject.SetActive(false);
            Debug.Log("test");
        }
    }
    
}
