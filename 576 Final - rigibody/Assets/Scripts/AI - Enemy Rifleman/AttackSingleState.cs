using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSingleState : FSMState
{
    private FSMRifleman manager;
    private EnemyRiflemanParameter parameter;
    private Quaternion desiredRotation = new Quaternion();

    public AttackSingleState(FSMRifleman manager)
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
        if (!parameter.canSeePlayer) {
            parameter.animationController.SetBool(parameter.singleShootingHash, false);
            manager.TransitionState(StateType.Idle);
        }

        if (parameter.health <= 0) {
            manager.TransitionState(StateType.Die);
        }

        if ((parameter.playerMove.isGrounded() || !parameter.wallRun.StopWallRun())) {
            parameter.shootingDirection = parameter.optimizedPlayerPosition;
        } else {
            parameter.shootingDirection = (parameter.playerCentroid - parameter.gunTip.transform.position).normalized;
        }
        
        desiredRotation = Quaternion.LookRotation(parameter.shootingDirection - Vector3.zero);
        manager.OrientationTo(desiredRotation);
    }

    public void OnExit()
    {
        if (!parameter.capturePlayerPostition) {
            parameter.playerLastPosition = parameter.playerCentroid;
            parameter.capturePlayerPostition = true;
        }
        desiredRotation = Quaternion.LookRotation(new Vector3(parameter.playerLastPosition.x, parameter.gunTip.transform.position.y, parameter.playerLastPosition.z) - parameter.gunTip.transform.position);
        manager.transform.rotation = desiredRotation;
    }
}
