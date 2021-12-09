using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : FSMState
{
    private FSMRifleman manager;
    private EnemyRiflemanParameter parameter;

    public IdleState(FSMRifleman manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.lights.gameObject.SetActive(false);
        parameter.capturePlayerPostition = false;
    }

    public void OnUpdate()
    {
        if (parameter.canSeePlayer) {
            manager.TransitionState(StateTypeRifleman.AttackSingle);
        }

        if (parameter.health <= 0) {
            parameter.die = true;
            manager.TransitionState(StateTypeRifleman.Die);
        }
    }

    public void OnExit()
    {

    }
}
