using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    // Start is called before the first frame update
    int health=10;
    bool isDead;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "PlayerWeapon")
        {
            health -= 2;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (health < 0)
            {
                isDead = true;
                GetComponent<ZombieMovementTest>().SetDead(true);
                animator.SetBool("Dead", true);
                Debug.Log("HE IS DEAD");
            }
        }
    }
}
