using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class CatStates : MonoBehaviour
{
    private FSM controller = new FSM();
    private float distanceRay = 4f;
    private float seekTimer;
    private float movementTimer;
    private float randomness = 0.75f;

    private float actualMoveSpeed;
    private float patrolMoveSpeed = 3f;
    private float seekMoveSpeed = 1.5f;
    private float chaseMoveSpeed = 10f;
    private float attackMoveSpeed = 4f;

    private float chaseTimer = 10f;
    private bool isChaseTimerSet = false;


    private Vector3 dir;
    private Vector3 previousDir;
    List<Vector3> possibleDir = new List<Vector3> {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

    public Rigidbody rb;

    public Transform mouse;
    private float seekDistance = 15f;
    private float seekDistanceSqr; // This needs to be the sqaure of the distance you want to check, so I'm checking 15
    
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
            Vector3 mouseDirection = (mouse.position - transform.position).normalized;
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
            bool mouseHit = Physics.Raycast(transform.position, primaryDir, out RaycastHit hitPrimary, distanceRay);
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
        }
        // If we could not find a new direction then just go back
        previousDir = -previousDir;
        return previousDir; 
    }

    public void patrol(){
        if ((mouse.position - transform.position).sqrMagnitude <= seekDistanceSqr){
            Vector3 mouseDirection = (mouse.position - transform.position).normalized;
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
        seekTimer += Time.deltaTime;
        movementTimer += Time.deltaTime;
        if (movementTimer >= 1f){
            dir = newDirection();
            movementTimer = 0;
        }
        if (seekTimer >= 5f){
            Vector3 mouseDirection = (mouse.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance)){
                if (hit.collider.transform == mouse){
                    seekTimer = 0;
                } else{
                    actualMoveSpeed = patrolMoveSpeed;
                    controller.setState(patrol);
                    dir = newDirection();
                }
            }
        }
    }

    public void chase(){
        
        // if the timer isn't set 
        if (!isChaseTimerSet) {
            
            // every second
            if (movementTimer >= 1f){
                movementTimer = 0f;
                
                // Take the mouse direction and determine if we need to set timer
                Vector3 mouseDirection = (mouse.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance)){
                    if (hit.collider.transform != mouse){
                        isChaseTimerSet = true;  
                    }
                }
                // Set cat direction to mouse and increase movement speed
                dir = mouseDirection;
                actualMoveSpeed = chaseMoveSpeed;
            } 
        }

        // If we have a timer set already 
        else {

            // Decrease the timer each second 
            chaseTimer -= Time.deltaTime;

            // when the timer finishes change state to patrol, reset our timer
            if (chaseTimer <= 0f){
                isChaseTimerSet = false;
                chaseTimer = 10f;
                controller.setState(patrol);
            }
 
            // if the timer is still going, keep tailing the mouse every second 
            if (movementTimer >= 1f){
                movementTimer = 0f;
                //Get the mouse direction and set cat direction to mouse direction 
                Vector3 mouseDirection = (mouse.position - transform.position).normalized;
                dir = mouseDirection;
            }
        }

    }

    public void attack(){
        
    }
}
