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
    public float baseDamage = 4;
    //public GameObject HeldArrow;
    //public GameObject AbilityCircle;

    //float mouseClickTime = 0f;

    PlayerMovement PlayerMovementRef;

    [Header("Attack States")]
    public bool attackingSword;
    public bool fastAttack1;
    public bool fastAttack2;
    public bool fastAttack3;
    public bool usingUltimate;
    public bool kicking;

    float rotateSpeed=350f;
    public float attackTimer = 0f;          // temporary backup to get out of stuck state

    [HideInInspector] public bool ultimateCooldown;
    [HideInInspector] public float ultimateCooldownTimer = 0;
    [HideInInspector] public float ultimateTimer = 0;

    [HideInInspector] public bool secondaryCooldown=false;
    [HideInInspector] public float secondaryCooldownTimer;


    [Header("Cooldown Time Parameters")]
    public float howLongUltimateCooldown;
    public float howLongSecondaryCooldown;


    Vector3 FacingDir;
    Vector3 FixedFacingDir;


    [Header("Warrior AOE Ultimate Prefab")]
    public GameObject WarriorAOEPefab;
    public float AOETime=3f;

    [Header("Leg Reference For Kickin")]
    public GameObject LeftLeg;

    public float BaseDamage
    {
        get { return baseDamage; }
        set { baseDamage = value;
        
            try
            {
                HandSword.GetComponent<BastardSword>().baseDMG = baseDamage;
            }

            catch
            {

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        PlayerMesh = transform.GetChild(0);

        HandSword.SetActive(false);
        BackSword.SetActive(false);

        PlayerMovementRef = GetComponent<PlayerMovement>();

        LeftLeg.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //set the damage of the sword from the player's base damage
        try
        {
            HandSword.GetComponent<BastardSword>().baseDMG = this.baseDamage;
        }

        catch
        { }


        if (!PlayerMovementRef.isDead)
            Controls();

        if (attackingSword || kicking)
        {
            //attackTimer = 0;
            //if(fastAttack3 || fastAttack2)
                transform.Translate(PlayerMesh.transform.forward * 1f *(transform.localScale.x)* Time.deltaTime);
            AttackOrientation();

            attackTimer += Time.deltaTime;
            if (attackTimer > 1.25f)
            {
                attackTimer = 0;

                attackingSword = false;
                animator.SetLayerWeight(2, 0);
                fastAttack1 = false;
                fastAttack2 = false;
                fastAttack3 = false;
                kicking = false;
                animator.SetBool("FastAttack1", false);
                animator.SetBool("FastAttack2", false);
                animator.SetBool("FastAttack3", false);
                animator.SetBool("Kicking", false);



                PlayerMovementRef.controlLock = false;

                HandSword.GetComponent<BoxCollider>().enabled = false;
                attackingSword = false;
            }
        }
        GetFacingDir();

        if (usingUltimate && ultimateTimer<5f)
        {
            PlayerMesh.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            rotateSpeed += 2.5f;
            ultimateTimer += Time.deltaTime;
            animator.SetLayerWeight(2, 1);
            HandSword.GetComponent<BoxCollider>().enabled = true;
            if (ultimateTimer > 5f)
            {
               
                animator.SetBool("UltimateSmash", true);
                PlayerMovementRef.controlLock = true;
                //Debug.Log("Switched to Ultimate Smash");
                GetComponent<Rigidbody>().AddForce(Vector3.up *5f, ForceMode.Impulse);
            }
        }

        if (ultimateCooldown)
        {
            ultimateCooldownTimer += Time.deltaTime;
            if (ultimateCooldownTimer > howLongUltimateCooldown)
            {
                ultimateCooldown = false;
                ultimateCooldownTimer = 0f;
                rotateSpeed = 350f;

            }
        }

        if (secondaryCooldown)
        {
            
            secondaryCooldownTimer += Time.deltaTime;
            if (secondaryCooldownTimer > howLongSecondaryCooldown)
            {
                secondaryCooldownTimer = 0;
                secondaryCooldown = false;
            }
        }


        if (attackingSword || usingUltimate)
        {
            HandSword.GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            HandSword.GetComponent<TrailRenderer>().enabled = false;
        }
    }

    void Controls()
    {
        if (Input.GetKeyDown(KeyCode.E) && !usingUltimate && !attackingSword && !kicking)
        {
            if (Attacking)
            {
                // then we switch to sheath // build mode
                BackSword.SetActive(true);
                HandSword.SetActive(false);
                Attacking = false;
                animator.SetLayerWeight(1, 0);

                PlayerMovementRef.inBuildMode = true;


            }
            else
            {
                //switch to attack mode
                BackSword.SetActive(false);
                HandSword.SetActive(true);
                Attacking = true;
                animator.SetLayerWeight(1, 1);

                PlayerMovementRef.inBuildMode = false;
            }
        }



        if (Input.GetMouseButton(0) && Attacking && !attackingSword && !usingUltimate &&!kicking)
        {
            FixedFacingDir = FacingDir;

            animator.SetLayerWeight(2, 1);

            if (animator.GetBool("FastAttack2"))
                animator.SetBool("FastAttack2", false);

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

        if (Input.GetMouseButton(0) && Attacking && fastAttack3)
        {
            FixedFacingDir = FacingDir;

            //Debug.Log("Chained 3rd Attack");
            attackTimer = 0;

            animator.SetLayerWeight(2, 1);
            animator.SetBool("FastAttack3",true);
            fastAttack3 = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 4f,ForceMode.Impulse);

        }


        //if (Attacking && Input.GetMouseButtonDown(1) && !ultimateCooldown)
        if (Attacking && Input.GetKeyDown(KeyCode.Q) && !ultimateCooldown)
        {
            animator.SetLayerWeight(2,1);
            animator.SetBool("FastAttack1", false);
            animator.SetBool("FastAttack2", false);
            animator.SetBool("FastAttack3", false);
            animator.SetBool("Ultimate",true);
            HandSword.GetComponent<BoxCollider>().enabled = true;
            usingUltimate = true;

            PlayerMovementRef.warriorUltimate = true;
            PlayerMovementRef.mSpeed = PlayerMovementRef.mSpeed * 0.5f;
            PlayerMovementRef.invincible = true;
        }


        // secondary kick
        if (Attacking && Input.GetMouseButtonDown(1) && !usingUltimate && !secondaryCooldown)
        {
            attackTimer = 0;

            animator.SetLayerWeight(2, 1);
            secondaryCooldown = true;

            animator.SetBool("Kicking", true);
            fastAttack1 = false;
            fastAttack2 = false;
            fastAttack3 = false;
            animator.SetBool("FastAttack1",false);
            animator.SetBool("FastAttack2",false);
            animator.SetBool("FastAttack3",false);

            FixedFacingDir = FacingDir;
            kicking = true;

            PlayerMovementRef.controlLock = true;

            HandSword.GetComponent<BoxCollider>().enabled = false;
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
            FixedFacingDir = PlayerMesh.transform.forward;
        }

        Quaternion lookDirection;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(FixedFacingDir, Vector3.up);
        //lookDirection = Quaternion.LookRotation(FacingDir, Vector3.up);
        PlayerMesh.localRotation = Quaternion.RotateTowards(PlayerMesh.localRotation, lookDirection, 25);

    }

    // for resetting everything while dying / respawning.
    public void ResetAllCombat()
    {
        // turn off all attack parameters.
        HandSword.SetActive(false);
        BackSword.SetActive(true);
        LeftLeg.SetActive(false);

        fastAttack1 = false;
        fastAttack2 = false;
        fastAttack3 = false;
        usingUltimate = false;

        animator.SetBool("FastAttack1",false);
        animator.SetBool("FastAttack1",false);
        animator.SetBool("FastAttack1",false);
        animator.SetBool("Ultimate",false);
        animator.SetBool("UltimateSmash",false);
        animator.SetBool("Kick",false);

        animator.SetLayerWeight(2, 0);
        Attacking = false;
    }
}
