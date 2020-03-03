using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttributes : MonoBehaviour
{
    public float health;
    float maxHealth;
    public float damage;
    public float speed;
    public bool isDead;
    public float heightOffset;

    Animator animator;

    //UI
    public Image HealthUI;

    // Start is called before the first frame update
    void Start()
    {       
        maxHealth = health;
        animator = GetComponent<Animator>();
    }

    public float GetHeightOffset()
    {
        return heightOffset;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public float GetHealth()
    {
        return health;
    }

    public void DealDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            health = 0;
            KillEnemy();
        }
        float healthPercent = ((health * 1.0f) / maxHealth);
        HealthUI.transform.localScale =new Vector3( healthPercent,1,1);
    }

    public void KillEnemy()
    {
        animator.SetBool("Dead", true);
        isDead = true;
        GameObject playerRef = GameObject.FindGameObjectWithTag("Player");
        // check if its warrior or archer

        if (playerRef.GetComponent<PlayerMovement>().playerClass == "Archer")
        {

            if (playerRef.GetComponent<CombatBehavior>().AttackTarget != null
                &&
                playerRef.GetComponent<CombatBehavior>().AttackTarget.name == transform.name)
            {
                playerRef.GetComponent<CombatBehavior>().AttackTarget = null;
                playerRef.GetComponent<CombatBehavior>().AcquireTarget();
            }
            transform.GetChild(0).gameObject.SetActive(false);
        }

        // remove UI Health display & destroy object in few seconds, remove own collider
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        HealthUI.enabled = false;
        Destroy(this.gameObject, 1.5f);
    }

}
