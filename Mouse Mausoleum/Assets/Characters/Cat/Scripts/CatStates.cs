using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class CatStates : MonoBehaviour
{
    private FSM controller = new FSM();
    private float distanceRay = 4f;
    private float seekTimer;
    private float attackTimer;
    private float backOffTimer;
    private float movementTimer;
    private float randomness = 0.75f;
    private float mouseDistance;

    private float actualMoveSpeed;
    private float patrolMoveSpeed = 3f;
    private float seekMoveSpeed = 1.5f;
    private float chaseMoveSpeed = 5f;
    private float attackMoveSpeed = 8f;

    private float chaseTimer;


    private Vector3 dir;
    private Vector3 previousDir;
    private Vector3 mouseDirection;
    List<Vector3> possibleDir = new List<Vector3> {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

    public Rigidbody rb;

    public Transform mouse;
    public GameObject mouseObj;
    private float seekDistance = 25f;
    private float seekDistanceSqr; // This needs to be the sqaure of the distance you want to check, so I'm checking 15

    private float chaseDistance = 10f;
    private float chaseDistanceSqr; // This needs to be the sqaure of the distance you want to check, so I'm checking 15

    private float attackDistance = 5f;
    private float collisionDistance = 1f;
    private bool mouseHit;
    
    void Start(){
        // Randomize the possible directions to start in, then pick a direction that doesn't have a wall and go
        List<Vector3> shuffled = possibleDir.OrderBy(x => UnityEngine.Random.value).ToList(); // found this on https://discussions.unity.com/t/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code/535113
        foreach (Vector3 d in shuffled){
            if (!Physics.Raycast(transform.position, d, distanceRay)){
                dir = d;
                previousDir = -dir;
            }
        }

        actualMoveSpeed = patrolMoveSpeed;

        seekDistanceSqr = seekDistance * seekDistance;

        chaseDistanceSqr = chaseDistance * chaseDistance;

        // Set the first state as patrolling
        controller.setState(patrol);
    }

    private void Update(){
        // Update FSM
        controller.Update();
        
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
                Debug.Log("Pausing 1");
                return primaryDir.normalized;
            }

            // Then if we cannot go in the cardinal direction of the mouse go in the next best direction the mouse is in, for example if the mouse is south east and prodominantly south, but south has a wall in the way then go east 
            mouseHit = Physics.Raycast(transform.position, secondaryDir, distanceRay);
            if (mouseHit == false) {
                Debug.Log("Pausing 2");
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
        } else if (controller.activeState == (Action)backOff ) {
            mouseDirection = (mouse.position - transform.position).normalized;
            return (mouseDirection * -1);
        }

        // If we could not find a new direction then just go back
        Debug.Log("Going back");
        previousDir = -previousDir;
        return previousDir; 
    }

    public void patrol(){
        Debug.Log("Patroling!");
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

    public void seek(){
        movementTimer += Time.deltaTime;

        // Check to see the cat can still see the mouse, if the cat sees a wall or the mouse is too far we start the countdown until the cat goes back to patrol
        mouseDirection = (mouse.position - transform.position).normalized;
        mouseHit = Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance);
        if (mouseHit == false || hit.collider.transform != mouse){
            seekTimer += Time.deltaTime;
            Debug.Log("Might stop seeking soon.");
            dir = newDirection();
        } else {
            Debug.Log("Seeking!");
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
            Debug.Log("Might stop chasing soon.");
            chaseTimer += Time.deltaTime;
            dir = newDirection();
        } else {
            Debug.Log("Chasing!");
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
            Debug.Log(hitMouse.distance);
            attackTimer = 0f;
            mouseObj.GetComponent<Health>().loseHealth();
            controller.setState(backOff);
        }
        else{
            attackTimer += Time.deltaTime;
            if (attackTimer >= 3f){
                actualMoveSpeed = chaseMoveSpeed;
                controller.setState(chase);
                dir = newDirection();
            }
        }
    }
    
    public void backOff(){
        // Move backwards 
        dir = newDirection();
        backOffTimer += Time.deltaTime;

        // Pause the cat
        if (backOffTimer >= 1f){
            actualMoveSpeed = 0f;
            if (backOffTimer >= 3f){
                actualMoveSpeed = attackMoveSpeed;
                controller.setState(attack);
            }
        }

    }
}
