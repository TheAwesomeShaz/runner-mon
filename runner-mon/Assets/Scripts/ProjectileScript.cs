using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] Transform enemyFireBallPos;
    [SerializeField] Transform[] enemyWaterballPos;

    // ANIMATION EVENT DO NOT DELETE!!!!!!!!
    public void ShootFireBall()
    {
        PlayerController.instance.UpdateAttackUI();
        if (PlayerController.instance.isFireType)
        {
            var fx = Instantiate(PlayerController.instance.fireBallFX, PlayerController.instance.fireBallPos.position, Quaternion.identity);
            Destroy(fx, 3f);
        }

        // which means that waterball is fired by the player
        if (PlayerController.instance.isWaterType)
        {
            foreach (Transform waterBallPos in PlayerController.instance.waterBallPos)
            {
                var fx = Instantiate(PlayerController.instance.waterBallFX, waterBallPos.position, Quaternion.Euler(0f, 0f, 0f));
                Destroy(fx, 3f);
            }
        }

    }

    public void EnemyShootFireBall()
    {
        PlayerController.instance.UpdateAttackUI();
        if (PlayerController.instance.isWaterType)
        {
            var fx = Instantiate(PlayerController.instance.fireBallFX, enemyFireBallPos.position, Quaternion.Euler(0f, 180f, 0f));
            Destroy(fx, 3f);
            print("enemy is shooting a fireBall");
            if (!PlayerController.instance.hasEvolvedFinal)
            {
                PlayerController.instance.Die();
            }
        }

        // which means that waterEnemyisActive
        if (PlayerController.instance.isFireType)

        {
            foreach (Transform waterBallPos in enemyWaterballPos)
            {
                var fx = Instantiate(PlayerController.instance.waterBallFX, waterBallPos.position, Quaternion.Euler(0f, 180f, 0f));
                Destroy(fx, 3f);

            }
            if (!PlayerController.instance.hasEvolvedFinal)
            {
                PlayerController.instance.Die();
            }

        }
    }
}
