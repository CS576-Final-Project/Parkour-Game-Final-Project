using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSingleState : FSMState
{
    private FSMRifleman manager;
    private EnemyRiflemanParameter parameter;
    private Quaternion desiredRotation = new Quaternion();
    private float promptTimer = 0f;
    private bool activatePrompt = false;
    private float activateTimer = 0f;
    private bool coroutineActive = false;
    private float rand = 0.5f;  // random number to control whether move left or right
    private float timer = 0f;  // timer to record time passed since last generation of rand

    public AttackSingleState(FSMRifleman manager)
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
        parameter.animationController.SetBool(parameter.singleShotReadyHash, true);
        activatePrompt = true;
        parameter.capturePlayerPostition = false;
    }

    public void OnUpdate()
    {
        if (!parameter.canSeePlayer) {
            manager.TransitionState(StateTypeRifleman.Idle);
        }

        if (parameter.health <= 0) {
            parameter.die = true;
            manager.TransitionState(StateTypeRifleman.Die);
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
        if (parameter.animationController.GetBool(parameter.singleShootingHash))
            parameter.animationController.SetBool(parameter.singleShootingHash, false);

        parameter.shootingDirection = parameter.optimizedPlayerPosition;
        
        desiredRotation = Quaternion.LookRotation(parameter.shootingDirection - Vector3.zero);
        manager.OrientationTo(desiredRotation);
        
        // random movement to left or right
        // define up direction:
        Vector3 up = new Vector3(0f, 1f, 0f);
        // find right vector and left vector of 
        Vector3 right = Vector3.Cross(parameter.shootingDirection, up.normalized);
        Vector3 left = -right;

        // enemy may change direction of movement every 1 sec
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            rand = Random.Range(0f, 1f);  // ge ta new random number 
            timer = 0f;  // reset timer
        }
        if (rand < 0.5)  // move left with probability of 0.5
        {
            parameter.rb.AddForce(left * parameter.randomSpeed, ForceMode.Acceleration);
            // change animation
            if (parameter.animationController.GetBool(parameter.singleShotReadyHash))
            {
                parameter.animationController.SetBool(parameter.walkLeftShootHash, true);
            }
            else if (parameter.animationController.GetBool(parameter.walkRightShootHash))
            {
                parameter.animationController.SetBool(parameter.walkRightShootHash, false);
                parameter.animationController.SetBool(parameter.walkLeftShootHash, true);
            }
        }
        else
        {
            parameter.rb.AddForce(right * parameter.randomSpeed, ForceMode.Acceleration);
            // change animation 
            if (parameter.animationController.GetBool(parameter.singleShotReadyHash))
            {
                parameter.animationController.SetBool(parameter.walkRightShootHash, true);
            }
            else if (parameter.animationController.GetBool(parameter.walkLeftShootHash))
            {
                parameter.animationController.SetBool(parameter.walkLeftShootHash, false);
                parameter.animationController.SetBool(parameter.walkRightShootHash, true);
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
        parameter.animationController.SetBool(parameter.singleShotReadyHash, false);
        if (!parameter.die) {
            if (!parameter.capturePlayerPostition) {
                parameter.playerLastPosition = parameter.playerCentroid;
                parameter.capturePlayerPostition = true;
            }
            desiredRotation = Quaternion.LookRotation(new Vector3(parameter.playerLastPosition.x, parameter.gunTip.transform.position.y, parameter.playerLastPosition.z) - parameter.gunTip.transform.position);
            manager.transform.rotation = desiredRotation;
        }
    }

    private void resumePrompt()
    {
        if (!parameter.lights.gameObject.activeInHierarchy) {
            promptTimer += Time.deltaTime;
            if (promptTimer >= 1.1f) {
                parameter.lights.gameObject.SetActive(true);
                promptTimer = 0f;
            }
        }
    }
}
