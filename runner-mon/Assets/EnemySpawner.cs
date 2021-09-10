using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject fireEnemy, waterEnemy;
    private bool isEnemySet;


    // Start is called before the first frame update
    void Start()
    {


        // attacks once then stands in place
        fireEnemy.GetComponent<Animator>().SetTrigger("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnemySet)
        {

            if (PlayerController.instance.isFireType)
            {
                fireEnemy.SetActive(false);
                waterEnemy.SetActive(true);
                isEnemySet = true;
            }
            if (PlayerController.instance.isWaterType)
            {
                waterEnemy.SetActive(false);
                fireEnemy.SetActive(true);
                isEnemySet = true;

            }
        }
    }
}
