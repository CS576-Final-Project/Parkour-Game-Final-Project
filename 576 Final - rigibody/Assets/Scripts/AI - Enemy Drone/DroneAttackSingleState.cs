using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAttackSingleState : FSMState
{
    private FSMDrone manager;
    private EnemyDroneParameter parameter;
    private Quaternion desiredRotation = new Quaternion();
    private float promptTimer = 0f;
    private bool activatePrompt = false;
    private float activateTimer = 0f;
    private bool coroutineActive = false;

    public DroneAttackSingleState(FSMDrone manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter() 
    {
        if (!coroutineActive) {
            manager.StartCoroutine(manager.SingleShoot());
            coroutineActive = true;
        }
        activatePrompt = true;
        parameter.capturePlayerPostition = false;
    }

    public void OnUpdate() 
    {
        if (!parameter.canSeePlayer) {
            manager.TransitionState(StateTypeDrone.Idle);
        }

        if (parameter.health <= 0) {
            parameter.die = true;
            manager.TransitionState(StateTypeDrone.Die);
        }

        if (activatePrompt) {
            activateTimer += Time.deltaTime;
            if (activateTimer >= 0.5f) {
                parameter.lights.gameObject.SetActive(true);
                activatePrompt = false;
                activateTimer = 0f;
            }
        }

        resumePrompt();

        parameter.shootingDirection = (parameter.playerCentroid - parameter.head.position).normalized;
        
        desiredRotation = Quaternion.LookRotation(parameter.shootingDirection - Vector3.zero);
        manager.OrientationTo(desiredRotation);
    }

    public void OnExit() 
    {
        // Stop all coroutines to avoid repetition
        manager.StopAllCoroutines();
        coroutineActive = false;
        manager.coroutineActive = false;

        // Deactivate the prompt object
        parameter.lights.gameObject.SetActive(false);

        // Terminate the shooting animation and keep correct rotation.
        if (!parameter.die) {
            if (!parameter.capturePlayerPostition) {
                parameter.playerLastPosition = parameter.playerCentroid;
                parameter.capturePlayerPostition = true;
            }
            desiredRotation = Quaternion.LookRotation(new Vector3(parameter.playerLastPosition.x, parameter.gunTip.transform.position.y, parameter.playerLastPosition.z) - parameter.gunTip.transform.position);
            manager.transform.rotation = desiredRotation;
        }
    }

    public void OnFixedUpdate()
    {

    }

    private void resumePrompt()
    {
        if (!parameter.lights.gameObject.activeInHierarchy) {
            promptTimer += Time.deltaTime;
            if (promptTimer >= 1.5f) {
                parameter.lights.gameObject.SetActive(true);
                promptTimer = 0f;
            }
        }
    }
}
