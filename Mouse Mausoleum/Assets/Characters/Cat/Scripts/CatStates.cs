using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

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
    private float actualMoveSpeed;
    private float patrolMoveSpeed = 3f;
    private float seekMoveSpeed = 1.5f;
    private float chaseMoveSpeed = 5f;
    private float attackMoveSpeed = 8f;

    // Direction vectors, and info for the mouse
    private Vector3 dir;
    private Vector3 previousDir;
    private Vector3 mouseDirection;
    List<Vector3> possibleDir = new List<Vector3> {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
    private float randomness = 0.75f;
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
        actualMoveSpeed = patrolMoveSpeed;

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
            actualMoveSpeed = patrolMoveSpeed;
        }
    }

    private void FixedUpdate(){
        // Always check for if cat is hitting a wall
        if (Physics.Raycast(transform.position, dir, distanceRay)){
            dir = newDirection();
        }
        rb.MovePosition(rb.position + dir * actualMoveSpeed * Time.deltaTime);
    }

    public Vector3 newDirection(){
        // If we are in the patrol state then just randomly choose a direction to go in
        if (controller.activeState == (Action)patrol){
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
            // Get cardinal direction of mouse to make wall detection and direction choice more dynamic to where the mouse is
            mouseDirection = (mouse.position - transform.position).normalized;
            Vector3 primaryDir;
            Vector3 secondaryDir;

            if (Mathf.Abs(mouseDirection.x) > Mathf.Abs(mouseDirection.z)){
                primaryDir = (mouseDirection.x > 0) ? Vector3.right : Vector3.left;
                secondaryDir = (mouseDirection.z > 0) ? Vector3.forward : Vector3.back;
            } else {
                primaryDir = (mouseDirection.z > 0) ? Vector3.forward : Vector3.back;
                secondaryDir = (mouseDirection.x > 0) ? Vector3.right : Vector3.left;
            }

            // First check if you can keep going in the cardinal direction of the mouse, and add some randomness to the movement to make it seem more natural
            mouseHit = Physics.Raycast(transform.position, primaryDir, out RaycastHit hitPrimary, distanceRay);
            if ((mouseHit == false || hitPrimary.collider.transform == mouse) && primaryDir != previousDir){
                // Add some random offset to make it seem like the cat is not sure where the mouse is
                primaryDir += new Vector3(UnityEngine.Random.Range(-randomness, randomness), 0, UnityEngine.Random.Range(-randomness, randomness));
                return primaryDir.normalized;
            }

            // Then if we cannot go in the cardinal direction of the mouse go in the next best direction the mouse is in, for example if the mouse is south east and prodominantly south, but south has a wall in the way then go east 
            mouseHit = Physics.Raycast(transform.position, secondaryDir, distanceRay);
            if (mouseHit == false) {
                return secondaryDir.normalized;
            }
        } else if (controller.activeState == (Action)chase || controller.activeState == (Action)attack){
            // If we can see the mouse then go towards the mouse
            mouseDirection = (mouse.position - transform.position).normalized;
            mouseHit = Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit);
            if (hit.collider.transform == mouse){
                mouseDirection = (mouse.position - transform.position).normalized;
                return mouseDirection;
            } else {
                // Get cardinal direction of mouse to make wall detection and direction choice more dynamic to where the mouse is
                mouseDirection = (mouse.position - transform.position).normalized;
                Vector3 primaryDir;
                Vector3 secondaryDir;

                if (Mathf.Abs(mouseDirection.x) > Mathf.Abs(mouseDirection.z)){
                    primaryDir = (mouseDirection.x > 0) ? Vector3.right : Vector3.left;
                    secondaryDir = (mouseDirection.z > 0) ? Vector3.forward : Vector3.back;
                } else {
                    primaryDir = (mouseDirection.z > 0) ? Vector3.forward : Vector3.back;
                    secondaryDir = (mouseDirection.x > 0) ? Vector3.right : Vector3.left;
                }

                // First check if you can keep going in the cardinal direction of the mouse, and add some randomness to the movement to make it seem more natural
                mouseHit = Physics.Raycast(transform.position, primaryDir, out RaycastHit hitPrimary, distanceRay);
                if ((mouseHit == false || hitPrimary.collider.transform == mouse) && primaryDir != previousDir){
                    // Add some random offset to make it seem like the cat is not sure where the mouse is
                    return primaryDir.normalized;
                }

                // Then if we cannot go in the cardinal direction of the mouse go in the next best direction the mouse is in, for example if the mouse is south east and prodominantly south, but south has a wall in the way then go east 
                mouseHit = Physics.Raycast(transform.position, secondaryDir, distanceRay);
                if (mouseHit == false) {
                    return secondaryDir.normalized;
                }
            }
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
        // Check if the mouse exists
        if (mouse != null){
            // If the mouse is within the seek distance check if its behind a wall, if not then set the state to seek
            if ((mouse.position - transform.position).sqrMagnitude <= seekDistanceSqr){
                mouseDirection = (mouse.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance)){
                    if (hit.collider.transform == mouse){
                        controller.setState(seek);
                        actualMoveSpeed = seekMoveSpeed;
                        dir = newDirection();
                    }
                }
            }
        }
    }

    public void seek(){
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
            actualMoveSpeed = patrolMoveSpeed;
            controller.setState(patrol);
            dir = newDirection();   
        }
        // If the mouse gets even closer to the cat then the cat starts to chase
        if ((mouse.position - transform.position).sqrMagnitude <= chaseDistanceSqr){
            mouseDirection = (mouse.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, seekDistance)){
                if (hitMouse.collider.transform == mouse){
                    controller.setState(chase);
                    actualMoveSpeed = chaseMoveSpeed;
                    dir = newDirection();
                }
            }
        }
    }

    public void chase(){
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
                actualMoveSpeed = attackMoveSpeed;
                controller.setState(attack);
            }
        }
        // when the timer finishes change state to patrol, reset our timer
        if (chaseTimer >= 5f){
            chaseTimer = 0f;
            controller.setState(seek);
            actualMoveSpeed = seekMoveSpeed;
            dir = newDirection();
        }
        if (movementTimer >= 1f){
            dir = newDirection();
            movementTimer = 0;
        }
    }

    public void attack(){
        
        // Recalculate mouse direction and see if mouse is in attack range 
        mouseDirection = (mouse.position - transform.position).normalized;
        
        dir = newDirection();

        //check for collision with the mouse 
        if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, collisionDistance)){
            attackTimer = 0f;
            mouseObj.GetComponent<Health>().loseHealth();
            controller.setState(backOff);
            backOffTimer = 0f;
            actualMoveSpeed = patrolMoveSpeed;
        }
        else{ // If the cat did not collide and the mouse is behind a wall or out of line of sight then go back to seeking
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitWall, collisionDistance)){
                if (hitWall.collider.transform != mouse){
                    chaseTimer = 0f;
                    controller.setState(seek);
                    actualMoveSpeed = seekMoveSpeed;
                    dir = newDirection();
                }
            }
            attackTimer += Time.deltaTime;
            // If its been more than three seconds without being close to the mouse go back to chase
            if (attackTimer >= 3f){
                actualMoveSpeed = chaseMoveSpeed;
                controller.setState(chase);
                dir = newDirection();
            }
        }
    }
    
    public void backOff(){
        // This method allows the mouse to respond to a cat attack and potentially get away
        if (mouse == null){ // If mouse is dead, go back to patrol
            controller.setState(patrol);
            actualMoveSpeed = patrolMoveSpeed;
        }
        // Move backwards 
        dir = newDirection();
        backOffTimer += Time.deltaTime;

        // Pause the cat
        if (backOffTimer >= 1f){
            actualMoveSpeed = 0f;
            // After 3 seconds go back to attacking
            if (backOffTimer >= 3f){
                actualMoveSpeed = attackMoveSpeed;
                controller.setState(attack);
                backOffTimer = 0f;
            }
        }
    }
}
