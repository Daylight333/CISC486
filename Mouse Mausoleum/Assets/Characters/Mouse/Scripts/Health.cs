using UnityEngine;

public class Health : MonoBehaviour
{
    private int health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        health = 3;
    }

    // Make sure the game ends when the mouse dies
    public void Update(){
        if (health <= 0f){
            Destroy(gameObject);
        }
    }

    public int getHealth(){
        return health;
    }

    public void loseHealth(){
        this.health -= 1;
    }

    public void gainHealth(){
        this.health += 1;
    }
}
