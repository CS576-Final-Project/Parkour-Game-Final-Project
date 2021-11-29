using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRifleman : MonoBehaviour
{
    public Animator animationController;

    public float radius;
    public float angle;

    public GameObject player;

    public LayerMask playerMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animator>();

        player = GameObject.FindWithTag("Player");
        StartCoroutine(FOVRoutine());

        radius = 50f;
        angle = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FOVRoutine() {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true) {
            yield return wait;
            FieldOfViewCheck();
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
