using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public Transform orientation;

    [Header("Wall Running")]
    [SerializeField] private float wallDistance;
    [SerializeField] private float minimumJumpHeight;

    public bool isWallLeft = false;
    public bool isWallRight = false;

    // Start is called before the first frame update
    void Start()
    {
        wallDistance = 0.6f;
        minimumJumpHeight = 1.5f;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckWall();

        if(CanWallRun()) {
            if(isWallLeft) {
                Debug.Log("Wall Left");
            }
            if(isWallRight) {
                Debug.Log("Wall Right");
            }
        }
    }

    private void CheckWall() {
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, wallDistance);
        isWallRight = Physics.Raycast(transform.position, orientation.right, wallDistance);
    }

    private bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }
}
