using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorCombatBehavior : MonoBehaviour
{


    private Animator animator;
    public bool Attacking; // if in attack mode, if false, you can build

    //public GameObject ProjectilePrefab;
    //public Transform LaunchPoint;
    //public Transform AttackTarget;

    Transform PlayerMesh;

    [Header("Weapon Slots")]
    public GameObject BackSword;
    public GameObject HandSword;
    //public GameObject HeldArrow;
    //public GameObject AbilityCircle;

    //float mouseClickTime = 0f;

    PlayerMovement PlayerMovementRef;

    [Header("Attack States")]
    public bool attackingSword;
    public bool fastAttack1;
    public bool fastAttack2;
    public float attackTimer = 0f;          // temporary backup to get out of stuck state

    [HideInInspector] public bool ultimateCooldown;
    float ultimateCooldownTimer = 0;


    Vector3 FacingDir;
    Vector3 FixedFacingDir;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        PlayerMesh = transform.GetChild(0);

        HandSword.SetActive(false);
        BackSword.SetActive(false);

        PlayerMovementRef = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!PlayerMovementRef.controlLock)
            Controls();

        if (attackingSword)
        {
            //attackTimer = 0;
            transform.Translate(PlayerMesh.transform.forward * 1f * Time.deltaTime);
            AttackOrientation();

            attackTimer += Time.deltaTime;
            if (attackTimer > 1.4f)
            {
                attackTimer = 0;

                attackingSword = false;
                animator.SetLayerWeight(2, 0);
                animator.SetBool("FastAttack1", false);
                animator.SetBool("FastAttack2", false);



                PlayerMovementRef.controlLock = false;

                HandSword.GetComponent<BoxCollider>().enabled = false;
                attackingSword = false;
            }
        }
        GetFacingDir();
    }

    void Controls()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Attacking)
            {
                // then we switch to sheath // build mode
                BackSword.SetActive(true);
                HandSword.SetActive(false);
                Attacking = false;
                animator.SetLayerWeight(1, 0);

            }
            else
            {
                //switch to attack mode
                BackSword.SetActive(false);
                HandSword.SetActive(true);
                Attacking = true;
                animator.SetLayerWeight(1, 1);
            }
        }



        if (Input.GetMouseButton(0) && Attacking && !attackingSword)
        {
            FixedFacingDir = FacingDir;

            animator.SetLayerWeight(2, 1);
            animator.SetBool("FastAttack1",true);
            attackingSword = true;
            PlayerMovementRef.controlLock = true;
        }

        
        if (Input.GetMouseButton(0) && Attacking && fastAttack2)
        {
            FixedFacingDir = FacingDir;

            //Debug.Log("Chained 2nd attack.");
            attackTimer = 0;
            animator.SetLayerWeight(2, 1);
            animator.SetBool("FastAttack2", true);
        }
        

    }



    void GetFacingDir()
    {
        FacingDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            FacingDir += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            FacingDir += transform.right*-1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            FacingDir += transform.forward*-1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            FacingDir += transform.right;
        }

        Vector3 drawPoint = transform.position;
        drawPoint.y += 0.5f;
        Debug.DrawRay(transform.position, FacingDir, Color.yellow);
    }

    void AttackOrientation()
    {
        if (FixedFacingDir == Vector3.zero)
        {
            FixedFacingDir = transform.forward;
        }

        Quaternion lookDirection;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(FixedFacingDir, Vector3.up);
        //lookDirection = Quaternion.LookRotation(FacingDir, Vector3.up);
        PlayerMesh.localRotation = Quaternion.RotateTowards(PlayerMesh.localRotation, lookDirection, 25);

    }
}
