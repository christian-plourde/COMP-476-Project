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

    Transform PlayerMesh;

    [Header("Weapon Slots")]
    public GameObject BackBow;
    public GameObject HandBow;

    public float mouseClickTime=0f;

    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        PlayerMesh = transform.GetChild(0);
        HandBow.SetActive(false);
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
                animator.SetLayerWeight(1, 0);
                animator.SetBool("Shot", false);
                animator.SetBool("Shooting", false);
                UnEquipWeapon();
            }
            else
            {
                Attacking = true;
                EquipWeapon();
                animator.SetLayerWeight(1, 1);
            }
        }

        // shooting controls
        if (Input.GetMouseButton(0) && Attacking)
        {
            animator.SetBool("Shot", false);
            animator.SetBool("Shooting",true);
            mouseClickTime += Time.deltaTime;

            //test
            Vector3 PlayerMeshLookAt = AttackTarget.position;
            PlayerMeshLookAt.y = transform.position.y;

            PlayerMesh.LookAt(PlayerMeshLookAt);

            //Debug.DrawRay(PlayerMesh.position,PlayerMesh.forward,Color.yellow);
            Debug.DrawRay(LaunchPoint.position,LaunchPoint.forward,Color.yellow);
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
    {
        HandBow.SetActive(true);
        BackBow.SetActive(false);
    }

    void UnEquipWeapon()
    {
        HandBow.SetActive(false);
        BackBow.SetActive(true);
    }

    void Shoot()
    {
        

        if (AttackTarget != null)
        {
            if (Vector3.Angle(LaunchPoint.forward, (AttackTarget.position - transform.position)) < 40)
            {
                GameObject obj = Instantiate(ProjectilePrefab, LaunchPoint.position, Quaternion.identity);
                Vector3 shotDirection = (AttackTarget.position - transform.position).normalized;
                shotDirection.y += 0.08f;
                obj.transform.LookAt(AttackTarget.position);

                if(Vector3.Distance(AttackTarget.position,transform.position) > 17.5f)
                    obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 35f, ForceMode.Impulse);
                else
                    obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 20f, ForceMode.Impulse);

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
