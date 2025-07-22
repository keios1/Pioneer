using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarinerStateMachine
{
    private IMarinerState currentState;
    private readonly MarinerAI mariner;

    public MarinerStateMachine(MarinerAI mariner)
    {
        this.mariner = mariner;
    }

    public void ChangeState(IMarinerState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public void Update()
    {
        currentState?.OnUpdate();
    }

    public IMarinerState GetCurrentState() => currentState;
}
