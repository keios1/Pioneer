using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarinerSecondPriorityState : IMarinerState
{
    private readonly MarinerAI mariner;

    public MarinerSecondPriorityState(MarinerAI mariner)
    {
        this.mariner = mariner;
    }

    public void OnEnter()
    {
        mariner.StartCoroutine(SecondPriorityRoutine());
    }

    private System.Collections.IEnumerator SecondPriorityRoutine()
    {
        yield return mariner.StartCoroutine(mariner.StartSecondPriorityAction());
        mariner.StateMachine.ChangeState(new MarinerRepairState(mariner));
    }

    public void OnUpdate() { }
    public void OnExit() { }
}