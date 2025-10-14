using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;

public class CatStates : MonoBehaviour
{
    // FSM, Rigid Body, and basic wall check distance
    private FSM controller = new FSM();
    public Rigidbody rb;
    private float distanceRay = 4f;
    
    // Timers for various states
    private float seekTimer;
    private float attackTimer;
    private float backOffTimer;
    private float movementTimer;
    private float chaseTimer;
    
    // Speeds for different states
    private float patrolMoveSpeed = 3f;
    private float seekMoveSpeed = 1.5f;
    private float chaseMoveSpeed = 5f;
    private float attackMoveSpeed = 8f;

    // Direction vectors, and info for the mouse
    private Vector3 dir;
    private Vector3 previousDir;
    private Vector3 mouseDirection;
    List<Vector3> possibleDir = new List<Vector3> {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
    private float mouseDistance;
    public Transform mouse;
    public GameObject mouseObj;
    private bool mouseHit;

    // Distances to check for each state
    private float seekDistance = 25f;
    private float seekDistanceSqr; // This needs to be the sqaure of the distance you want to check to use the sqrMagnitude function
    private float chaseDistance = 10f;
    private float chaseDistanceSqr;
    private float attackDistance = 5f;
    private float collisionDistance = 1f;

    public NavMeshAgent agent;
    
    
    void Start(){
        // Randomize the possible directions to start in, then pick a direction that doesn't have a wall and go
        List<Vector3> shuffled = possibleDir.OrderBy(x => UnityEngine.Random.value).ToList(); // found this on https://discussions.unity.com/t/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code/535113
        foreach (Vector3 d in shuffled){
            if (!Physics.Raycast(transform.position, d, distanceRay)){
                dir = d;
                previousDir = -dir;
            }
        }

        // Set the initial speed, seek distance and chase distance checks
        agent.speed = patrolMoveSpeed;

        seekDistanceSqr = seekDistance * seekDistance;

        chaseDistanceSqr = chaseDistance * chaseDistance;

        // Set the first state as patrolling
        controller.setState(patrol);
    }

    private void Update(){
        // Update FSM
        controller.Update();
        
        // Check if the mouse still exists, if its dead then go back to patrol
        if (mouse == null){
            controller.setState(patrol);
            agent.speed = patrolMoveSpeed;
        }
    }

    private void FixedUpdate(){
        // Always check for if cat is hitting a wall
        if (Physics.Raycast(transform.position, dir, distanceRay)){
            dir = newDirection();
        }
        if (controller.activeState == (Action)patrol || controller.activeState == (Action)backOff){
            agent.SetDestination(dir + transform.position);
        } else{
            agent.SetDestination(dir);
        }
    }

    public Vector3 newDirection(){
        // If we are in the patrol state then just randomly choose a direction to go in
        if (controller.activeState == (Action)patrol){
            Debug.Log("Hi");
            // Randomize the directions
            List<Vector3> shuffled = possibleDir.OrderBy(x => UnityEngine.Random.value).ToList();

            // Remove the previous direction from the choices so the cat doesn't bounce back and forth
            shuffled.Remove(previousDir);

            // Go through to find a direction that doesn't have a wall, and reset previousDir to the new opposite direction
            foreach (Vector3 d in shuffled){
                if (!Physics.Raycast(transform.position, d, distanceRay)){
                    previousDir = -d;
                    return d;
                }
            }
        } else if (controller.activeState == (Action)seek){

            mouseDirection = mouse.position;

            Vector3 swayAngle = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(0.5f, 0.5f)); // small sway each frame

            // Rotate the direction around the Y-axis
            Vector3 swayedDirection = swayAngle + mouseDirection;

            return swayedDirection;

        } else if (controller.activeState == (Action)chase || controller.activeState == (Action)attack){
            // If we can see the mouse then go towards the mouse
            return mouse.position;
        } else if (controller.activeState == (Action)backOff){
            // If we are backing off to give the mouse some time to react then move backwards
            mouseDirection = (mouse.position - transform.position).normalized;
            return (mouseDirection * -1);
        }
        // If we could not find a new direction then just go back
        previousDir = -previousDir;
        return previousDir; 
    }

