using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CatStates : MonoBehaviour
{
    // FSM, Rigid Body, and basic patroling
    private FSM controller = new FSM();
    public Rigidbody rb;
    public Animator animator;
    public Transform[] checkPoints;
    private int destPoint = 0;
    private bool hasBackedOff = false;
    int checkpointLayer;
    int layerMask;

    // Timers for various states
    private float seekTimer;
    private float attackTimer;
    private float backOffTimer;
    private float attackAnimation;
    private float movementTimer;
    private float chaseTimer;
    
    // Speeds for different states
    private float patrolMoveSpeed = 3f;
    private float seekMoveSpeed = 1.5f;
    private float chaseMoveSpeed = 5f;
    private float attackMoveSpeed = 8f;

    // Direction vectors, and info for the mouse
    private Vector3 mouseDirection;
    public Transform mouse;
    public GameObject mouseObj;
    private bool mouseHit;
    private Vector3 swayAngle;
    private Vector3 swayedDirection = new Vector3(0f, 0f, 0f);

    // Distances to check for each state
    private float seekDistance = 25f;
    private float seekDistanceSqr; // This needs to be the sqaure of the distance you want to check to use the sqrMagnitude function
    private float chaseDistance = 10f;
    private float chaseDistanceSqr;
    private float attackDistance = 5f;
    private float collisionDistance = 2f;

    public NavMeshAgent agent;

    private bool mouseDead = false;
    
    
    void Start(){

        // Set the initial speed, seek distance and chase distance checks
        agent.speed = patrolMoveSpeed;

        seekDistanceSqr = seekDistance * seekDistance;

        chaseDistanceSqr = chaseDistance * chaseDistance;

        checkpointLayer = LayerMask.NameToLayer("CheckPoint");

        layerMask = ~(1 << checkpointLayer);

        // Set the first state as patrolling
        animator.Play("TomWalk");
        controller.setState(patrol);

        agent.destination = checkPoints[0].position;
    }

    private void Update(){
        // Update FSM
        controller.Update();
        
        // Check if the mouse still exists, if its dead then go back to patrol
        if (mouseDead == false){
            if (mouse == null){
                mouseDead = true;
                controller.setState(patrol);
                agent.speed = patrolMoveSpeed;
                animator.CrossFade("TomWalk", 0.1f, 0, 0.5f);
                agent.updateRotation = true;
                agent.isStopped = false;
            }
        }
    }

    private void FixedUpdate(){
        movementTimer += Time.deltaTime;
        newDirection();//
    }

    public void newDirection(){
        // If we are in the patrol state then just randomly choose a direction to go in
        if (controller.activeState == (Action)patrol){
            if (agent.remainingDistance < 0.5f){
                // Choose the next checkpoint in the array as the destination
                destPoint = (destPoint + 1) % checkPoints.Length;

                // Set the cat to go to the currently selected checkpoint
                agent.destination = checkPoints[destPoint].position;
            }
        } else if (controller.activeState == (Action)seek){

            mouseDirection = mouse.position;

            if (movementTimer >= 1f){
                movementTimer = 0f;
                swayAngle = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(0.5f, 0.5f));
            }

            swayedDirection = swayAngle + mouseDirection;

            agent.destination = swayedDirection;

        } else if (controller.activeState == (Action)chase || controller.activeState == (Action)attack){
            // If we can see the mouse then go towards the mouse
            agent.destination = mouse.position;
        } else if (controller.activeState == (Action)backOff){
            // If we are backing off to give the mouse some time to react then move backwards
            if (attackAnimation >= 1f){
                if (hasBackedOff == false){
                    hasBackedOff = true;
                    mouseDirection = (transform.position - mouse.position).normalized;
                    agent.destination = transform.position + (mouseDirection * 5f);
                }
            } else {
                attackAnimation += Time.deltaTime;
            }
        }
    }

    public void patrol(){
        // Check if the mouse exists
        if (mouse != null){
            // If the mouse is within the seek distance check if its behind a wall, if not then set the state to seek
            if ((mouse.position - transform.position).sqrMagnitude <= seekDistanceSqr){
                mouseDirection = (mouse.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance, layerMask)){
                    if (hit.collider.transform == mouse){
                        animator.CrossFade("TomSeek", 0.1f, 0, 0.5f);
                        controller.setState(seek);
                        agent.speed = seekMoveSpeed;
                    }
                }
            }
        }
    }

    public void seek(){
        // Check to see the cat can still see the mouse, if the cat sees a wall or the mouse is too far we start the countdown until the cat goes back to patrol
        mouseDirection = (mouse.position - transform.position).normalized;
        mouseHit = Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, seekDistance, layerMask);
        if (mouseHit == false || hit.collider.transform != mouse){
            seekTimer += Time.deltaTime;
        } else {
            seekTimer = 0f;
        }
        // If the cat has not been able to see the mouse for more than 5 seconds, go back to patrol
        if (seekTimer >= 10f){
            seekTimer = 0f;
            agent.speed = patrolMoveSpeed;
            agent.destination = checkPoints[destPoint].position;
            animator.CrossFade("TomWalk", 0.1f, 0, 0.5f);
            controller.setState(patrol);  
        }
        // If the mouse gets even closer to the cat then the cat starts to chase
        if ((mouse.position - transform.position).sqrMagnitude <= chaseDistanceSqr){
            mouseDirection = (mouse.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, seekDistance, layerMask)){
                if (hitMouse.collider.transform == mouse){
                    animator.CrossFade("TomRun", 0.1f, 0, 0.5f);
                    controller.setState(chase);
                    agent.speed = chaseMoveSpeed;
                }
            }
        }
    }

    public void chase(){
        // Check to see the cat can still see the mouse, if the cat sees a wall or the mouse is too far we start the countdown until the cat goes back to seeking
        mouseDirection = (mouse.position - transform.position).normalized;
        mouseHit = Physics.Raycast(transform.position, mouseDirection, out RaycastHit hit, chaseDistance, layerMask);
        if (mouseHit == false || hit.collider.transform != mouse){
            chaseTimer += Time.deltaTime;
        } else {
            chaseTimer = 0f;
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, attackDistance, layerMask)){
                agent.speed = attackMoveSpeed;
                controller.setState(attack);
            }
        }
        // when the timer finishes change state to patrol, reset our timer
        if (chaseTimer >= 5f){
            chaseTimer = 0f;
            animator.CrossFade("TomSeek", 0.1f, 0, 0.5f);
            controller.setState(seek);
            agent.speed = seekMoveSpeed;
        }
    }

    public void attack(){
        Debug.Log("Attack");

        // Recalculate mouse direction and see if mouse is in attack range 
        mouseDirection = (mouse.position - transform.position).normalized;

        //check for collision with the mouse 
        if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitMouse, 2f, layerMask)){
            if (hitMouse.collider.transform == mouse){
                attackTimer = 0f;
                hasBackedOff = false;
                controller.setState(backOff);
                agent.isStopped = true;
                agent.updateRotation = false;
                agent.ResetPath();            
                agent.Warp(agent.transform.position);  //
                StartCoroutine(waitToDoDamage());
                attackAnimation = 0f;
                newDirection();
            }
        }
        else{ // If the cat did not collide and the mouse is behind a wall or out of line of sight then go back to seeking
            if (Physics.Raycast(transform.position, mouseDirection, out RaycastHit hitWall, collisionDistance, layerMask)){
                if (hitWall.collider.transform != mouse){
                    chaseTimer = 0f;
                    animator.CrossFade("TomSeek", 0.1f, 0, 0.5f);
                    controller.setState(seek);
                    agent.speed = seekMoveSpeed;
                }
            }
            attackTimer += Time.deltaTime;
            // If its been more than three seconds without being close to the mouse go back to chase
            if (attackTimer >= 3f){
                agent.speed = chaseMoveSpeed;
                animator.CrossFade("TomRun", 0.1f, 0, 0.5f);
                controller.setState(chase);
            }
        }
    }

    IEnumerator waitToDoDamage(){
        // Start the animation 40% of the way through because we cannot directly alter the animation clip, so just start when the swing happens
        animator.CrossFade("TomAttack", 0.1f, 0, 0.4f);
        // Adjust for the fact that we are starting the animation 40% of the way through
        float attackAnimationLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.6f;
        yield return new WaitForSeconds(attackAnimationLength);
        mouseObj.GetComponent<Health>().loseHealth();
        backOff();
    }

    // This method allows the mouse to respond to a cat attack and potentially get away
    public void backOff(){
        Debug.Log("Backoff");

        
        backOffTimer += Time.deltaTime;

        if (backOffTimer >= 3f){
            agent.Warp(agent.transform.position);
            animator.Play("TomIdle");
        }
        // After 4 seconds go back to attacking
        if (backOffTimer >= 4f){
            agent.updateRotation = true;
            agent.isStopped = false;
            controller.setState(attack);
            animator.CrossFade("TomRun", 0.1f, 0, 0.5f);
            backOffTimer = 0f;
            hasBackedOff = false;
            attackAnimation = 0f;
        }
        
    }
}
