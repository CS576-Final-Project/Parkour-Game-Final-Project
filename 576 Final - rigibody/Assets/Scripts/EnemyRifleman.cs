using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRifleman : MonoBehaviour
{
    public Animator animationController;
    private GameObject player;

    // For detecting player
    public float radius;
    public float angle;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private bool canSeePlayer;

    private float walkingVelocity;
    private float runningVelocity;
    private Vector3 shootingDirection;

    private int singleShootingHash;

    public Transform gunTip;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animator>();

        player = GameObject.FindWithTag("Player");
        StartCoroutine(FOVRoutine());
        StartCoroutine(SingleShoot());

        radius = 80f;
        angle = 100f;

        singleShootingHash = Animator.StringToHash("ShootingSingleShot");
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo currInfo = animationController.GetCurrentAnimatorStateInfo(0);

        shootingDirection = (player.transform.position - gunTip.transform.position).normalized;

        if (canSeePlayer) {
            float angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(shootingDirection.x, shootingDirection.z);
            transform.eulerAngles = new Vector3(0.0f, angleToRotate, 0.0f);

            animationController.SetBool(singleShootingHash, true);

        } else {
            animationController.SetBool(singleShootingHash, false);
        }
    }

    private IEnumerator FOVRoutine() {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true) {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private IEnumerator SingleShoot() {
        while (true)
        {   
            float shootingDelay = 0.5f;
            WaitForSeconds wait = new WaitForSeconds(shootingDelay);

            if (canSeePlayer)
            {
                GameObject newObject = Instantiate(bullet, gunTip.position, gunTip.rotation);
                newObject.GetComponent<Bullet>().shootingDirection = shootingDirection;
            }
            yield return wait; // next shot will be shot after this delay
        }
    }

    private void FieldOfViewCheck() {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, playerMask);

        if (rangeChecks.Length != 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)) {
                    canSeePlayer = true;
                } else {
                    canSeePlayer = false;
                }
            } else {
                canSeePlayer = false;
            }
        } else if (canSeePlayer) {
            canSeePlayer = false;
        }
    }
}
