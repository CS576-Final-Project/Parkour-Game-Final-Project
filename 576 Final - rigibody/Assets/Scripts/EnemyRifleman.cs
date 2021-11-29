using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRifleman : MonoBehaviour
{
    public Animator animationController;
    private GameObject player;
    private PlayerMove playerMove;

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
    public Transform Head;
    public GameObject bullet;

    public bool die = false;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animator>();

        player = GameObject.FindWithTag("Player");
        playerMove = player.GetComponent<PlayerMove>();

        StartCoroutine(FOVRoutine());
        StartCoroutine(SingleShoot());

        radius = 80f;
        angle = 100f;

        singleShootingHash = Animator.StringToHash("ShootingSingleShot");
    }

    // Update is called once per frame
    void Update()
    {
        if (!die) {
            AnimatorStateInfo currInfo = animationController.GetCurrentAnimatorStateInfo(0);

            Vector3 optimizedPlayerPosition = new Vector3(player.transform.position.x, player.transform.position.y + 0.3f, player.transform.position.z);
            shootingDirection = (optimizedPlayerPosition - gunTip.transform.position).normalized;

            if (canSeePlayer) {
                if (optimizedPlayerPosition.y <= gunTip.transform.position.y) {
                    float angleToRotate = Mathf.Rad2Deg * Mathf.Atan2(shootingDirection.x, shootingDirection.z);
                    transform.eulerAngles = new Vector3(0.0f, angleToRotate, 0.0f);
                } else {
                    Quaternion desiredRotation = Quaternion.LookRotation(optimizedPlayerPosition - gunTip.transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 10f);
                }

                animationController.SetBool(singleShootingHash, true);

            } else {
                animationController.SetBool(singleShootingHash, false);
            }
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

            if (canSeePlayer && !die)
            {
                GameObject newObject = Instantiate(bullet, gunTip.position, gunTip.rotation);
                newObject.GetComponent<Bullet>().shootingDirection = shootingDirection;
            }
            yield return wait; // next shot will be shot after this delay
        }
    }

    private void FieldOfViewCheck() {
        Collider[] rangeChecks = Physics.OverlapSphere(Head.position, radius, playerMask);

        if (rangeChecks.Length != 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - Head.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(Head.position, directionToTarget, distanceToTarget, obstructionMask)) {
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
