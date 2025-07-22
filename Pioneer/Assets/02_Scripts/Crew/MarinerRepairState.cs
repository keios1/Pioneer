using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarinerRepairState : IMarinerState
{
    private readonly MarinerAI mariner;

    public MarinerRepairState(MarinerAI mariner)
    {
        this.mariner = mariner;
    }

    public void OnEnter()
    {
        mariner.StartCoroutine(RepairRoutine());
    }

    private System.Collections.IEnumerator RepairRoutine()
    {
        yield return mariner.FindAndRepairCoroutine();
        mariner.StateMachine.ChangeState(new MarinerSecondPriorityState(mariner));
    }

    public void OnUpdate() { }
    public void OnExit() { }
}