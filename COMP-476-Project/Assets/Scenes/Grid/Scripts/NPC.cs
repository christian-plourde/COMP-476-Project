using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    float MAX_VELOCITY = 0.6f;
    float currentVelocity = 0.0f;
    float MAX_ANGULAR_VELOCITY = 40.0f;
    float currentAngularVelocity = 0.0f;
    float MAX_ANGULAR_ACCELERATION = 50.0f;
    AlignedMovement movement;
    GenerateGrid grid_generator;
    GridSquare grid_square;

    public GridSquare GridSquare
    {
        get { return grid_square; }
        set { grid_square = value;
            Position = grid_square.m_Centre;
        }
    }

    public float MaxVelocity
    {
        get { return MAX_VELOCITY; }
        set { MAX_VELOCITY = value; }
    }

    public float MaxAngularVelocity
    {
        get { return MAX_ANGULAR_VELOCITY; }
    }

    public float MaxAngularAcceleration
    {
        get { return MAX_ANGULAR_ACCELERATION; }
    }

    public float Velocity
    {
        get { return currentVelocity; }
        set { currentVelocity = value; }
    }

    public float AngularVelocity
    {
        get { return currentAngularVelocity; }
        set { currentAngularVelocity = value; }
    }

    public Vector3 Position
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }

    public AlignedMovement Movement
    {
        get { return movement; }
        set { movement = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Movement = new KinematicArrive(this);
        grid_generator = FindObjectOfType<GenerateGrid>();
        GridSquare = grid_generator[5, 5];
        Movement.Target = grid_generator[0, 0].m_Centre;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Movement.HasArrived)
            Movement.Move();

    }
}
