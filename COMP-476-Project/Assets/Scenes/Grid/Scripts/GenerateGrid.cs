using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Graph;

/*This script assumes a square plane.*/

public class GridCoordinate
{
    private int row;
    private int col;

    public int Row
    {
        get { return row; }
    }

    public int Column
    {
        get { return col; }
    }

    public GridCoordinate(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public override string ToString()
    {
        return "[" + row + ", " + col + "]";
    }
}

public class GridSquare : IHeuristic<GridSquare>
{
    private Vector3 m_Centre;
    private GridCoordinate coordinate;

    public Vector3 Position
    {
        get { return m_Centre; }
        set { m_Centre = value; }
    }

    public GridCoordinate Coordinate
    {
        get { return coordinate; }
        set { coordinate = value; }
    }

    public GridSquare(Vector3 centre)
    {
        this.m_Centre = centre;
    }

    public GridSquare(Vector3 centre, GridCoordinate coord)
    {
        this.m_Centre = centre;
        this.coordinate = coord;
    }

    public bool IsNeighbor(GridSquare s)
    {
        if (s.coordinate.Row == this.coordinate.Row && ((this.coordinate.Column - 1) == s.coordinate.Column || (this.coordinate.Column + 1) == s.coordinate.Column))
            return true;

        else if (s.coordinate.Column == this.coordinate.Column && ((this.coordinate.Row - 1) == s.coordinate.Row || (this.coordinate.Row + 1) == s.coordinate.Row))
            return true;

        /*
        else if (((s.coordinate.Column == (this.coordinate.Column - 1)) && s.coordinate.Row == (this.coordinate.Row - 1)) ||
                ((s.coordinate.Column == (this.coordinate.Column + 1)) && s.coordinate.Row == (this.coordinate.Row - 1)) ||
                ((s.coordinate.Column == (this.coordinate.Column - 1)) && s.coordinate.Row == (this.coordinate.Row + 1)) ||
                ((s.coordinate.Column == (this.coordinate.Column + 1)) && s.coordinate.Row == (this.coordinate.Row + 1))
                )
            return true;
            */

        else
            return false;
    }

    public double ComputeHeuristic(GridSquare goal)
    {
        return (this.Position - goal.Position).magnitude;
    }

    public override string ToString()
    {
        return coordinate.ToString() + " " + Position.ToString();
    }

}

public class GenerateGrid : Subject
{
    public Transform m_TopRight;
    public Transform m_TopLeft;

    public int m_SquaresPerFace;

    private float PLANE_SIDELENGTH;
    private float GRIDSQUARE_SIDELENGTH;

    private List<GridSquare> m_GridSquares = new List<GridSquare>();
    PathFinderGraph<LevelNode> graph; //the graph used for pathfinding

    public GameObject level_node_prefab;
    public Camera cam;
    

    public Graph<LevelNode> Graph
    {
        get { return graph; }
    }

    void Awake()
    {
        this.getPlaneDimensions();
        graph = new PathFinderGraph<LevelNode>();

        this.initializeSquares();

        //add each of the characters to the observer list
        foreach(Character c in FindObjectsOfType<Character>())
        {
            AttachObserver(c);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<LevelNode>())
                {

                    hit.transform.gameObject.GetComponent<LevelNode>().ToggleOpen();
                    Notify();
                }
            }

        }
    }

    private List<GraphNode<LevelNode>> getNeighbors(GridSquare square)
    {
        List<GraphNode<LevelNode>> neighbors = new List<GraphNode<LevelNode>>();
        foreach(GraphNode<LevelNode> n in graph.Nodes)
        {
            if (n.Value.GridSquare.IsNeighbor(square) && !neighbors.Contains(n))
                neighbors.Add(n);
        }

        return neighbors;
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
                this.m_GridSquares.Add(new GridSquare(new Vector3(x, this.transform.position.y, z), new GridCoordinate(i, j)));
            }
        }

        //when the squares are initialized they also need to be added to the graph for pathfinding
        foreach(GridSquare s in m_GridSquares)
        {
            GameObject node = Instantiate(level_node_prefab, s.Position, Quaternion.identity);

            LevelNode l = node.GetComponent<LevelNode>();
            l.GridSquare = s;

            graph.Add(l.GraphNode);
        }

        //now we need to connect neighbors
        foreach(GraphNode<LevelNode> n in graph.Nodes)
        {
            foreach(GraphNode<LevelNode> g in getNeighbors(n.Value.GridSquare))
            {
                n.AddNeighbor(g, n.Value.ComputeHeuristic(g.Value));
                n.Value.AddLineRenderer();
            }
        }

        
    }

    //A function to find out the dimensions of our floor plane. 
    void getPlaneDimensions()
    {
        this.PLANE_SIDELENGTH = (this.m_TopRight.position - this.m_TopLeft.position).magnitude;
    }
}
