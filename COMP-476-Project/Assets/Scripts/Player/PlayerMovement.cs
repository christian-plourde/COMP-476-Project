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

    private Animator animator;

    //private Transform Target;

    private Transform PlayerMesh;


    void Start()
    {
        PlayerMesh = transform.GetChild(0);
        animator = PlayerMesh.GetComponent<Animator>();
        //Target = transform.GetChild(2);
    }

    void Update()
    {
        Movement();
        //PlayerMesh.transform.Rotate(Vector3.up * 10f);
    }

    void Movement()
    {
        Vector3 FacingDirection=Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * mSpeed * Time.deltaTime);
            FacingDirection += transform.forward;
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-1 * transform.forward * mSpeed * Time.deltaTime);
            FacingDirection+= (transform.forward * -1);
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * mSpeed * Time.deltaTime);
            FacingDirection += transform.right;
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-1 * transform.right * mSpeed * Time.deltaTime);
            FacingDirection += (transform.right * -1);
            //delegate align towards facing direction.
            AlignOrientation(FacingDirection);
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
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, align_Rotation_Speed);
        //transform.rotation = Quaternion.RotateTowards(PlayerMesh.rotation, lookDirection, 4);
        //transform.rotation = Quaternion.RotateTowards(PlayerMesh.rotation, lookDirection, 4);
        PlayerMesh.localRotation= Quaternion.RotateTowards(PlayerMesh.rotation, lookDirection, 4);
    }
}
