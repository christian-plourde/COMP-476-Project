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
    public Transform AttackTarget;

    public float mouseClickTime=0f;

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
        if (Input.GetMouseButton(0) && Attacking)
        {
            animator.SetBool("Shot", false);
            animator.SetBool("Shooting",true);
            mouseClickTime += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && Attacking)
        {
            
            animator.SetBool("Shot", true);
            animator.SetBool("Shooting", false);
            Shoot();
            mouseClickTime = 0;
        }
    }

    void EquipWeapon()
    { }

    void UnEquipWeapon()
    { }

    void Shoot()
    {
        

        if (AttackTarget != null)
        {
            if (Vector3.Angle(LaunchPoint.forward, (AttackTarget.position - transform.position)) < 40)
            {
                GameObject obj = Instantiate(ProjectilePrefab, LaunchPoint.position, Quaternion.identity);
                obj.GetComponent<Rigidbody>().AddForce((AttackTarget.position - transform.position).normalized * 25f, ForceMode.Impulse);
            }
            else
            {
                animator.SetBool("Shooting", false);
            }
        }
        else
        {
            animator.SetBool("Shooting", false);
        }
    }

}
