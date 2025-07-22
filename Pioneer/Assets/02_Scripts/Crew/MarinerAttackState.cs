using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarinerAttackState : IMarinerState
{
    private readonly MarinerAI mariner;

    public MarinerAttackState(MarinerAI mariner)
    {
        this.mariner = mariner;
    }

    public void OnEnter()
    {
        mariner.StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        yield return mariner.StartCoroutine(mariner.AttackSequence());
        mariner.StateMachine.ChangeState(new MarinerWanderState(mariner));
    }

    public void OnUpdate() { }
    public void OnExit() { }
}
