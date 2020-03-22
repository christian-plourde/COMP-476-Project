using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;
    public bool Attacking; // if true, means you are using your bow, this value is set to true after build phase is over

    public GameObject ProjectilePrefab;
    public GameObject SpecialProjectilePrefab;
    public Transform LaunchPoint;
    public Transform AttackTarget;

    public float baseDamage = 5;
    private float artOfWarMultiplier = 1;
    private float fastestManAliveMultiplier = 1;

    Transform PlayerMesh;

    [Header("Weapon Slots")]
    public GameObject BackBow;
    public GameObject HandBow;
    public GameObject HeldArrow;
    public GameObject AbilityCircle;

    float mouseClickTime=0f;

    PlayerMovement PlayerMovementRef;

    // audio bool
    bool playingDrawSound = false;


    [HideInInspector]public bool ultimateCooldown;
    public float ultimateCooldownTimer = 0;

    public bool secondaryArrowCooldown;
    public float secondaryArrowCooldownTimer=0;
    bool useSecondaryArrow;

    [Header("Cooldown Time Parameters")]
    public float howLongUltimateCooldown;
    public float howLongSecondaryCooldown;

    public float ArtOfWarMultiplier
    {
        get { return artOfWarMultiplier; }
        set { artOfWarMultiplier = value; }
    }

    public float FastestManAliveMultiplier
    {
        get { return fastestManAliveMultiplier; }
        set { fastestManAliveMultiplier = value; }
    }

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
            if (ultimateCooldownTimer > howLongUltimateCooldown)
            {
                ultimateCooldown = false;
                ultimateCooldownTimer = 0;
            }
        }

        if (secondaryArrowCooldown)
        {
            secondaryArrowCooldownTimer += Time.deltaTime;
            if (secondaryArrowCooldownTimer > howLongSecondaryCooldown)
            {
                secondaryArrowCooldown = false;
                secondaryArrowCooldownTimer = 0;
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

                PlayerMovementRef.inBuildMode = true;
            }
            else
            {
                Attacking = true;
                EquipWeapon();
                animator.SetLayerWeight(1, 1);

                PlayerMovementRef.inBuildMode = false;

            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (AttackTarget == null)
                AcquireTarget();
            else
                AcquireNextTarget();
        }



        // shooting controls
        if (Input.GetMouseButton(0) && Attacking)
        {
            animator.SetBool("Shot", false);
            animator.SetBool("Shooting", true);
            mouseClickTime += Time.deltaTime;

            //test
            if (AttackTarget != null)
            {
                Vector3 PlayerMeshLookAt = AttackTarget.position;
                PlayerMeshLookAt.y = transform.position.y;
                PlayerMesh.LookAt(PlayerMeshLookAt);
            }
            //Debug.DrawRay(PlayerMesh.position,PlayerMesh.forward,Color.yellow);
            //Debug.DrawRay(LaunchPoint.position, LaunchPoint.forward, Color.yellow);
            if (!playingDrawSound)
            {
                playingDrawSound = true;
                SFXManager.instance.Play("BowDraw");
            }
        }
        /*
        else if(Input.GetMouseButton(0) && !Attacking)
        {
            Attacking = true;
            EquipWeapon();
            animator.SetLayerWeight(1, 1);
        }
        */
        if (Input.GetMouseButtonUp(0) && Attacking)
        {
            if (AttackTarget == null)
                AcquireTarget();

            if (mouseClickTime > 0.9f)
            {
                animator.SetBool("Shot", true);
                animator.SetBool("Shooting", false);
                playingDrawSound = false;
            }
            else
            {
                animator.SetBool("Shot", false);
                animator.SetBool("Shooting", false);
                Invoke("ArcherArrowSheath",0.4f);
                SFXManager.instance.Stop("BowDraw");
                playingDrawSound = false;
            }
            //Shoot();
            mouseClickTime = 0;
        }


        if (Input.GetMouseButton(1) && Attacking && !secondaryArrowCooldown)
        {
            animator.SetBool("Shot", false);
            animator.SetBool("Shooting", true);
            mouseClickTime += Time.deltaTime;

            if (!playingDrawSound)
            {
                playingDrawSound = true;
                SFXManager.instance.Play("BowDraw");
            }

            //test
            if (AttackTarget != null)
            {
                Vector3 PlayerMeshLookAt = AttackTarget.position;
                PlayerMeshLookAt.y = transform.position.y;
                PlayerMesh.LookAt(PlayerMeshLookAt);
            }
        }

        if (Input.GetMouseButtonUp(1) && Attacking && !secondaryArrowCooldown)
        {
            if (AttackTarget == null)
                AcquireTarget();

            if (mouseClickTime > 0.9f)
            {
                animator.SetBool("Shot", true);
                animator.SetBool("Shooting", false);
                secondaryArrowCooldown = true;
                useSecondaryArrow = true;

                playingDrawSound = false;
            }
            else
            {
                animator.SetBool("Shot", false);
                animator.SetBool("Shooting", false);
                Invoke("ArcherArrowSheath", 0.4f);
                SFXManager.instance.Stop("BowDraw");
                playingDrawSound = false;

            }
            //Shoot();
            mouseClickTime = 0;
        }



        // ultimate
        //if (Attacking && Input.GetMouseButtonDown(1) && !ultimateCooldown)
        if (Attacking && Input.GetKeyDown(KeyCode.T) && !ultimateCooldown)
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
            float specialMult = 1;
            if (Vector3.Angle(LaunchPoint.forward, (AttackTarget.position - transform.position)) < 40)
            {
                GameObject obj;
                if (useSecondaryArrow)
                {
                    obj = Instantiate(SpecialProjectilePrefab, LaunchPoint.position, Quaternion.identity);
                    useSecondaryArrow = false;
                    specialMult = 1.2f;
                }
                else
                {
                    obj = Instantiate(ProjectilePrefab, LaunchPoint.position, Quaternion.identity);
                    try
                    {
                        obj.GetComponent<Arrow>().SetArrowDamage(baseDamage * artOfWarMultiplier * fastestManAliveMultiplier);
                        //Debug.Log("Arrow damage is: " + obj.GetComponent<Arrow>().baseDamage);
                    }

                    catch
                    {
                        
                    }
                }
                    

                Vector3 shotDirection = (AttackTarget.position - transform.position).normalized;
                shotDirection.y += AttackTarget.GetComponent<EnemyAttributes>().GetHeightOffset();
                obj.transform.LookAt(AttackTarget.position);

                //Debug.Log("Distance to Target: "+ Vector3.Distance(AttackTarget.position, transform.position));
                if(Vector3.Distance(AttackTarget.position,transform.position) > 3f)
                    obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 20f*specialMult, ForceMode.Impulse);
                //obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 35f*(transform.localScale.x)*specialMult, ForceMode.Impulse);  // old
                else if(Vector3.Distance(AttackTarget.position, transform.position) > 4.5f)
                    obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 30f * specialMult, ForceMode.Impulse);
                else
                    obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 12.5f , ForceMode.Impulse);
                    //obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 25f * (transform.localScale.x), ForceMode.Impulse);            // old

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





    // Target Acquiring and switching methods
    public void AcquireTarget()
    {
        Collider[] arr = Physics.OverlapSphere(transform.position, 45);

        float closestTarget = 1000f;
        GameObject potentialTarget = null;
        foreach (Collider c in arr)
        {
            if (c.tag == "Enemy")
            {
                if (!c.GetComponent<EnemyAttributes>().isDead)
                {
                    if (Vector3.Distance(c.transform.position, transform.position) < closestTarget)
                    {
                        closestTarget = Vector3.Distance(c.transform.position, transform.position);
                        potentialTarget = c.gameObject;
                    }
                }
            }
        }



        if (potentialTarget != null)
        {
            AttackTarget = potentialTarget.transform;
            if (AttackTarget.transform.GetChild(0).name == "TargetMesh")
            {
                AttackTarget.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Missing Targetmesh on target or wrong index in hierarchy. Object: "+AttackTarget.name);
            }
        }
    }


    void AcquireNextTarget()
    {
        // try to find someone else who is not current target if possible.
        // switch to new target. If no new target available dont do anything
        Collider[] arr = Physics.OverlapSphere(transform.position, 45);

        float closestTarget = 1000f;
        GameObject potentialTarget = null;
        foreach (Collider c in arr)
        {
            if (c.tag == "Enemy")
            {
                if (!c.GetComponent<EnemyAttributes>().isDead && c.name != AttackTarget.name)
                {
                    if (Vector3.Distance(c.transform.position, transform.position) < closestTarget)
                    {
                        closestTarget = Vector3.Distance(c.transform.position, transform.position);
                        potentialTarget = c.gameObject;
                    }
                }
            }
        }

        if (potentialTarget != null)
        {
            // turn off targetmesh for old target
            AttackTarget.transform.GetChild(0).gameObject.SetActive(false);


            AttackTarget = potentialTarget.transform;
            if (AttackTarget.transform.GetChild(0).name == "TargetMesh")
            {
                AttackTarget.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Missing Targetmesh on target or wrong index in hierarchy. Object: " + AttackTarget.name);
            }
        }
    }


    public void ResetAllCombat()
    {
        // turn off all attack parameters.
        BackBow.SetActive(true);
        HandBow.SetActive(false);
        HeldArrow.SetActive(false);


        animator.SetBool("Shot", false);
        animator.SetBool("Shooting", false);

        animator.SetLayerWeight(1, 0);
        Attacking = false;

        if (AttackTarget != null)
        {
            AttackTarget.transform.GetChild(0).gameObject.SetActive(false);
            AttackTarget = null;
        }
    }
}
