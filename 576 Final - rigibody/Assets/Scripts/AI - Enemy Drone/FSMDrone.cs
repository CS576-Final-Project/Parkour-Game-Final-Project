using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StateTypeDrone
{
    Idle, Patrol, LookForPlayer, AttackSingle, Die
}

[Serializable]
public class EnemyDroneParameter
{
    public GameObject player;
    public PlayerMove playerMove;
    public WallRun wallRun;
    public MoveSway sway;

    public float health = 300f;
    public Vector3 playerCentroid;

    // For detecting player
    public float radius = 80f;
    public float angle = 170f;
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
    public float bulletSpeed = 20f;

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
    
    public float randomSpeed = 50f;  // random movement speed 

    public Transform leftDirection;  // denote the left direction of drone
    public Transform rightDirection;  // denote the right direction of drone
    public Transform upDirection;  // denote the right direction of drone
    public Transform downDirection;  // denote the right direction of drone
}

public class FSMDrone : MonoBehaviour
{
    public EnemyDroneParameter parameter;
    private FSMState currState;
    private Dictionary<StateTypeDrone, FSMState> states = new Dictionary<StateTypeDrone, FSMState>();
    public bool coroutineActive = true;
    public Transform rightBlender;
    public Transform leftBlender;

    // Start is called before the first frame update
    void Start()
    {
        parameter.player = GameObject.FindWithTag("Player");
        parameter.playerMove = parameter.player.GetComponent<PlayerMove>();
        parameter.wallRun = parameter.player.GetComponent<WallRun>();
        parameter.sway = GameObject.FindWithTag("Head").GetComponent<MoveSway>();

        parameter.source = parameter.gunTip.GetComponent<AudioSource>();

        //StartCoroutine(SingleShoot());

        states.Add(StateTypeDrone.Idle, new DroneIdleState(this));
        states.Add(StateTypeDrone.AttackSingle, new DroneAttackSingleState(this));
        states.Add(StateTypeDrone.Die, new DroneDieState(this));

        TransitionState(StateTypeDrone.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -84.6999969f, -65.3000031f),
                                         Mathf.Clamp(transform.position.y, 109.547279f, 130.11f),
                                         Mathf.Clamp(transform.position.z, -155.009995f, -128.899994f));

        parameter.playerCentroid = parameter.player.transform.GetChild(0).GetComponent<CapsuleCollider>().bounds.center;

        parameter.optimizedPlayerPosition = IterativeApproximation(parameter.playerCentroid, parameter.playerMove.rb.velocity, parameter.bulletSpeed);
        parameter.optimizedPlayerPosition.Normalize();

        if (!coroutineActive) {
        }

        rightBlender.Rotate(0, 800 * Time.deltaTime, 0);
        leftBlender.Rotate(0, 800 * Time.deltaTime, 0);

        currState.OnUpdate();
    }

    public void TransitionState(StateTypeDrone state)
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
    
    // Single shooting part
    public IEnumerator SingleShoot() {
        while (true)
        {  
            yield return new WaitForSeconds(1.2f);
            if (parameter.canSeePlayer && !parameter.die) {
                parameter.lights.gameObject.SetActive(false);

                yield return new WaitForSeconds(0.3f); // next shot will be shot after this delay
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
