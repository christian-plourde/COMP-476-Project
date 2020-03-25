using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    /// <summary>
    /// An empty gameobject containing the boid anchor
    /// </summary>
    [SerializeField] GameObject m_BoidContainer;
    /// <summary>
    /// The boid prefab to clone
    /// </summary>
    [SerializeField] GameObject m_BoidObject;
    /// <summary>
    /// The number of boids we instantiate.
    /// </summary>
    public int m_NumBoids;

    Transform m_Anchor;

    List<Boid> m_Boids = new List<Boid>();
    /// <summary>
    /// Our time constant
    /// </summary>
    public static float DELTA_T = 0.025f;
    /// <summary>
    /// The padding the boids are to keep between each other
    /// </summary>
    public static float PADDING = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        float spawn_radius = 2.0f;
        float conversion = Mathf.PI / 180.0f;
        this.m_Anchor = this.m_BoidContainer.transform.GetChild(0);
        for(int i = 0; i < m_NumBoids; i++)
        {
            float angle = i * (360.0f / m_NumBoids) * conversion;
            Vector3 spawn_pos = this.m_Anchor.position + new Vector3(spawn_radius * Mathf.Cos(angle), spawn_radius * Mathf.Sin(angle));
            this.m_Boids.Add(GameObject.Instantiate(this.m_BoidObject, spawn_pos, new Quaternion(), this.m_BoidContainer.transform).GetComponent<Boid>());
            this.m_Boids[i].Initialize(this.m_Anchor);
        }

        //Initialize the FlyingEnemyBehaviour Decision Tree
        this.m_BoidContainer.GetComponent<FlyingEnemyBehaviour>().Initialize();
    }

    void UpdateBoids()
    {
        for(int i = 0; i < this.m_Boids.Count; i++)
        {
            //Get the position and velocity of all but the currently executing boid
            List<Vector3> other_positions = new List<Vector3>();
            List<Vector3> other_velocities = new List<Vector3>();

            for (int j = 0; j < this.m_Boids.Count; j++)
            {
                if (i != j)
                {
                    other_positions.Add(this.m_Boids[j].Position);
                    other_velocities.Add(this.m_Boids[i].Velocity);
                }
            }
            //We have everybody else's position and velocity
            Boid b = this.m_Boids[i];
            b.ComputeR1(other_positions);
            b.ComputeR2(other_velocities);
            b.ComputeR3(other_positions);
            b.ComputeR4();
            b.ComputeInputVector();
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateBoids();
    }
}
