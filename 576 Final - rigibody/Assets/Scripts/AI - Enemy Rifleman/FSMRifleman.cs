using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StateType
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

    public int singleShootingHash;

    public Transform gunTip;
    public Transform head;
    public GameObject bullet;
    public Vector3 shootingDirection;
    public Vector3 optimizedPlayerPosition;
    public float bulletSpeed = 30f;

    public bool die = false;
    public Vector3 diePlayerPosition;
    public Transform explosionPoint;
    public GameObject deathExplosionEffect;
    public float selfExplodeTimer = 0f;
    public float timeToExplode = 2.5f;

    public int MAX_ITERATIONS = 10;
    public float EPSILON = 0.08f;
}

public class FSMRifleman : MonoBehaviour
{
    public EnemyRiflemanParameter parameter;
    private FSMState currState;
    private Dictionary<StateType, FSMState> states = new Dictionary<StateType, FSMState>();

    // Start is called before the first frame update
    void Start()
    {
        parameter.animationController = GetComponent<Animator>();
        parameter.player = GameObject.FindWithTag("Player");
        parameter.playerMove = parameter.player.GetComponent<PlayerMove>();
        parameter.wallRun = parameter.player.GetComponent<WallRun>();
        parameter.sway = GameObject.FindWithTag("Head").GetComponent<MoveSway>();

        parameter.singleShootingHash = Animator.StringToHash("ShootingSingleShot");

        StartCoroutine(FOVRoutine());
        StartCoroutine(SingleShoot());

        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.AttackSingle, new AttackSingleState(this));
        states.Add(StateType.Die, new DieState(this));

        TransitionState(StateType.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        parameter.playerCentroid = parameter.player.transform.GetChild(0).GetComponent<CapsuleCollider>().bounds.center;
        
        parameter.optimizedPlayerPosition = IterativeApproximation(parameter.playerCentroid, parameter.playerMove.rb.velocity, parameter.bulletSpeed);
        parameter.optimizedPlayerPosition.Normalize();

        currState.OnUpdate();
    }

    public void TransitionState(StateType state)
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
    private IEnumerator SingleShoot() {
        while (true)
        {   
            float shootingDelay = 0.5f;
            WaitForSeconds wait = new WaitForSeconds(shootingDelay);

            if (parameter.canSeePlayer && !parameter.die)
            {
                GameObject newObject = Instantiate(parameter.bullet, parameter.gunTip.position, parameter.gunTip.rotation);
                newObject.transform.GetChild(0).GetComponent<Bullet>().shootingDirection = parameter.shootingDirection;
                newObject.transform.GetChild(0).GetComponent<Bullet>().speed = parameter.bulletSpeed;
            }
            yield return wait; // next shot will be shot after this delay
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
