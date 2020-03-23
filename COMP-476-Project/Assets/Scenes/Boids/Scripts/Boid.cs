using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Boid rules:
R1: always be movig toward the perceived centre of mass
- always keep your velocity moving toward the perceived velocity centre of mass
- always keep your distance from your boid friends
     */

public class Boid : MonoBehaviour
{
    private Transform m_Anchor;

    public Vector3 Position { get { return this.transform.position; } }

    /// <summary>
    /// The current velocity
    /// </summary>
    private Vector3 m_Velocity;
    public float MAX_VELOCITY = 5.0f;
    public Vector3 Velocity { get { return this.m_Velocity; } }

    /// <summary>
    /// The current acceleration
    /// </summary>
    private Vector3 m_Acceleration;
    public float MAX_ACCELERATION = 5.0f;
    public Vector3 Acceleration { get { return this.m_Acceleration; } }

    private float m_AngularVelocity = 0.0f;
    public float MAX_ANGULAR_VELOCITY = 5.0f;

    private float m_AngularAcceleration;
    public float MAX_ANGULAR_ACCELERATION = 5.0f;

    //private float BoidManager.DELTA_T = 0.025f;

    //private float m_Padding;

    private Vector3 r1;
    private Vector3 r2;
    private Vector3 r3;
    private Vector3 r4;

    private Vector3 m_InputVector = new Vector3();

    /// <summary>
    /// Return the normalized displacement towards the perceived centre of mass
    /// </summary>
    /// <param name="positions_of_everybody_else"></param>
    /// <returns></returns>
    public void ComputeR1(List<Vector3> positions_of_everybody_else)
    {
        Vector3 sum = new Vector3();
        foreach (Vector3 v in positions_of_everybody_else)
        {
            sum += v;
        }

        this.r1 = (sum / positions_of_everybody_else.Count).normalized;
    }

    /// <summary>
    /// Return the average velocity (which corresponds to direction of motion)
    /// </summary>
    /// <param name="velocities_of_everybody_else"></param>
    /// <returns></returns>
    public void ComputeR2(List<Vector3> velocities_of_everybody_else)
    {
        Vector3 sum = new Vector3();
        foreach (Vector3 v in velocities_of_everybody_else)
        {
            sum += v;
        }

        this.r2 = (sum / velocities_of_everybody_else.Count).normalized;
    }

    /// <summary>
    /// Keep your distance from everybody else
    /// </summary>
    /// <param name="positions_of_everybody_else"></param>
    /// <returns></returns>
    public void ComputeR3(List<Vector3> positions_of_everybody_else)
    {
        Vector3 sum = new Vector3();
        int count = 0;
        foreach (Vector3 p in positions_of_everybody_else)
        {
            if ((this.transform.position - p).magnitude < BoidManager.PADDING)
            {
                count++;
                Vector3 away_from_target = this.transform.position - p;
                sum += (away_from_target);
            }
        }

        this.r3 = (sum / count).normalized;
    }

    /// <summary>
    /// Returns the normalized direction to the anchor
    /// </summary>
    /// <returns></returns>
    public void ComputeR4()
    {
        Vector3 destination = this.m_Anchor.position;
        Vector3 origin = this.transform.position;
        this.r4 = (destination - origin).normalized;
    }

    public void Initialize(Transform anchor)
    {
        this.m_Anchor = anchor;
    }

    public void ComputeInputVector()
    {
        this.m_InputVector = ((2 * r1) + r2 + r3 + (3 * r4)).normalized;
    }

    private void Update()
    {
        if (this.m_Anchor != null)
        {
            //Calculate acceleration
            this.m_Acceleration = MAX_ACCELERATION * this.m_InputVector.normalized;

            //Calculate velocity
            this.m_Velocity += this.m_Acceleration * BoidManager.DELTA_T;
            if (this.m_Velocity.magnitude > MAX_VELOCITY)
            {
                this.m_Velocity = this.m_Velocity.normalized * MAX_VELOCITY;
            }

            this.transform.LookAt(this.transform.position + this.m_Velocity);

            //Vector3 to_forward = this.m_Velocity.normalized;
            //Vector3 to_target = (this.m_Anchor.position - this.transform.position).normalized;

            ////|A x B| = |A||B|sin theta
            ////arcsin((|A x B| / |A||B|) = theta, where both |A| = 1, |B| = 1 => arcsin(|A x B|) = theta
            //Vector3 cross = Vector3.Cross(to_forward, to_target);
            //float theta = Mathf.Asin(cross.magnitude);

            ////Angular acceleration
            ////this.m_AngularAcceleration = this.MAX_ANGULAR_ACCELERATION * (theta / (30 * Mathf.PI / 180.0f));
            //this.m_AngularAcceleration = this.MAX_ANGULAR_ACCELERATION;
            //float acc = this.m_AngularAcceleration * BoidManager.DELTA_T;
            //this.m_AngularVelocity += (theta > (Mathf.PI / 12.0f)) ? acc : 0.0f;
            //if (this.m_AngularVelocity > this.MAX_ANGULAR_VELOCITY)
            //{
            //    this.m_AngularVelocity = MAX_ANGULAR_VELOCITY;
            //}
            ////Debug.Log("Angle: " + theta);

            //this.transform.Rotate(cross, this.m_AngularVelocity * BoidManager.DELTA_T);


            this.transform.position += this.m_Velocity * BoidManager.DELTA_T;
        }
        
    }
}
