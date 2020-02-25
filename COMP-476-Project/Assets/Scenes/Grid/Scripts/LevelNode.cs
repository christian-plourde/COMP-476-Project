using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Graph;

public class LevelNode : MonoBehaviour, IHeuristic<LevelNode>
{
    GraphNode<LevelNode> node; //this is node in the actual graph that will be used for pathfinding
    private Graph<LevelNode> graph;
    private GridSquare grid_square;
    public GameObject lineRendererPrefab; //this is a prefab game object that contains a line renderer. This is important because we will need to instantiate a separate line renderer for each game object
    public Material default_node_mat;
    public Material closed_node_mat;
    List<LineRenderer> lineRenderers; //this is the list of line renderers. we will instantiate one for each connection that the node has to draw the paths in the scene
    private bool closed = false;

    public bool Open
    {
        get { return this.closed; }
    }

    public void AddLineRenderer()
    {
        GameObject o = Instantiate(lineRendererPrefab);
        lineRenderers.Add(o.GetComponent<LineRenderer>());
    }

    public Graph<LevelNode> Graph
    {
        set { graph = value; }
    }

    public GridSquare GridSquare
    {
        get { return grid_square; }
        set { grid_square = value; }
    }

    public GraphNode<LevelNode> GraphNode
    {
        get { return node; }
    }

    public double ComputeHeuristic(LevelNode goal)
    {
        if (closed)
            return double.MaxValue;

        return (goal.GraphNode.Value.transform.position - node.Value.transform.position).magnitude;
    }

    private void Awake()
    {
        lineRenderers = new List<LineRenderer>();
        //let's set the graph node that its connected to 
        node = new GraphNode<LevelNode>(this);
    }

    //will switch the node to opened if closed and vice versa. this will set all its links to disconnected
    public void ToggleOpen()
    {
        this.closed = !this.closed;
        foreach (GraphEdge<LevelNode> e in this.GraphNode.Links)
        {
            e.Disconnected = !e.Disconnected;
        }

        if (closed)
            this.GetComponent<MeshRenderer>().material = closed_node_mat;
        else
            this.GetComponent<MeshRenderer>().material = default_node_mat;

        
    }

    void Update()
    {
        int i = 0;
        foreach (GraphEdge<LevelNode> o in node.Links)
        {
            lineRenderers[i].SetPosition(0, this.transform.position); //draw a line from the start to the end of the connection
            lineRenderers[i].SetPosition(1, o.End.Value.transform.position);
            i++;
        }
    }

}
