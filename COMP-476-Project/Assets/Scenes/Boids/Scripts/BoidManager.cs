using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    /// <summary>
    /// An empty gameobject containing the boid anchor
    /// </summary>
    [SerializeField] GameObject m_BoidContainer;
    [SerializeField] GameObject m_BoidObject;
    
    public int m_NumBoids;

    Transform m_Anchor;

    List<Boid> m_Boids = new List<Boid>();

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
            this.m_Boids[i].SetAnchor(this.m_Anchor);
        }

    }

    void MoveBoids()
    {
        foreach (Boid b in this.m_Boids)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
