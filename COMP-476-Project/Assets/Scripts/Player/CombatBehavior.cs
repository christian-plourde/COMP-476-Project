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
    public GameObject HeldArrow;
    public GameObject AbilityCircle;

    float mouseClickTime=0f;

    PlayerMovement PlayerMovementRef;



    [HideInInspector]public bool ultimateCooldown;
    float ultimateCooldownTimer = 0;

    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        PlayerMesh = transform.GetChild(0);
        HandBow.SetActive(false);

        HeldArrow.SetActive(false);

        PlayerMovementRef=GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerMovementRef.controlLock)
            Controls();

        if (ultimateCooldown)
        {
            ultimateCooldownTimer += Time.deltaTime;
            if (ultimateCooldownTimer > 30f)
            {
                ultimateCooldown = false;
                ultimateCooldownTimer = 0;
            }
        }
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
                ArcherArrowSheath();
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
            if (mouseClickTime > 0.9f)
            {
                animator.SetBool("Shot", true);
                animator.SetBool("Shooting", false);
            }
            else
            {
                animator.SetBool("Shot", false);
                animator.SetBool("Shooting", false);
                Invoke("ArcherArrowSheath",0.4f);
            }
            //Shoot();
            mouseClickTime = 0;
        }


        // ultimate
        if(Attacking && Input.GetMouseButtonDown(1) && !ultimateCooldown)
        {
            animator.SetFloat("Movement", 0);
            animator.SetFloat("SprintMultiplier", 1);
            Instantiate(AbilityCircle,transform.position,Quaternion.identity);
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

    public void ArcherShoot()
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

        ArcherArrowSheath();
    }

    public void ArcherArrowSheath()
    {
        HeldArrow.SetActive(false);
    }

    public void ArcherArrowEquip()
    {
        HeldArrow.SetActive(true);
    }

    
}
