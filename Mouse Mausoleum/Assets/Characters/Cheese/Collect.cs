using UnityEngine;

public class Collect : MonoBehaviour {
    
    public GameObject mouseObject;
    public Renderer rend;

    void Start(){
        rend = GetComponent<MeshRenderer>();
        rend.material.color = Color.yellow;
    }

    // If the Mouse collides with the cheese make the cheese dissapear
    void OnCollisionEnter(Collision cheeseCollide){
        
        if (cheeseCollide.gameObject == mouseObject){
            gameObject.SetActive(false);
            
        }
        mouseObject.GetComponent<Health>().gainHealth();
        Debug.Log("Cheese collected and one extra health gained");
    }
    
}
