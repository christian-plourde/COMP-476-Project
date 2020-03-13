using System.Collections;
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
    public bool inBuildMode;
    public bool building;
    public bool managingTower;

    [HideInInspector]public bool warriorUltimate;
    [Header("References")]
    public GameObject respawnUIPrefab;

    public GridSquare currentGridSquare;
    public GenerateGrid grid;


    public GridSquare GridSquare
    {
        get { return currentGridSquare; }
    }

    //called each update to update the current grid square that the player is on
    public void UpdateGridSquare()
    {
        double smallest_dist = double.MaxValue;
        //foreach of the squares, compare the position of the square to the player's position
        foreach(GridSquare s in grid.GridSquares)
        {
            double curr_dist = (s.Position - this.transform.position).magnitude;
            if (curr_dist < smallest_dist)
            {
                smallest_dist = curr_dist;
                this.currentGridSquare = s;
            }
        }
    }

    void Start()
    {
        // scale speed according to current scale (because player is smaller in the grid scene)
        mSpeed = mSpeed * transform.localScale.x;

        ogSpeed = mSpeed;
        PlayerMesh = transform.GetChild(0);
        Debug.Log("Playermesh is:"+PlayerMesh.name);
        animator = PlayerMesh.GetComponent<Animator>();

        maxHealth = health;

        respawnPos = transform.position;
        this.grid = FindObjectOfType<GenerateGrid>();

        inBuildMode = true;
    }

    void Update()
    {
        if(!isDead && !controlLock)
            Movement();
        // Force rotation = 0
        transform.rotation = Quaternion.Euler(Vector3.zero);


        // respawn test:
        if (isDead && Input.GetKeyDown(KeyCode.R))
            RespawnPlayer(false);

        if (!isDead && Input.GetKeyDown(KeyCode.K))
            DealDamage(25f);
        //set the current grid square
        UpdateGridSquare();



        
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

    public void StopWalkingAnim()
    {
        animator.SetFloat("Movement", 0);
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



    /// <summary>
    /// A function to deal damage to the player.
    /// </summary>
    /// <param name="dmg"></param>
    public void DealDamage(float dmg)
    {
        if (!invincible && !isDead)
        {
            health -= dmg;
            if (health <= 0)
            {
                health = 0;
                KillPlayer();
            }

            // play a random hurt sound (1 to 4)
            int r = Random.Range(1,4);
            SFXManager.instance.Play("PlayerHurt"+r);
        }
    }

    void KillPlayer()
    {
        controlLock = true;
        isDead = true;

        animator.SetBool("Dead", true);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);

        GameObject gb=GameObject.FindGameObjectWithTag("BuildMenu");
        if (gb != null)
            Destroy(gb.gameObject);

        gb=GameObject.FindGameObjectWithTag("ManageMenu");
        if (gb != null)
            Destroy(gb.gameObject);


        Instantiate(respawnUIPrefab);

        SFXManager.instance.Play("PlayerDeath");


    }


    public void RespawnPlayer(bool buyback)
    {
        controlLock = false;
        isDead = false;
        inBuildMode = true;
        building = false;
        managingTower = false;

        animator.SetBool("Dead", false);
        health = maxHealth;
        invincible = false;
        if (!buyback)
        {
            transform.position = respawnPos;
        }
        else
        {
            SFXManager.instance.Play("BuybackRespawn");
        }
        //Reset weapons and stuff.
        if (playerClass == "Warrior")
        {
            WarriorCombatBehavior scriptRef = GetComponent<WarriorCombatBehavior>();
            scriptRef.ResetAllCombat();
        }
        else
        {
            CombatBehavior scriptRef = GetComponent<CombatBehavior>();
            scriptRef.ResetAllCombat();
        }
        
    }

    // gold exchange functions
    
    public void AddGold(int amount)
    {
        gold += amount;

        // call update affordable on menus
        GameObject gb;
        gb = GameObject.FindGameObjectWithTag("BuildMenu");
        if (gb != null)
            gb.GetComponent<BuildMenu>().UpdateIfCanAfford(gold);

        gb = GameObject.FindGameObjectWithTag("ManageMenu");
        if (gb != null)
            gb.GetComponent<ManageMenu>().UpdateIfCanAfford(gold);

        gb = GameObject.FindGameObjectWithTag("RespawnMenu");
        if (gb != null)
            gb.GetComponent<RespawnButton>().UpdateIfCanAfford(gold);

    }

    public void RemoveGold(int amount)
    {
        gold -= amount;
        if (gold < 0)
        {
            gold = 0;
        }
    }
}
