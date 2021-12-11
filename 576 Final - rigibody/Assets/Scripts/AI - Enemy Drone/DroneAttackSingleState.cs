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
    private float rand = 0.5f;  // random number to control whether move left or right
    private float rand2 = 0.5f;  // random number to control whether move up or down
    private float timer = 0f;  // timer to record time passed since last generation of rand

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

        parameter.shootingDirection = parameter.optimizedPlayerPosition;
        
        desiredRotation = Quaternion.LookRotation(parameter.shootingDirection - Vector3.zero);
        manager.OrientationTo(desiredRotation);
        
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            rand = Random.Range(0f, 1f);  // ge ta new random number 
            rand2 = Random.Range(0f, 1f);  // ge ta new random number 
            timer = 0f;  // reset timer
        }
        if (rand < 0.5)  // random movement
        {
            manager.gameObject.transform.position = Vector3.MoveTowards(manager.gameObject.transform.position, 
                parameter.leftDirection.position, parameter.randomSpeed * Time.deltaTime * 0.1f);
            if (rand2 < 0.3)
            {
                manager.gameObject.transform.position = Vector3.MoveTowards(manager.gameObject.transform.position, 
                    parameter.upDirection.position, parameter.randomSpeed * Time.deltaTime * 0.1f);
            }
            else if (rand2 >= 0.3 && rand2 < 0.6)
            {
                manager.gameObject.transform.position = Vector3.MoveTowards(manager.gameObject.transform.position, 
                    parameter.downDirection.position, parameter.randomSpeed * Time.deltaTime * 0.1f);
            }
        }
        else
        {
            manager.gameObject.transform.position = Vector3.MoveTowards(manager.gameObject.transform.position, 
                parameter.rightDirection.position, parameter.randomSpeed * Time.deltaTime * 0.1f);
            if (rand2 < 0.3)
            {
                manager.gameObject.transform.position = Vector3.MoveTowards(manager.gameObject.transform.position, 
                    parameter.upDirection.position, parameter.randomSpeed * Time.deltaTime * 0.1f);
            }
            else if (rand2 >= 0.3 && rand2 < 0.6)
            {
                manager.gameObject.transform.position = Vector3.MoveTowards(manager.gameObject.transform.position, 
                    parameter.downDirection.position, parameter.randomSpeed * Time.deltaTime * 0.1f);
            }
        }

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
