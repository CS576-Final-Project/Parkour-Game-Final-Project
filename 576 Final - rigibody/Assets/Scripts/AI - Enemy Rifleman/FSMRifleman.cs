using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StateTypeRifleman
{
    Idle, Patrol, LookForPlayer, AttackSingle, Die
}

[Serializable]
public class EnemyRiflemanParameter
{
    public Animator animationController;
    public GameObject player;
    public PlayerMove playerMove;
    public WallRun wallRun;
    public MoveSway sway;

    public float health = 100f;
    public Vector3 playerCentroid;

    // For detecting player
    public float radius = 80f;
    public float angle = 120f;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    public bool capturePlayerPostition = false;
    public bool knowPlayerLastPosition = false;
    public Vector3 playerLastPosition;
    public float lostPlayerTimer = 0f;
    public float timeToLost = 10f;

    public float walkingVelocity;
    public float runningVelocity;

    public int singleShotReadyHash;
    public int singleShootingHash;
    public int dieHash;

    public Transform gunTip;
    public Transform head;
    public Transform parallelRotation;
    public Transform lights;
    public GameObject bullet;
    public Vector3 shootingDirection;
    public Vector3 optimizedPlayerPosition;
    public float bulletSpeed = 30f;

    public bool die = false;
    public Quaternion diePlayerRotation;
    public Transform explosionPoint;
    public GameObject deathExplosionEffect;
    public float selfExplodeTimer = 0f;
    public float timeToExplode = 2.5f;

    public int MAX_ITERATIONS = 10;
    public float EPSILON = 0.08f;

    public AudioSource source;
    public AudioClip oneShotClip;

    public bool oneShotPlayed = false;
}

public class FSMRifleman : MonoBehaviour
{
    public EnemyRiflemanParameter parameter;
    private FSMState currState;
    private Dictionary<StateTypeRifleman, FSMState> states = new Dictionary<StateTypeRifleman, FSMState>();
    public bool coroutineActive = true;
    public HealthBar healthBar;  // enemy health bar

    // Start is called before the first frame update
    void Start()
    {
        parameter.animationController = GetComponent<Animator>();
        parameter.player = GameObject.FindWithTag("Player");
        parameter.playerMove = parameter.player.GetComponent<PlayerMove>();
        parameter.wallRun = parameter.player.GetComponent<WallRun>();
        parameter.sway = GameObject.FindWithTag("Head").GetComponent<MoveSway>();

        parameter.singleShotReadyHash = Animator.StringToHash("SingleShootReady");
        parameter.singleShootingHash = Animator.StringToHash("ShootingSingleShot");
        parameter.dieHash = Animator.StringToHash("Die");

        parameter.source = parameter.gunTip.GetComponent<AudioSource>();

        StartCoroutine(FOVRoutine());
        //StartCoroutine(SingleShoot());

        states.Add(StateTypeRifleman.Idle, new IdleState(this));
        states.Add(StateTypeRifleman.AttackSingle, new AttackSingleState(this));
        states.Add(StateTypeRifleman.Die, new DieState(this));

        TransitionState(StateTypeRifleman.Idle);
        
        healthBar.SetHealth(parameter.health);  // set health at start
    }

    // Update is called once per frame
    void Update()
    {
        parameter.playerCentroid = parameter.player.transform.GetChild(0).GetComponent<CapsuleCollider>().bounds.center;

        parameter.optimizedPlayerPosition = IterativeApproximation(parameter.playerCentroid, parameter.playerMove.rb.velocity, parameter.bulletSpeed);
        parameter.optimizedPlayerPosition.Normalize();

        if (!coroutineActive) {
            StartCoroutine(FOVRoutine());
        }

        currState.OnUpdate();
    }

    public void TransitionState(StateTypeRifleman state)
    {
        if (currState != null)
            currState.OnExit();
        currState = states[state];
        currState.OnEnter();
    }

    public void OrientationTo(Quaternion desiredRotation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 3f);
    }

    public void TakeDamage(float damage)
    {
        parameter.health -= damage;
        healthBar.SetHealth(parameter.health);  // change health bar if take damage
    }

    private IEnumerator FOVRoutine() {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true) {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    
    // Single shooting part
    public IEnumerator SingleShoot() {
        while (true)
        {  
            yield return new WaitForSeconds(1.2f);
            if (parameter.canSeePlayer && !parameter.die) {
                parameter.lights.gameObject.SetActive(false);
                parameter.animationController.SetBool(parameter.singleShootingHash, true);

                yield return new WaitForSeconds(0.2f); // next shot will be shot after this delay
                parameter.oneShotPlayed = false;
                GameObject newObject = Instantiate(parameter.bullet, parameter.gunTip.position, parameter.gunTip.rotation);
                newObject.transform.GetChild(0).GetComponent<Bullet>().shootingDirection = parameter.shootingDirection;
                newObject.transform.GetChild(0).GetComponent<Bullet>().speed = parameter.bulletSpeed;
                if (!parameter.oneShotPlayed) {
                    parameter.source.PlayOneShot(parameter.oneShotClip);
                    parameter.oneShotPlayed = true;
                }
            }
        }
    }

    // Detecting player
    private void FieldOfViewCheck() {
        Collider[] rangeChecks = Physics.OverlapSphere(parameter.head.position, parameter.radius, parameter.playerMask);

        if (rangeChecks.Length != 0) {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - parameter.head.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < parameter.angle / 2) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(parameter.head.position, directionToTarget, distanceToTarget, parameter.obstructionMask)) {
                    parameter.canSeePlayer = true;
                    parameter.knowPlayerLastPosition = true;
                } else {
                    parameter.canSeePlayer = false;
                }
            } else {
                parameter.canSeePlayer = false;
            }
        } else if (parameter.canSeePlayer) {
            parameter.canSeePlayer = false;
        }
    }

    private Vector3 IterativeApproximation(Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed) 
    { 
        if (parameter.playerMove.isSliding || !parameter.sway.canSlide) {
            projectileSpeed *= 20;
        } else if (!parameter.playerMove.isGrounded()) {
            projectileSpeed *= 30;
        }
        float t = 0.0f; 
        for (int iteration = 0; iteration < parameter.MAX_ITERATIONS; ++iteration) 
        { 
            float old_t = t;
            t = Vector3.Distance(parameter.gunTip.transform.position, targetPosition + t * targetVelocity) / projectileSpeed;
            if (Mathf.Abs(t - old_t) < parameter.EPSILON)
            { 
                break;  
            }
        }
        return targetPosition + t * targetVelocity - parameter.gunTip.transform.position; 
    }
}
