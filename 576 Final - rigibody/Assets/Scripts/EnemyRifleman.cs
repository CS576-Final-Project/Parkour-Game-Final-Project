using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRifleman : MonoBehaviour
{
    public Animator animationController;
    private GameObject player;
    private PlayerMove playerMove;
    private WallRun wallRun;
    private MoveSway sway;

    // For detecting player
    public float radius;
    public float angle;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private bool canSeePlayer;
    private bool capturePlayerPostition = false;
    private bool knowPlayerLastPosition = false;
    private Vector3 playerLastPosition;
    private float lostPlayerTimer = 0f;
    private float timeToLost = 10f;

    private float walkingVelocity;
    private float runningVelocity;

    private int singleShootingHash;

    public Transform gunTip;
    public Transform head;
    public GameObject bullet;
    private Vector3 shootingDirection;
    private float bulletSpeed = 30f;

    public bool die = false;
    public Vector3 diePlayerPosition;
    public Transform explosionPoint;
    public GameObject deathExplosionEffect;
    private float selfExplodeTimer = 0f;
    private float timeToExplode = 2.5f;

    private int MAX_ITERATIONS = 10;
    private float EPSILON = 0.08f;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animator>();

        player = GameObject.FindWithTag("Player");
        playerMove = player.GetComponent<PlayerMove>();
        wallRun = player.GetComponent<WallRun>();
        sway = GameObject.FindWithTag("Head").GetComponent<MoveSway>();

        StartCoroutine(FOVRoutine());
        StartCoroutine(SingleShoot());

        radius = 80f;
        angle = 120f;

        singleShootingHash = Animator.StringToHash("ShootingSingleShot");
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo currInfo = animationController.GetCurrentAnimatorStateInfo(0);
        Quaternion desiredRotation = new Quaternion();
        Vector3 playerCentroid = player.transform.GetChild(0).GetComponent<CapsuleCollider>().bounds.center;

        if (!die) {
            Vector3 optimizedPlayerPosition = IterativeApproximation(playerCentroid, playerMove.rb.velocity, bulletSpeed);
            optimizedPlayerPosition.Normalize();
            // Vector3 optimizedPlayerPosition = new Vector3();
            // shootingDirection = (playerCentroid - gunTip.transform.position).normalized;
            if ((playerMove.isGrounded() || !wallRun.StopWallRun())) {
                shootingDirection = optimizedPlayerPosition;
            } else {
                shootingDirection = (playerCentroid - gunTip.transform.position).normalized;
            }

            if (canSeePlayer) {
                capturePlayerPostition = false;
                desiredRotation = Quaternion.LookRotation(shootingDirection - Vector3.zero);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 3f);

                animationController.SetBool(singleShootingHash, true);

            } else {
                // Cannot see player but know player's last position
                animationController.SetBool(singleShootingHash, false);
                // Make NPCs have the correct rotation when they lose the player's position
                if (knowPlayerLastPosition) {
                    if (!capturePlayerPostition) {
                        playerLastPosition = playerCentroid;
                        capturePlayerPostition = true;
                    }
                    desiredRotation = Quaternion.LookRotation(new Vector3(playerLastPosition.x, gunTip.transform.position.y, playerLastPosition.z) - gunTip.transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 3f);
                } else {
                    // Cannot see player and don't know player's position

                }
            }

            // Will lost tracking of player after 10s
            if (knowPlayerLastPosition) {
                LostPlayerPositionTimer();
            } else {
                lostPlayerTimer = 0f;
            }

        } else {
            // Make NPCs have the correct rotation when they die
            if (canSeePlayer) {
                desiredRotation = Quaternion.LookRotation(diePlayerPosition - gunTip.transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 3f);
            }

            SelfExplode();
        }
    }

    // Keep detecting player
    private IEnumerator FOVRoutine() {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true) {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    
    // Single shooting part
    private IEnumerator SingleShoot() {
        while (true)
        {   
            float shootingDelay = 0.5f;
            WaitForSeconds wait = new WaitForSeconds(shootingDelay);

            if (canSeePlayer && !die)
            {
                GameObject newObject = Instantiate(bullet, gunTip.position, gunTip.rotation);
                newObject.transform.GetChild(0).GetComponent<Bullet>().shootingDirection = shootingDirection;
                newObject.transform.GetChild(0).GetComponent<Bullet>().speed = bulletSpeed;
            }
            yield return wait; // next shot will be shot after this delay
        }
    }


    // Detecting player
    private void FieldOfViewCheck() {
        Collider[] rangeChecks = Physics.OverlapSphere(head.position, radius, playerMask);

        if (rangeChecks.Length != 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - head.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(head.position, directionToTarget, distanceToTarget, obstructionMask)) {
                    canSeePlayer = true;
                    knowPlayerLastPosition = true;
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

    private void LostPlayerPositionTimer() {
        lostPlayerTimer += Time.deltaTime;
        if (lostPlayerTimer >= timeToLost) {
            knowPlayerLastPosition = false;
        }
    }

    private void SelfExplode() {
        selfExplodeTimer += Time.deltaTime;
        if (selfExplodeTimer >= timeToExplode) {
            Instantiate(deathExplosionEffect, explosionPoint.position, deathExplosionEffect.transform.rotation, null);
            Destroy(gameObject);
        }
    }

    private Vector3 IterativeApproximation(Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed) 
    { 
        if (playerMove.isSliding || !sway.canSlide) {
            projectileSpeed *= 20;
        }
        float t = 0.0f; 
        for (int iteration = 0; iteration < MAX_ITERATIONS; ++iteration) 
        { 
            float old_t = t;
            t = Vector3.Distance(gunTip.transform.position, targetPosition + t * targetVelocity) / projectileSpeed;
            if (Mathf.Abs(t - old_t) < EPSILON)
            { 
                break;  
            }
        }
        return targetPosition + t * targetVelocity - gunTip.transform.position; 
    }
}
