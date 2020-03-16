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

    /// <summary>
    /// The current velocity
    /// </summary>
    private Vector3 m_Velocity;

    public float MAX_VELOCITY = 5.0f;
    /// <summary>
    /// The current acceleration
    /// </summary>
    private Vector3 m_Acceleration;

    public float MAX_ACCELERATION = 5.0f;

    private float m_AngularVelocity = 0.0f;
    public float MAX_ANGULAR_VELOCITY = 5.0f;

    private float m_AngularAcceleration;
    public float MAX_ANGULAR_ACCELERATION = 5.0f;

    private float DELTA_T = 0.025f;

    public void SetAnchor(Transform anchor)
    {
        this.m_Anchor = anchor;
    }

    private void Update()
    {
        if (this.m_Anchor != null)
        {
            
            //Calculate acceleration
            this.m_Acceleration = MAX_ACCELERATION * (this.m_Anchor.position - this.transform.position).normalized;

            //Calculate velocity
            this.m_Velocity += this.m_Acceleration * DELTA_T;
            if (this.m_Velocity.magnitude > MAX_VELOCITY)
            {
                this.m_Velocity = this.m_Velocity.normalized * MAX_VELOCITY;
            }

            Vector3 to_forward = this.m_Velocity.normalized;
            Vector3 to_target = (this.m_Anchor.position - this.transform.position).normalized;

            //|A x B| = |A||B|sin theta
            //arcsin((|A x B| / |A||B|) = theta, where both |A| = 1, |B| = 1 => arcsin(|A x B|) = theta
            Vector3 cross = Vector3.Cross(to_forward, to_target);
            float theta = Mathf.Asin(cross.magnitude);

            //Angular acceleration
            //this.m_AngularAcceleration = this.MAX_ANGULAR_ACCELERATION * (theta / (30 * Mathf.PI / 180.0f));
            this.m_AngularAcceleration = this.MAX_ANGULAR_ACCELERATION;
            float acc = this.m_AngularAcceleration * DELTA_T;
            this.m_AngularVelocity += (theta > (Mathf.PI / 12.0f)) ? acc : 0.0f;
            if (this.m_AngularVelocity > this.MAX_ANGULAR_VELOCITY)
            {
                this.m_AngularVelocity = MAX_ANGULAR_VELOCITY;
            }
            //Debug.Log("Angle: " + theta);

            this.transform.Rotate(cross, this.m_AngularVelocity * DELTA_T);


            this.transform.position += this.m_Velocity * DELTA_T;
        }
        
    }
}
