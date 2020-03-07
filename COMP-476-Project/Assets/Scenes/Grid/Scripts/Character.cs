using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph;
using System.Linq;
using System;

public enum BEHAVIOUR_TYPE { BASE_SEEK, MOVE_TO_TOWER, MOVE_TO_PLAYER }


//a class used for an npc character
public class Character : NPC
{
    private BEHAVIOUR_TYPE m_PreviousBehaviour = BEHAVIOUR_TYPE.BASE_SEEK;

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
        Debug.Log("Character : Seeking base");
        try
        {
            //if we haven't set a target yet, or if the target has been closed,... 
            if (currentTarget == null || !currentTarget.Value.Open)
            {
                try
                {
                    //...then we need to turn around
                    Path = graph.ShortestPath(Path[current_path_node_index - 1], Path[path.Length - 1]).ToArray();
                    current_path_node_index = 0;

                    Movement.Target = Path[current_path_node_index].Value.transform.position;
                    currentTarget = Path[current_path_node_index];
                }

                //if our index is out of bounds, deal with it
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
            //If we've arrived...
            if (Movement.HasArrived)
            {
                //increment timer
                t += 0.1f;
                //if my timer is greater-than or equal to 1, I've arrived
                if (t >= 1)
                {
                    //update current node
                    current_node = Path[current_path_node_index];
                    //update next target
                    currentTarget = Path[++current_path_node_index];
                    //reset timer
                    t = 0;
                }

                //Interpolate to some point between our current position and next target
                Movement.Target = Vector3.Lerp(current_node.Value.transform.position, currentTarget.Value.transform.position, t);
            }

        }
        //if something goes wrong with the next node I want to visit, just return the base node closest to the player
        catch
        {
            Path = graph.ShortestPath(current_node, ClosestBaseNode.GraphNode).ToArray<GraphNode<LevelNode>>();
        }

        //Call NPC.Update, which ensures we keep moving until we arrive at our destination
        base.Update();
    }

    //Our update function when we want to be moving to the tower
    private void MoveToTowerUpdate()
    {
        Debug.Log("Character : Seeking tower");
        try
        {
            //if we haven't set a target yet, or if the target has been closed,... 
            if (currentTarget == null || !currentTarget.Value.Open)
            {
                try
                {
                    //...then we need to turn around
                    Path = graph.ShortestPath(Path[current_path_node_index - 1], Path[path.Length - 1]).ToArray();
                    current_path_node_index = 0;

                    Movement.Target = Path[current_path_node_index].Value.transform.position;
                    currentTarget = Path[current_path_node_index];
                }

                //if our index is out of bounds, deal with it
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
            //If we've arrived...
            if (Movement.HasArrived)
            {
                //increment timer
                t += 0.1f;
                //if my timer is greater-than or equal to 1, I've arrived
                if (t >= 1)
                {
                    //update current node
                    current_node = Path[current_path_node_index];
                    //update next target
                    currentTarget = Path[++current_path_node_index];
                    //reset timer
                    t = 0;
                }

                //Interpolate to some point between our current position and next target
                Movement.Target = Vector3.Lerp(current_node.Value.transform.position, currentTarget.Value.transform.position, t);
            }

        }
        //if something goes wrong with the next node I want to visit, just return the base node closest to the player
        catch
        {
            LevelNode ln = this.gameObject.GetComponent<ZombieMovement>().m_TargetTowerNode.gameObject.GetComponent<LevelNode>();
            Path = graph.ShortestPath(current_node, ln.GraphNode).ToArray<GraphNode<LevelNode>>();
        }

        //Call NPC.Update, which ensures we keep moving until we arrive at our destination
        base.Update();
    }

    //Our update function when we want to be moving to the tower
    private void MoveToPlayerUpdate()
    {
        Debug.Log("Character : Seeking player");
        try
        {
            //if we haven't set a target yet, or if the target has been closed,... 
            if (currentTarget == null || !currentTarget.Value.Open)
            {
                try
                {
                    //...then we need to turn around
                    Path = graph.ShortestPath(Path[current_path_node_index - 1], Path[path.Length - 1]).ToArray();
                    current_path_node_index = 0;

                    Movement.Target = Path[current_path_node_index].Value.transform.position;
                    currentTarget = Path[current_path_node_index];
                }

                //if our index is out of bounds, deal with it
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
            //If we've arrived...
            if (Movement.HasArrived)
            {
                //increment timer
                t += 0.1f;
                //if my timer is greater-than or equal to 1, I've arrived
                if (t >= 1)
                {
                    //update current node
                    current_node = Path[current_path_node_index];
                    //update next target
                    currentTarget = Path[++current_path_node_index];
                    //reset timer
                    t = 0;
                }

                //Interpolate to some point between our current position and next target
                Movement.Target = Vector3.Lerp(current_node.Value.transform.position, currentTarget.Value.transform.position, t);
            }

        }
        //if something goes wrong with the next node I want to visit, just return the base node closest to the player
        catch
        {
            LevelNode ln = this.gameObject.GetComponent<ZombieMovement>().m_TargetTowerNode.gameObject.GetComponent<LevelNode>();
            Path = graph.ShortestPath(current_node, ln.GraphNode).ToArray<GraphNode<LevelNode>>();
        }

        //Call NPC.Update, which ensures we keep moving until we arrive at our destination
        base.Update();
    }


    // Update is called once per frame
    protected override void Update()
    {
        if (grid.PlayerBaseNodes.Contains(current_node.Value))
            Destroy(this.gameObject);

        if(!Immobilized && m_PreviousBehaviour == behaviour_type)
        {
            
            switch (behaviour_type)
            {
                case BEHAVIOUR_TYPE.BASE_SEEK: BaseSeekUpdate(); break;
                //case BEHAVIOUR_TYPE.MOVE_TO_PLAYER: MoveToPlayerUpdate(); break;
                //case BEHAVIOUR_TYPE.MOVE_TO_TOWER: MoveToTowerUpdate(); break;
            }
        }
        else
        {
            m_PreviousBehaviour = behaviour_type;
        }
    }
}
