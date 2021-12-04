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
            manager.TransitionState(StateType.Idle);
        }

        if (parameter.health <= 0) {
            parameter.die = true;
            manager.TransitionState(StateType.Die);
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
    }

    public void OnExit()
    {
        manager.StopAllCoroutines();
        coroutineActive = false;
        manager.coroutineActive = false;
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
            if (promptTimer >= 1.5f) {
                parameter.lights.gameObject.SetActive(true);
                promptTimer = 0f;
            }
        }
    }

    // public IEnumerator SingleShoot() {
    //     while (true)
    //     {   
    //         float shootingDelay = 3f;
    //         WaitForSeconds wait = new WaitForSeconds(shootingDelay);

    //         if (parameter.canSeePlayer && !parameter.die) {
    //             parameter.lights.gameObject.SetActive(false);
    //             parameter.animationController.SetBool(parameter.singleShootingHash, true);

    //             GameObject newObject = Object.Instantiate(parameter.bullet, parameter.gunTip.position, parameter.gunTip.rotation);
    //             newObject.transform.GetChild(0).GetComponent<Bullet>().shootingDirection = parameter.shootingDirection;
    //             newObject.transform.GetChild(0).GetComponent<Bullet>().speed = parameter.bulletSpeed;
    //         }
    //         yield return wait; // next shot will be shot after this delay
    //     }
    // }
}
