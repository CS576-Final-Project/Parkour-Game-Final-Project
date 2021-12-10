using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDieState : FSMState
{
    private FSMDrone manager;
    private EnemyDroneParameter parameter;
    
    public DroneDieState(FSMDrone manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter() 
    {
        parameter.diePlayerRotation = Quaternion.LookRotation(new Vector3(parameter.parallelRotation.position.x, parameter.gunTip.transform.position.y, parameter.parallelRotation.position.z) - parameter.gunTip.transform.position);
    }

    public void OnUpdate() 
    {
        if (parameter.canSeePlayer) {
            manager.OrientationTo(parameter.diePlayerRotation);
        }

        Object.Destroy(manager.GetComponent<SphereCollider>());
        SelfExplode();
    }

    public void OnExit() 
    {

    }

    public void OnFixedUpdate()
    {

    }

    private void SelfExplode() {
        Object.Instantiate(parameter.deathExplosionEffect, parameter.explosionPoint.position, parameter.deathExplosionEffect.transform.rotation, null);
        Object.Destroy(manager.gameObject);
    }
}
