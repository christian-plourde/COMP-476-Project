using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Boid rules:
- always be movig toward the perceived centre of mass
- always keep your velocity moving toward the perceived velocity centre of mass
- always keep your distance from your boid friends
     */

public class MoveBoid : MonoBehaviour
{
    private Transform m_Anchor;

    private Vector3 m_Direction;

    private float m_Velocity;
    public float MAX_VELOCITY = 4.0f;

    private float m_Acceleration;
    public float MAX_ACCELERATION = 5.0f;

    private float DELTA_T = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        //Anchor should always be first child of the gameobject
        this.m_Anchor = this.transform.parent.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate acceleration
        this.m_Acceleration = MAX_ACCELERATION * DELTA_T;

        //Calculate velocity
        this.m_Velocity += this.m_Acceleration * DELTA_T;
        if (this.m_Velocity > MAX_VELOCITY) { this.m_Velocity = MAX_VELOCITY; }

        this.m_Direction = (this.m_Anchor.position - this.transform.position).normalized;

        //Face where you're going
        this.transform.LookAt(this.m_Anchor.position);

        //Move
        this.transform.position += (this.m_Velocity * DELTA_T * this.m_Direction.normalized);
    }

}
