using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneIdleState : FSMState
{
    private FSMDrone manager;
    private EnemyDroneParameter parameter;
    private float y;
    private float timer;

    public DroneIdleState(FSMDrone manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
        y = manager.transform.position.y;
    }

    public void OnEnter() 
    {

    }

    public void OnUpdate() 
    {
        if (parameter.canSeePlayer) {
            manager.TransitionState(StateTypeDrone.AttackSingle);
        }

        if (parameter.health <= 0) {
            parameter.die = true;
            manager.TransitionState(StateTypeDrone.Die);
        }
        
        timer += Time.deltaTime;
        y += Mathf.Sin(timer * Mathf.PI * 0.85f) * 0.005f;
        manager.transform.position = new Vector3(manager.transform.position.x, y, manager.transform.position.z);
    }

    public void OnExit() 
    {

    }
}
