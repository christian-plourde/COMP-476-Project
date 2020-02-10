using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This script assumes a square plane.*/

public class NewBehaviourScript : MonoBehaviour
{
    public Transform m_TopRight;
    public Transform m_TopLeft;

    public int m_SquaresPerFace;

    private float PLANE_SIDELENGTH;
    private float GRIDSQUARE_SIDELENGTH;

    private List<GridSquare> m_GridSquares = new List<GridSquare>();

    class GridSquare
    {
        public Vector3 m_Centre;

        public GridSquare(Vector3 centre)
        {
            this.m_Centre = centre;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        this.getPlaneDimensions();

        this.initializeSquares();
    }

    //A function to render the squares in the editor(this function doesn't run when you run the game)
    void OnDrawGizmos()
    {
        this.getPlaneDimensions();
        this.initializeSquares();

        foreach (GridSquare gs in this.m_GridSquares)
        {
            // Draw a semitransparent blue cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawCube(gs.m_Centre, new Vector3(this.GRIDSQUARE_SIDELENGTH - 0.05f, 0.5f, this.GRIDSQUARE_SIDELENGTH - 0.05f));
        }
    }

    void initializeSquares()
    {
        this.GRIDSQUARE_SIDELENGTH = this.PLANE_SIDELENGTH / (float)m_SquaresPerFace;
        Vector3 start = this.m_TopLeft.position;
        for (int i = 0; i < m_SquaresPerFace; i++)
        {
            for (int j = 0; j < m_SquaresPerFace; j++)
            {
                float x = start.x + (GRIDSQUARE_SIDELENGTH / 2.0f) + (GRIDSQUARE_SIDELENGTH * i);
                float z = start.z - (GRIDSQUARE_SIDELENGTH / 2.0f) - (GRIDSQUARE_SIDELENGTH * j);
                this.m_GridSquares.Add(new GridSquare(new Vector3(x, this.transform.position.y, z)));
            }
        }
    }

    //A function to find out the dimensions of our floor plane. 
    void getPlaneDimensions()
    {
        this.PLANE_SIDELENGTH = (this.m_TopRight.position - this.m_TopLeft.position).magnitude;
    }
}
