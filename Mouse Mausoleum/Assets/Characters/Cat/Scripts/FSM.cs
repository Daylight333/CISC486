using System;
using UnityEngine;

public class FSM
{
    private Action activeState;

    public void Update()
    {
        if (activeState != null){
            activeState();
        }
    }

    public void setState(Action state){
        activeState = state;
    }
}
