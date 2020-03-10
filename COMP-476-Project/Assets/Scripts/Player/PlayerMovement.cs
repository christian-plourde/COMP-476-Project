﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* 3rd Person
     * 8 Directional Movement with WASD 
     * Will Align with movement direction before running (Align Behavior in this class)
     */


    [Header("Player Variables")]
    public float health = 100f;
    float maxHealth;
    public int gold = 200;
    public bool invincible;
    Vector3 respawnPos;


    [Header("Movement Parameters")]
    float ogSpeed;
    public float mSpeed=5f;
    public float sprintMultiplier=1f;

    private Animator animator;

    //private Transform Target;

    Transform PlayerMesh;

    [Header("Player States")]
    public bool isRunning;
    bool isBuilding;
    public bool controlLock;
    public bool isDead;
    public string playerClass;

    [HideInInspector]public bool warriorUltimate;


    void Start()
    {
        ogSpeed = mSpeed;
        PlayerMesh = transform.GetChild(0);
        Debug.Log("Playermesh is:"+PlayerMesh.name);
        animator = PlayerMesh.GetComponent<Animator>();

        maxHealth = health;

        respawnPos = transform.position;
    }

    void Update()
    {
        if(!isDead && !controlLock)
            Movement();
        // Force rotation = 0
        transform.rotation = Quaternion.Euler(Vector3.zero);


        // respawn test:
        if (isDead && Input.GetKeyDown(KeyCode.R))
            RespawnPlayer();

        if (!isDead && Input.GetKeyDown(KeyCode.K))
            DealDamage(25f);
        
    }

    void Movement()
    {
        Vector3 FacingDirection=Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward.normalized *sprintMultiplier* mSpeed * Time.deltaTime);
            FacingDirection += transform.forward;
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);

        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-1 * transform.forward.normalized * sprintMultiplier * mSpeed * Time.deltaTime);
            FacingDirection+= (transform.forward * -1);
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);

        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right.normalized * sprintMultiplier * mSpeed * Time.deltaTime);
            FacingDirection += transform.right;
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);

        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-1 * transform.right.normalized * sprintMultiplier * mSpeed * Time.deltaTime);
            FacingDirection += (transform.right * -1);
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);
        }


        // sprint multipler
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetFloat("SprintMultiplier",1.6f);      // animation speed multipler
            sprintMultiplier = 1.4f;                        // translation speed multiplier
        }
        else
        {
            animator.SetFloat("SprintMultiplier", 1);
            sprintMultiplier = 1;
        }
        //animation:
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            isRunning = true;
            if (animator.GetFloat("Movement") < 1f)
                animator.SetFloat("Movement", animator.GetFloat("Movement")+0.04f);
        }
        else
        {
            isRunning = false;
            if (animator.GetFloat("Movement") > 0f)
                animator.SetFloat("Movement", animator.GetFloat("Movement") - 0.04f);
        }




        // orientation controls
        Debug.DrawRay(transform.position,FacingDirection,Color.red);

        
    }

    

    void AlignOrientation(Vector3 FaceDir)
    {
        
        Quaternion lookDirection;
        //FaceDir.y = 0;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(FaceDir, Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(PlayerMesh.localRotation, lookDirection, 4);

        if(!warriorUltimate)
            PlayerMesh.localRotation = Quaternion.RotateTowards(PlayerMesh.localRotation, lookDirection, 4);


        // PlayerMesh.rotation = Quaternion.Euler(new Vector3(0,PlayerRotAngle,0)) ;
    }


    public void ResetSpeed()
    {
        mSpeed = ogSpeed;
    }



    // for death & respawn methods

    void DealDamage(float dmg)
    {
        if (!invincible)
        {
            health -= dmg;
            if (health <= 0)
            {
                health = 0;
                KillPlayer();
            }
        }
    }

    void KillPlayer()
    {
        controlLock = true;
        isDead = true;

        animator.SetBool("Dead", true);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
    }


    void RespawnPlayer()
    {
        controlLock = false;
        isDead = false;
        animator.SetBool("Dead", false);
        health = maxHealth;
        invincible = false;

        //Reset weapons and stuff.
        if (playerClass == "Warrior")
        {
            WarriorCombatBehavior scriptRef = GetComponent<WarriorCombatBehavior>();
            scriptRef.ResetAllCombat();
        }
        else
        {
            CombatBehavior scriptRef = GetComponent<CombatBehavior>();
        }
        
    }

    // gold exchange functions
    
    public void AddGold(int amount) { }

    public void RemoveGold(int amount) { }
}
