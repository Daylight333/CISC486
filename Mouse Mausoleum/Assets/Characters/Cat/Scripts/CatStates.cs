using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class CatStates : MonoBehaviour
{
    private FSM controller = new FSM();
    private float distanceRay = 4f;

    private float moveSpeed = 2f;
    private Vector3 dir;
    private Vector3 previousDir;
    List<Vector3> possibleDir = new List<Vector3> {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

    public Rigidbody rb;
    
    void Start(){
        // Randomize the possible directions to start in, then pick a direction that doesn't have a wall and go
        List<Vector3> shuffled = possibleDir.OrderBy(x => UnityEngine.Random.value).ToList(); // found this on https://discussions.unity.com/t/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code/535113
        foreach (Vector3 d in shuffled){
            if (!Physics.Raycast(transform.position, d, distanceRay)){
                dir = d;
                previousDir = -dir;
            }
        }

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
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
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
            // If we could not find a new direction then just go back the way you came.
            previousDir = -previousDir;
            return previousDir; 
        }
        // If we could not find a new direction then just go back the way you came.
            previousDir = -previousDir;
            return previousDir; 
    }

    public void patrol(){
        

    }

    public void seek(){
        
    }

    public void chase(){
        
    }

    public void attack(){
        
    }
}
