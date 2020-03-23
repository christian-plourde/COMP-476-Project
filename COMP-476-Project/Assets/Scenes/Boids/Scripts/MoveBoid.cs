using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Boid rules:
- always be moving toward the perceived centre of mass
- always keep your velocity moving toward the perceived velocity centre of mass
- always keep your distance from your boid friends
     */

public class MoveBoid : MonoBehaviour
{
    //public float m_Padding;

    //private Transform m_Anchor;

    //private Vector3 m_Orientation;

    //private Vector3 m_Velocity;
    //public float MAX_VELOCITY = 4.0f;

    //private Vector3 m_Acceleration;
    //public float MAX_ACCELERATION = 5.0f;

    //private Vector2 m_AngularVelocity;
    //public float MAX_ANGULAR_VELOCITY = 4.0f;

    //private Vector2 m_AngularAcceleration;
    //public float MAX_ANGULAR_ACCELERATION = 5.0f;

    //private float DELTA_T = 0.025f;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    //Anchor should always be first child of the gameobject
    //    this.m_Anchor = this.transform.parent.GetChild(0);
    //    this.m_Orientation = (this.m_Anchor.position - this.transform.position).normalized;
    //}

    ///// <summary>
    ///// Return the normalized displacement towards the perceived centre of mass
    ///// </summary>
    ///// <param name="positions_of_everybody_else"></param>
    ///// <returns></returns>
    //Vector3 R1(List<Vector3> positions_of_everybody_else)
    //{
    //    Vector3 sum = new Vector3();
    //    foreach(Vector3 v in positions_of_everybody_else)
    //    {
    //        sum += v;
    //    }

    //    return (sum/positions_of_everybody_else.Count).normalized;
    //}

    ///// <summary>
    ///// Return the average velocity (which corresponds to direction of motion)
    ///// </summary>
    ///// <param name="velocities_of_everybody_else"></param>
    ///// <returns></returns>
    //Vector3 R2(List<Vector3> velocities_of_everybody_else)
    //{
    //    Vector3 sum = new Vector3();
    //    foreach (Vector3 v in velocities_of_everybody_else)
    //    {
    //        sum += v;
    //    }

    //    return (sum / velocities_of_everybody_else.Count).normalized;
    //}

    ///// <summary>
    ///// Keep your distance from everybody else
    ///// </summary>
    ///// <param name="positions_of_everybody_else"></param>
    ///// <returns></returns>
    //Vector3 R3(List<Vector3> positions_of_everybody_else)
    //{
    //    foreach(Vector3 p in positions_of_everybody_else)
    //    {
    //        if 
    //    }

    //    return new Vector3();
    //}

    ///// <summary>
    ///// Returns the normalized direction to the anchor
    ///// </summary>
    ///// <returns></returns>
    //Vector3 R4()
    //{
    //    Vector3 destination = this.m_Anchor.position;
    //    Vector3 origin = this.transform.position;
    //    return (destination - origin).normalized;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //Find the total movement input
    //    //Vector3 input = R1() + R2() + R3() + R4();

    //    //Calculate acceleration
    //    this.m_Acceleration = MAX_ACCELERATION * DELTA_T;

    //    //Calculate velocity
    //    this.m_Velocity += this.m_Acceleration * DELTA_T;
    //    if (this.m_Velocity > MAX_VELOCITY) { this.m_Velocity = MAX_VELOCITY; }

    //    //this.UpdateDirection();

    //    //Face where you're going
    //    this.transform.LookAt(this.m_Anchor.position);

    //    //Move
    //    this.transform.position += (this.m_Velocity * DELTA_T * this.m_Orientation.normalized);
    //}

    //float Dot(Vector3 lhs, Vector3 rhs)
    //{
    //    return (lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z);
    //}

    //void UpdateDirection()
    //{
    //    //find our new direction
    //    Vector3 forward = this.transform.forward;
    //    Vector3 to_target = (this.m_Anchor.position - this.transform.position).normalized;
    //    //An arbitrary radius
    //    float R = 1.0f;

    //    float dot = Dot(forward, to_target);
    //    float theta = Mathf.Acos(dot / (forward.magnitude * to_target.magnitude));

    //    float L = Mathf.PI * R * theta / 180.0f;

    //    ////Add to old acceleration
    //    //this.m_AngularAcceleration = 
    //    ////Add to old velocity
    //    //Vector3 cross = Vector3.Cross(to_target, forward);
    //    //this.transform.RotateAround(cross, this.m_AngularVelocity);
    //}

}
