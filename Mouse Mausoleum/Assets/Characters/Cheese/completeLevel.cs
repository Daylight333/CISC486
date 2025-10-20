using UnityEngine;

public class completeLevel : MonoBehaviour {
    
    public GameObject mouseObject;
    public Renderer rend;

    void Start(){
        // Turn cheese colour to white 
        rend = GetComponent<MeshRenderer>();
        rend.material.color = Color.white;
    }

    void OnCollisionEnter(Collision cheeseCollide){
        
        // If the Mouse collides with the cheese make the cheese dissapear
        if (cheeseCollide.gameObject == mouseObject){
            gameObject.SetActive(false);
            
        }
        // Stop the game
        Time.timeScale = 0f;
    }
    
}