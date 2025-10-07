using System;
using UnityEngine;

public class FSM
{
    public Action activeState;

    public void Update()
    {
        // Make sure to keep running the state every update loop
        if (activeState != null){
            activeState();
        }
    }

    // Set the current state
    public void setState(Action state){
        activeState = state;
    }
}
