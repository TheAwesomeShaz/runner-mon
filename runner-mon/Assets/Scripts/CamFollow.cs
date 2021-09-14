using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 attackCamOffset;
    Vector3 desiredPos;
    bool isFollowing;

    public GameObject target;
    public float smoothSpeed = 0.125f;
    public bool isAttackCam;

    public static CamFollow instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFollowing = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFollowing)
        {

            if (isAttackCam)
            {
                desiredPos = target.transform.position + attackCamOffset;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(20f, -90f, 0f), smoothSpeed * Time.deltaTime);
                Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
                transform.position = smoothedPos;


            }
            else
            {
                desiredPos = target.transform.position + offset;
                // transform.rotation = Quaternion.Euler(20f, -15f, 0f);

                Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, smoothedPos.y, smoothedPos.z);

            }
        }


        // transform.LookAt(target.transform);

    }
}