using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarinerWanderState : IMarinerState
{
    private readonly MarinerAI mariner;
    private float timer;

    public MarinerWanderState(MarinerAI mariner)
    {
        this.mariner = mariner;
    }

    public void OnEnter()
    {
        mariner.SetRandomDirection();
        timer = mariner.moveDuration;
    }

    public void OnUpdate()
    {
        mariner.Wander();
        timer -= Time.deltaTime;

        if (timer <= 0f)
            mariner.StateMachine.ChangeState(new MarinerIdleState(mariner));
    }

    public void OnExit() { }
}