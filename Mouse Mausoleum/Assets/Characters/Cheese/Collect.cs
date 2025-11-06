using UnityEngine;

public class Collect : MonoBehaviour {
    
    public GameObject mouseObject;
    public Renderer rend;

    void Start(){

        // Turn the cheese blue
        rend = GetComponent<MeshRenderer>();
        rend.material.color = Color.blue;
    }

    void OnCollisionEnter(Collision cheeseCollide){
        
        // If the Mouse collides with the cheese make the cheese dissapear
        if (cheeseCollide.gameObject == mouseObject){
            gameObject.SetActive(false);
            
        }
        // Make the mouse gain health
        mouseObject.GetComponent<Health>().gainHealth();
    }
    
}
