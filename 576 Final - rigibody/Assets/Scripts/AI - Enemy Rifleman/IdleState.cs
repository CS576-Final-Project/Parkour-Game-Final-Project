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
        parameter.capturePlayerPostition = false;
    }

    public void OnUpdate()
    {
        if (parameter.canSeePlayer) {
            parameter.animationController.SetBool(parameter.singleShootingHash, true);
            manager.TransitionState(StateType.AttackSingle);
        }

        if (parameter.health <= 0) {
            parameter.die = true;
            manager.TransitionState(StateType.Die);
        }

        // Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(parameter.playerLastPosition.x, parameter.gunTip.transform.position.y, parameter.playerLastPosition.z) - parameter.gunTip.transform.position);
        // manager.OrientationTo(desiredRotation);
    }

    public void OnExit()
    {

    }
}
