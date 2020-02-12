using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttributes : MonoBehaviour
{
    public int health;
    public int damage;
    public float speed;
    public bool isDead;

    Animator animator;

    //UI
    public Image HealthUI;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }



    public void DealDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            health = 0;
            KillEnemy();
        }
        HealthUI.transform.localScale =new Vector3( health / 10f,1,1);
    }

    public void KillEnemy()
    {
        animator.SetBool("Dead", true);
        isDead = true;
    }

}
