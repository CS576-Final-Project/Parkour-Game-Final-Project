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
        parameter.lights.gameObject.SetActive(false);
        parameter.animationController.SetBool(parameter.dieHash, true);
        parameter.diePlayerRotation = Quaternion.LookRotation(new Vector3(parameter.parallelRotation.position.x, parameter.gunTip.transform.position.y, parameter.parallelRotation.position.z) - parameter.gunTip.transform.position);
    }

    public void OnUpdate()
    {
        if (parameter.canSeePlayer) {
            manager.OrientationTo(parameter.diePlayerRotation);
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
