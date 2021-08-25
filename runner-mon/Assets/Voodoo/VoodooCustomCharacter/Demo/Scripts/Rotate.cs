using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public float speed;
    public Vector3 eulerAngles;

	void Update () {
        transform.Rotate(eulerAngles * speed * Time.deltaTime);
	}
}
