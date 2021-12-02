using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponPlace : MonoBehaviour
{
    private PlayerMove playerMovement;
    private MoveSway sway;
    private WallRun wallRun;
    public GrapplingHook playerHook;

    private Vector3 initialPosition;
    private Vector3 leftHandPosition;
    private Vector3 awayPosition;

    private Quaternion initialRotation;
    private Vector3 awayRotationVector;
    private Quaternion awayRotation;

    private Vector3 initialForward;

    private float smooth = 8f;
    private float swayMultiplier = 20f;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        sway = GameObject.Find("Head").GetComponent<MoveSway>();
        wallRun = GameObject.FindWithTag("Player").GetComponent<WallRun>();
        playerHook = GameObject.FindWithTag("Player").GetComponent<GrapplingHook>();

        initialPosition = transform.localPosition;
        leftHandPosition = new Vector3(initialPosition.x - 0.3f, initialPosition.y, initialPosition.z);
        awayPosition = new Vector3(transform.localPosition.x - 0.4f, initialPosition.y - 0.1f, initialPosition.z - 0.6f);

        initialRotation = transform.localRotation;
        awayRotationVector = new Vector3(-10f, 90f, 0f);
        awayRotation = Quaternion.Euler(awayRotationVector);

        initialForward = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // Gun sway
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up) * Quaternion.LookRotation(initialForward, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        if (sway.isWallRight() || wallRun.isWallRight) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, leftHandPosition, Time.deltaTime * 5f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.deltaTime * 6f);
        }
        
        if (sway.isWallAhead()) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, awayPosition, Time.deltaTime * 10f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, awayRotation, Time.deltaTime * 8f);
        }

        if (!sway.isWallAhead() && !sway.isWallRight() && !wallRun.isWallRight && !playerHook.play) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 8f);
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.deltaTime * 10f);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
    }
}
