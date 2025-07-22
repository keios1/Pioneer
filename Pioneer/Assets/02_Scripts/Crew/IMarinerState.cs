using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMarinerState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}
