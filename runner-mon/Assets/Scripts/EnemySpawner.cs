using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public float enemyHealth = 80;
    [SerializeField] float enemyAttackDamage = 10;
    [SerializeField] GameObject fireEnemy, waterEnemy;
    private bool isEnemySet;

    [SerializeField] float attackDelay = 4f;
    private bool hasStartedAttack;
    private bool enemyCanAttack;

    public bool isDead;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Attacks once then stands in place
        fireEnemy.GetComponent<Animator>().SetTrigger("Attack");
        waterEnemy.GetComponent<Animator>().SetTrigger("Attack");
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

        if (PlayerController.instance.hasReachedEnd && PlayerController.instance.isAlive && !hasStartedAttack && !isDead && PlayerController.instance.isAlive && PlayerController.instance.hasStartedAttack)
        {
            print("Starting Attack");
            enemyCanAttack = true;
            hasStartedAttack = true;
            StartCoroutine(EnemyAttack());
        }
        if (isDead || !PlayerController.instance.isAlive)
        {
            waterEnemy.GetComponent<Animator>().ResetTrigger("Attack");
            fireEnemy.GetComponent<Animator>().ResetTrigger("Attack");
        }
    }

    public void GetHurt()
    {
        if (!isDead)
        {
            // i.e the water enemy is activated
            if (PlayerController.instance.isFireType)
            {
                waterEnemy.GetComponent<Animator>().SetTrigger("Hurt");

            }

            // i.e the fire enemy is activated
            if (PlayerController.instance.isWaterType)
            {
                fireEnemy.GetComponent<Animator>().SetTrigger("Hurt");
            }
        }
    }

    // Will be called externally by the playerController
    public void TakeDamage(float damage)
    {
        if (enemyHealth > 0)
        {
            enemyHealth -= damage;
            GetHurt();
        }
        else if (enemyHealth <= 0)
        {
            enemyCanAttack = false;
            print("Resetting attack");

            waterEnemy.GetComponent<Animator>().ResetTrigger("Attack");
            fireEnemy.GetComponent<Animator>().ResetTrigger("Attack");

            Die();
        }
    }

    private void Die()
    {
        if (!isDead)
        {
            enemyCanAttack = false;
            // i.e the water enemy is activated
            if (PlayerController.instance.isFireType)
            {
                waterEnemy.GetComponent<Animator>().SetBool("Die", true);
            }

            // i.e the fire enemy is activated
            if (PlayerController.instance.isWaterType)
            {
                print("Setting Die trigger");
                fireEnemy.GetComponent<Animator>().SetBool("Die", true);
            }
            isDead = true;
            enemyCanAttack = false;
            PlayerController.instance.canAttack = false;
            PlayerController.instance.LevelEnd(1);
        }
    }

    public IEnumerator EnemyAttack()
    {
        if (enemyCanAttack && !isDead && PlayerController.instance.isAlive)
        {
            yield return new WaitForSeconds(attackDelay);
            // i.e the water enemy is activated
            if (PlayerController.instance.isFireType)
            {
                waterEnemy.GetComponent<Animator>().SetTrigger("Attack");
                PlayerController.instance.TakeDamage(enemyAttackDamage);

                StartCoroutine(EnemyAttack());
            }

            // i.e the fire enemy is activated
            else if (PlayerController.instance.isWaterType && !isDead)
            {
                print("Setting attack trigger on charizard");
                fireEnemy.GetComponent<Animator>().SetTrigger("Attack");
                PlayerController.instance.TakeDamage(enemyAttackDamage);

                StartCoroutine(EnemyAttack());
            }
            else
            {
                enemyCanAttack = false;
                yield return null;
            }

        }
        else { yield return null; }
    }

}
