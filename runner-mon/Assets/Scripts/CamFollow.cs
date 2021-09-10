using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Vector3 offset;
    public GameObject target;
    public float smoothSpeed = 0.125f;

    public static CamFollow instance;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 desiredPos = target.transform.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;

        // transform.LookAt(target.transform);

    }
}