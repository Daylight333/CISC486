using UnityEngine;

public class Health : MonoBehaviour
{
    private int health;
    private GameOverManager gameOverManager;

    void Start()
    {
        health = 2;
        gameOverManager = FindFirstObjectByType<GameOverManager>();
    }

    void Update()
    {
        if (health <= 0 && gameOverManager != null)
        {
            gameOverManager.TriggerGameOver();
            Destroy(gameObject);
        }
    }

    public int getHealth(){
        return health;
    }

    public void loseHealth()
    {
        this.health -= 1;
        Debug.Log("[Health] Lost 1 health. Current: " + health);
    }

    public void gainHealth()
    {
        this.health += 1;
        Debug.Log("[Health] Gained 1 health. Current: " + health);
    }
}
