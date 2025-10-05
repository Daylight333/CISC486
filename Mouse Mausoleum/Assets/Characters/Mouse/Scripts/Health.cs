using UnityEngine;

public class Health : MonoBehaviour
{
    private int health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        health = 3;
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
