using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovementTest : MonoBehaviour
{
    public Transform Target;
    public float mSpeed;

    private Animator animator;
    bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Chasing",true);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead && Vector3.Distance(Target.position,transform.position)>3)
            Move();
    }

    void Move()
    {
        Vector3 Dir = (Target.position - transform.position);
        //Align(Dir);

        //transform.Translate(Dir.normalized*mSpeed*Time.deltaTime);
        transform.Translate(transform.forward*-mSpeed*Time.deltaTime);
    }

    void Align(Vector3 FaceDir)
    {
        Quaternion lookTowards;
        lookTowards = Quaternion.LookRotation(transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookTowards, 40f);
    }

    public void SetDead(bool value)
    {
        isDead = value;
    }
}
