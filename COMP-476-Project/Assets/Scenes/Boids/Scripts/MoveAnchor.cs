using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnchor : MonoBehaviour
{
    private Vector3 m_StartingPosition;

    float theta = 0.0f;
    float DELTA_T = 0.025f;

    public float m_Radius = 4.0f;

    private void Start()
    {
        this.m_StartingPosition = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        float x = this.m_Radius * Mathf.Cos(theta);
        float z = this.m_Radius * Mathf.Sin(theta);
        this.transform.position = m_StartingPosition + new Vector3(x, 0.0f, z);
        theta += DELTA_T * 0.5f;
    }
}
