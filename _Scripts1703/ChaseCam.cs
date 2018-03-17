using UnityEngine;
using System.Collections;

public class ChaseCam : MonoBehaviour {

    // Ref to player
    private Transform playerTarget;
    // Cam variables
    private float distance = 120.0f;
    private float height = 100.0f;
    private float damping = 5.0f;
    private bool smoothRotation = true;
    private bool followBehind = true;
    private float rotationDamping = 10.0f;

    private void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        Vector3 newPos;
        
        if (followBehind)
        {
            newPos = playerTarget.TransformPoint(0, height, -distance);
        }
        else
        {
            newPos = playerTarget.TransformPoint(0, height, distance);
        }
        // Chase player
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * damping);

        if (smoothRotation)
        {
            Quaternion newRot = Quaternion.LookRotation(playerTarget.position - transform.position, playerTarget.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * rotationDamping);
        }
        else
        {
            transform.LookAt(playerTarget, playerTarget.up);
        }
    }
}
