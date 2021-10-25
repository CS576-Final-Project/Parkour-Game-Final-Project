using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponPlace : MonoBehaviour
{
    private PlayerMove playerMovement;
    private MoveSway sway;

    private Vector3 initionalPosition;
    private Vector3 leftHandPosition;
    private Vector3 awayPosition;

    private Quaternion initionalRotation;
    private Vector3 awayRotationVector;
    private Quaternion awayRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        sway = GameObject.Find("Head").GetComponent<MoveSway>();

        initionalPosition = transform.localPosition;
        leftHandPosition = new Vector3(-initionalPosition.x, initionalPosition.y, initionalPosition.z);
        awayPosition = new Vector3(transform.localPosition.x - 0.2f, initionalPosition.y, initionalPosition.z - 0.6f);

        initionalRotation = transform.localRotation;
        awayRotationVector = new Vector3(90f, 180f, 0f);
        awayRotation = Quaternion.Euler(awayRotationVector);
    }

    // Update is called once per frame
    void Update()
    {
        if (sway.isWallRight()) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, leftHandPosition, Time.deltaTime * 5f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initionalRotation, Time.deltaTime * 6f);
        } 
        if (sway.isWallAhead()) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, awayPosition, Time.deltaTime * 6f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, awayRotation, Time.deltaTime * 6f);
        }
        if (!sway.isWallAhead() && !sway.isWallRight()) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initionalPosition, Time.deltaTime * 6f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initionalRotation, Time.deltaTime * 6f);
        }
    }
}
