using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallScript : MonoBehaviour
{

    //ANIMATION EVENT DO NOT DELETE!!!!!!!!
    public void ShootFireBall()
    {
        Instantiate(PlayerController.instance.fireBallFX, PlayerController.instance.fireBallPos.position, Quaternion.identity);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
