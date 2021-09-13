using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 attackCamOffset;
    Vector3 desiredPos;

    public GameObject target;
    public float smoothSpeed = 0.125f;
    public bool isAttackCam;

    public static CamFollow instance;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttackCam)
        {
            desiredPos = target.transform.position + attackCamOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(20f, -90f, 0f), smoothSpeed * Time.deltaTime);
        }
        else
        {
            desiredPos = target.transform.position + offset;
            transform.rotation = Quaternion.Euler(20f, -25f, 0f);

        }
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;


        // transform.LookAt(target.transform);

    }
}