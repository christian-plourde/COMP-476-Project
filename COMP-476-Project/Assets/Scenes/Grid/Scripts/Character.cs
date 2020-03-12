﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph;
using System.Linq;
using System;

public enum BEHAVIOUR_TYPE { BASE_SEEK, MOVE_TO_TOWER, MOVE_TO_PLAYER }


//a class used for an npc character
public class Character : NPC
{

    /// <summary>
    /// The node the character starts at.
    /// </summary>
    private LevelNode startNode;
    /// <summary>
    /// The node that the chracter is currently at in the graph
    /// </summary>
    private GraphNode<LevelNode> current_node;
    /// <summary>
    /// A reference to the graph used for setting the character's movement path
    /// </summary>
    private Graph<LevelNode> graph;
    /// <summary>
    /// A list containing the nodes in the character's path.
    /// </summary>
    private GraphNode<LevelNode>[] path = new GraphNode<LevelNode>[0];
    /// <summary>
    /// The index of the current node in the character path.
    /// </summary>
    private int current_path_node_index = 0;
    private GraphNode<LevelNode> currentTarget;
    /// <summary>
    /// A reference to the scene grid
    /// </summary>
    private GenerateGrid grid;
    private BEHAVIOUR_TYPE behaviour_type;
    /// <summary>
    /// A float to manage interpolation with our spline movement.
    /// </summary>
    private float t = 0.0f;

    private PlayerMovement player;

    public BEHAVIOUR_TYPE BehaviourType
    {
        set { behaviour_type = value; }
    }

    /// <summary>
    /// Returns the closest node considered to be a member of the player's base (relative to this calling Character).
    /// </summary>
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
            current_path_node_index = 0;
        }
    }

    
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

        //set reference to the player
        player = FindObjectOfType<PlayerMovement>();
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
            try
            {
                Movement.Target = Path[current_path_node_index].Value.transform.position;
                currentTarget = Path[current_path_node_index];
            }

            catch
            {

            }
            
        }
    }

    /// <summary>
    /// An update function to manage our pseudo-wander, Base Seek. Manages movement such that the enemy seek the player's base.
    /// </summary>
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
        //as long as we are not at player keep moving
        if(CurrentNode.Value.GridSquare != player.GridSquare)
            base.Update();
    }

    //Our update function when we want to be moving to the tower
    private void MoveToTowerUpdate()
    {
        //Debug.Log("Character : Seeking tower");
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
        //as long as we are not at tower keep moving
        if (this.gameObject.GetComponent<ZombieMovement>().m_TargetTowerNode.gameObject.GetComponent<LevelNode>().GridSquare != CurrentNode.Value.GridSquare)
            base.Update();
    }

    /// <summary>
    /// Our update function when we want to be moving to the player's position
    /// </summary>
    private void MoveToPlayerUpdate()
    {
        //Debug.Log("Character : Seeking player");
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
            Path = graph.ShortestPath(current_node, player.GridSquare.Node).ToArray<GraphNode<LevelNode>>();
        }

        //Call NPC.Update, which ensures we keep moving until we arrive at our destination
        //as long as not at player keep moving
        if (CurrentNode.Value.GridSquare != player.GridSquare)
            base.Update();
    }//end f'n

    public GraphNode<LevelNode> CurrentNode
    {
        get { return current_node; }
    }


    // Update is called once per frame
    protected override void Update()
    {
        if (grid.PlayerBaseNodes.Contains(current_node.Value))
            Destroy(this.gameObject);

        if(!Immobilized)
        {
            
            switch (behaviour_type)
            {
                case BEHAVIOUR_TYPE.BASE_SEEK: BaseSeekUpdate(); break;
                case BEHAVIOUR_TYPE.MOVE_TO_PLAYER: MoveToPlayerUpdate(); break;
                case BEHAVIOUR_TYPE.MOVE_TO_TOWER: MoveToTowerUpdate(); break;
            }
        }
    }
}
