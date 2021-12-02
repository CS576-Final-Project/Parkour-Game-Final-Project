using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : FSMState
{
    private FSMRifleman manager;
    private EnemyRiflemanParameter parameter;

    public DieState(FSMRifleman manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.die = true;
        parameter.animationController.SetBool("Die", true);
        parameter.diePlayerPosition = new Vector3(parameter.player.transform.position.x, parameter.gunTip.position.y, parameter.player.transform.position.z);
    }

    public void OnUpdate()
    {
        if (parameter.canSeePlayer) {
            Quaternion desiredRotation = Quaternion.LookRotation(parameter.diePlayerPosition - parameter.gunTip.transform.position);
            manager.OrientationTo(desiredRotation);
        }

        if (parameter.animationController.GetCurrentAnimatorStateInfo(0).IsName("Die")) {
            parameter.animationController.SetBool("NoDieLoop", false);
            parameter.animationController.speed = 0.6f;
            if (parameter.animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                parameter.animationController.speed = 0;
            }
        }
        SelfExplode();
    }

    public void OnExit()
    {

    }

    private void SelfExplode() {
        parameter.selfExplodeTimer += Time.deltaTime;
        if (parameter.selfExplodeTimer >= parameter.timeToExplode) {
            Object.Instantiate(parameter.deathExplosionEffect, parameter.explosionPoint.position, parameter.deathExplosionEffect.transform.rotation, null);
            Object.Destroy(manager.gameObject);
        }
    }
}
