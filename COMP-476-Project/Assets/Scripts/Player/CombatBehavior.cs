﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;
    public bool Attacking; // if true, means you are using your bow, this value is set to true after build phase is over

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
            Debug.DrawRay(LaunchPoint.position, LaunchPoint.forward, Color.yellow);
        }
        else if(Input.GetMouseButton(0) && !Attacking)
        {
            Attacking = true;
            EquipWeapon();
            animator.SetLayerWeight(1, 1);
        }

        if (Input.GetMouseButtonUp(0) && Attacking)
        {
            if (AttackTarget == null)
                AcquireTarget();

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
                    obj.GetComponent<Rigidbody>().AddForce(shotDirection.normalized * 25f, ForceMode.Impulse);

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
}