    public void patrol(){
        Debug.Log("Patrol");

        // Check if the mouse exists
        if (mouse != null){
            // If the mouse is within the seek distance check if its behind a wall, if not then set the state to seek
            if ((mouse.position - transform.position).sqrMagnitude <= seekDistanceSqr){
                mouseDirection = (mouse.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance)){
                    if (hit.collider.transform == mouse){
                        controller.setState(seek);
                        agent.speed = seekMoveSpeed;
                        dir = newDirection();
                    }
                }
            }
        }
    }

    public void seek(){
        Debug.Log("Seek");

        movementTimer += Time.deltaTime;

        // Check to see the cat can still see the mouse, if the cat sees a wall or the mouse is too far we start the countdown until the cat goes back to patrol
        mouseDirection = (mouse.position - transform.position).normalized;
        mouseHit = Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance);
        if (mouseHit == false || hit.collider.transform != mouse){
            seekTimer += Time.deltaTime;
            dir = newDirection();
        } else {
            seekTimer = 0f;
        }
        // If its been a second change direction slightly to add more dynamic movement
        if (movementTimer >= 1f){
            dir = newDirection();
            movementTimer = 0;
        }
        // If the cat has not been able to see the mouse for more than 5 seconds, go back to patrol
        if (seekTimer >= 10f){
            seekTimer = 0f;
            agent.speed = patrolMoveSpeed;
            controller.setState(patrol);
            dir = newDirection();   
        }
        // If the mouse gets even closer to the cat then the cat starts to chase
        if ((mouse.position - transform.position).sqrMagnitude <= chaseDistanceSqr){
            mouseDirection = (mouse.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, seekDistance)){
                if (hitMouse.collider.transform == mouse){
                    controller.setState(chase);
                    agent.speed = chaseMoveSpeed;
                    dir = newDirection();
                }
            }
        }
    }

    public void chase(){
        Debug.Log("Chase");

        movementTimer += Time.deltaTime;

        // Check to see the cat can still see the mouse, if the cat sees a wall or the mouse is too far we start the countdown until the cat goes back to seeking
        mouseDirection = (mouse.position - transform.position).normalized;
        mouseHit = Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, chaseDistance);
        if (mouseHit == false || hit.collider.transform != mouse){
            chaseTimer += Time.deltaTime;
            dir = newDirection();
        } else {
            chaseTimer = 0f;
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, attackDistance)){
                agent.speed = attackMoveSpeed;
                controller.setState(attack);
            }
        }
        // when the timer finishes change state to patrol, reset our timer
        if (chaseTimer >= 5f){
            chaseTimer = 0f;
            controller.setState(seek);
            agent.speed = seekMoveSpeed;
            dir = newDirection();
        }
        if (movementTimer >= 1f){
            dir = newDirection();
            movementTimer = 0;
        }
    }

    public void attack(){
        Debug.Log("Attack");

        // Recalculate mouse direction and see if mouse is in attack range 
        mouseDirection = (mouse.position - transform.position).normalized;
        
        dir = newDirection();

        //check for collision with the mouse 
        if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, collisionDistance)){
            attackTimer = 0f;
            mouseObj.GetComponent<Health>().loseHealth();
            controller.setState(backOff);
            backOffTimer = 0f;
            agent.speed = patrolMoveSpeed;
        }
        else{ // If the cat did not collide and the mouse is behind a wall or out of line of sight then go back to seeking
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitWall, collisionDistance)){
                if (hitWall.collider.transform != mouse){
                    chaseTimer = 0f;
                    controller.setState(seek);
                    agent.speed = seekMoveSpeed;
                    dir = newDirection();
                }
            }
            attackTimer += Time.deltaTime;
            // If its been more than three seconds without being close to the mouse go back to chase
            if (attackTimer >= 3f){
                agent.speed = chaseMoveSpeed;
                controller.setState(chase);
                dir = newDirection();
            }
        }
    }
    
    public void backOff(){
        Debug.Log("Backoff");//

        // This method allows the mouse to respond to a cat attack and potentially get away
        if (mouse == null){ // If mouse is dead, go back to patrol
            controller.setState(patrol);
            agent.speed = patrolMoveSpeed;
        }
        // Move backwards 
        dir = newDirection();
        backOffTimer += Time.deltaTime;

        // Pause the cat
        if (backOffTimer >= 1f){
            agent.speed = 0f;
            // After 3 seconds go back to attacking
            if (backOffTimer >= 3f){
                agent.speed = attackMoveSpeed;
                controller.setState(attack);
                backOffTimer = 0f;
            }
        }
    }
}
