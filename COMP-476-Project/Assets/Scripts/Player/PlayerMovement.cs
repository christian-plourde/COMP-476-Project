using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* 3rd Person
     * 8 Directional Movement with WASD 
     * Will Align with movement direction before running (Align Behavior in this class)
     */

    [Header("Movement Parameters")]
    public float mSpeed=5f;
    public float sprintMultiplier=1f;

    private Animator animator;

    //private Transform Target;

    private Transform PlayerMesh;

    [Header("Player States")]
    bool isRunning;
    bool isBuilding;
    bool controlLock;
    bool isDead;

    // to hard code orientation
    public float PlayerRotAngle = 0;


    void Start()
    {
        PlayerMesh = transform.GetChild(0);
        Debug.Log("Playermesh is:"+PlayerMesh.name);
        animator = PlayerMesh.GetComponent<Animator>();
        //Target = transform.GetChild(2);
    }

    void Update()
    {
        if(!isDead && !controlLock)
            Movement();
        // Force rotation = 0
        transform.rotation = Quaternion.Euler(Vector3.zero);
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

            PlayerRotAngle = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-1 * transform.forward.normalized * sprintMultiplier * mSpeed * Time.deltaTime);
            FacingDirection+= (transform.forward * -1);
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);

            PlayerRotAngle = 180;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right.normalized * sprintMultiplier * mSpeed * Time.deltaTime);
            FacingDirection += transform.right;
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);

            PlayerRotAngle = 90;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-1 * transform.right.normalized * sprintMultiplier * mSpeed * Time.deltaTime);
            FacingDirection += (transform.right * -1);
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);

            PlayerRotAngle = -90;
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
        PlayerMesh.localRotation = Quaternion.RotateTowards(PlayerMesh.localRotation, lookDirection, 4);


        // PlayerMesh.rotation = Quaternion.Euler(new Vector3(0,PlayerRotAngle,0)) ;
    }
}
