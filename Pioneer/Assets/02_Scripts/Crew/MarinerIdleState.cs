using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarinerIdleState : IMarinerState
{
    private readonly MarinerAI mariner;
    private float timer;

    public MarinerIdleState(MarinerAI mariner)
    {
        this.mariner = mariner;
    }

    public void OnEnter()
    {
        timer = mariner.idleDuration;
    }

    public void OnUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
            mariner.StateMachine.ChangeState(new MarinerWanderState(mariner));
    }

    public void OnExit() { }
}