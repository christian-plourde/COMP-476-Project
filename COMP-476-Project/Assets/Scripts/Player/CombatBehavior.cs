using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;
    bool Attacking; // if true, means you are using your bow, this value is set to true after build phase is over

    public GameObject ProjectilePrefab;
    public Transform LaunchPoint;

    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
    }

    void Controls()
    {
        // input just for testing
        if (Input.GetKeyDown(KeyCode.E))
        {
            //set uppper body layer weight to 1
            if (Attacking)
            {
                Attacking = false;
                animator.SetLayerWeight(1,0);
                animator.SetBool("Shot", false);
                animator.SetBool("Shooting", false);
            }
            else
            {
                Attacking = true;
                animator.SetLayerWeight(1, 1);
            }
        }

        // shooting controls
        if (Input.GetMouseButtonDown(0) && Attacking)
        {
            animator.SetBool("Shot", false);
            animator.SetBool("Shooting",true);
        }

        if (Input.GetMouseButtonUp(0) && Attacking)
        {
            animator.SetBool("Shooting", false);
            animator.SetBool("Shot", true);
        }
    }

    void EquipWeapon()
    { }

    void UnEquipWeapon()
    { }

    void ShootArrow()
    {

    }

}
