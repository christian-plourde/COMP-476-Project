using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph;
using System.Linq;
using System;

public enum BEHAVIOUR_TYPE { BASE_SEEK }

//a class used for an npc character
public class Character : NPC
{
    private LevelNode startNode; //the node that the chracter should start at
    private GraphNode<LevelNode> current_node; //the node that the chracter is currently at in the graph
    private Graph<LevelNode> graph; //a reference to the graph that is used for setting the movement path for the chracter
    private GraphNode<LevelNode>[] path = new GraphNode<LevelNode>[0]; //this is a list containing the nodes in the current chracters path
    private int current_path_node_index = 0; //the step of the path the character s currently executing
    private GraphNode<LevelNode> currentTarget;
    private GenerateGrid grid;
    private BEHAVIOUR_TYPE behaviour_type;
    private float t = 0.0f; //the parameter for following the spline 

    public BEHAVIOUR_TYPE BehaviourType
    {
        set { behaviour_type = value; }
    }

    //gets the closest base node to the current npc (the closest goal node)
    public LevelNode ClosestBaseNode
    {
        get {

            LevelNode winner = grid.PlayerBaseNodes[0];
            float smallest_dist = float.MaxValue;

            foreach(LevelNode n in grid.PlayerBaseNodes)
            {
                if((current_node.Value.transform.position - n.transform.position).magnitude < smallest_dist)
                {
                    smallest_dist = (current_node.Value.transform.position - n.transform.position).magnitude;
                    winner = n;
                }
            }

            return winner;

        }
    }

    public GraphNode<LevelNode>[] Path
    {
        get { return path; }
        set { path = value;
            t = 0.0f;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        grid = FindObjectOfType<GenerateGrid>();

        startNode = grid.EnemyBaseNodes[UnityEngine.Random.Range(0, grid.EnemyBaseNodes.Count)];

        //initialize the node that the character is at to the graph node of the level node that he was placed at to begin
        current_node = startNode.GetComponent<LevelNode>().GraphNode;

        //set the position of the character to the position of the current node
        if(!Immobilized)
            transform.position = current_node.Value.transform.position;

        graph = FindObjectOfType<GenerateGrid>().Graph;
        Movement.Target = current_node.Value.transform.position;
    }

    public override void ObserverUpdate()
    {
        try
        {
            Path = graph.ShortestPath(Path[current_path_node_index--], Path[path.Length - 1]).ToArray();

            if (!Path.Contains(currentTarget))
            {
                Movement.Target = current_node.Value.transform.position;
                currentTarget = current_node;
            }
        }

        catch
        {
            Movement.Target = Path[current_path_node_index].Value.transform.position;
            currentTarget = Path[current_path_node_index];
        }
    }

    private void BaseSeekUpdate()
    {
        try
        {
            if (!currentTarget.Value.Open)
            {
                try
                {
                    Path = graph.ShortestPath(Path[current_path_node_index - 1], Path[path.Length - 1]).ToArray();
                    current_path_node_index = 0;

                    Movement.Target = Path[current_path_node_index].Value.transform.position;
                    currentTarget = Path[current_path_node_index];
                }

                catch (Exception)
                {
                    current_path_node_index = Path.Length;
                    Movement.Target = current_node.Value.transform.position;
                    currentTarget = current_node;
                }
            }
        }

        catch
        {

        }

        try
        {
            if (Movement.HasArrived)
            {
                current_node = Path[current_path_node_index];
                //Movement.Target = Path[++current_path_node_index].Value.transform.position;
                currentTarget = Path[++current_path_node_index];
            }

            t += Time.deltaTime * MaxVelocity / 10;
            Movement.Target = Vector3.Slerp(current_node.Value.transform.position, Path[current_path_node_index + 1].Value.transform.position, t);
        }

        catch
        {
            Path = graph.ShortestPath(current_node, ClosestBaseNode.GraphNode).ToArray<GraphNode<LevelNode>>();
        }


        base.Update();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (grid.PlayerBaseNodes.Contains(current_node.Value) || (currentTarget != null && grid.PlayerBaseNodes.Contains(currentTarget.Value)))
            Destroy(this.gameObject);

        if(!Immobilized)
        {
            switch (behaviour_type)
            {
                case BEHAVIOUR_TYPE.BASE_SEEK: BaseSeekUpdate(); break;
            }
        }
    }
}
