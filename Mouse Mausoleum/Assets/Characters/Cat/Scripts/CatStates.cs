using UnityEngine;

public class CatStates : MonoBehaviour
{
    private FSM controller = new FSM();

    private float timer = 0f;
    private float changeDirection = 4f;

    private float moveSpeed = 2f;
    private Vector3 dir;

    public Rigidbody rb;
    //test
    void Start()
    {
        dir = new Vector3(Random.Range(-1f,1f), 0, Random.Range(1f,-1f));
        controller.setState(patrol);
    }

    private void Update(){
        controller.Update();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    public void patrol(){
        timer += Time.deltaTime;

        if (timer >= changeDirection){
            timer = 0f;
            dir = new Vector3(Random.Range(-1f,1f), 0, Random.Range(1f,-1f));
        }

    }

    public void seek(){
        
    }

    public void chase(){
        
    }

    public void attack(){
        
    }
}
