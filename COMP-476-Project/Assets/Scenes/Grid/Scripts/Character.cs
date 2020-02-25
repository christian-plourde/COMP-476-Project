﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph;
using System.Linq;
using System;

//a class used for an npc character
public class Character : NPC
{
    private LevelNode startNode; //the node that the chracter should start at
    private GraphNode<LevelNode> current_node; //the node that the chracter is currently at in the graph
    private Graph<LevelNode> graph; //a reference to the graph that is used for setting the movement path for the chracter
    public Camera cam;
    private GraphNode<LevelNode>[] path = new GraphNode<LevelNode>[0]; //this is a list containing the nodes in the current chracters path
    private int current_path_node_index = 0; //the step of the path the character s currently executing
    private GraphNode<LevelNode> currentTarget;
    bool end_of_path = false;
    public bool is_enemy = false;

    public GraphNode<LevelNode>[] Path
    {
        get { return path; }
        set { path = value;
            end_of_path = false;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        startNode = Resources.FindObjectsOfTypeAll<LevelNode>()[0];

        //initialize the node that the character is at to the graph node of the level node that he was placed at to begin
        current_node = startNode.GetComponent<LevelNode>().GraphNode;

        //set the position of the character to the position of the current node
        transform.position = current_node.Value.transform.position;

        graph = FindObjectOfType<GenerateGrid>().Graph;
        Movement.Target = current_node.Value.transform.position;

        MaxVelocity = 10 * MaxVelocity;
    }

    public override void ObserverUpdate()
    {
        try
        {
            path = graph.ShortestPath(path[current_path_node_index--], path[path.Length - 1]).ToArray();

            if (!path.Contains(currentTarget))
            {
                Movement.Target = current_node.Value.transform.position;
                currentTarget = current_node;
            }
        }

        catch
        {
            Movement.Target = path[current_path_node_index].Value.transform.position;
            currentTarget = path[current_path_node_index];
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        
        try
        {
            if (!currentTarget.Value.Open)
            {
                try
                {
                    path = graph.ShortestPath(path[current_path_node_index - 1], path[path.Length - 1]).ToArray();
                    current_path_node_index = 0;
                    Movement.Target = path[current_path_node_index].Value.transform.position;
                    currentTarget = path[current_path_node_index];
                }

                catch(Exception)
                {
                    current_path_node_index = path.Length;
                    Movement.Target = current_node.Value.transform.position;
                    currentTarget = current_node;
                }
            }
        }

        catch
        {

        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject.GetComponent<LevelNode>() && is_enemy)
                {
                    current_path_node_index = 0;
                    try
                    {
                        path = graph.ShortestPath(current_node, hit.transform.gameObject.GetComponent<LevelNode>().GraphNode).ToArray();
                    }

                    catch
                    {
                        path = new GraphNode<LevelNode>[0];
                    }
                    
                    
                    //check to make sure the node we are going to is in the path. if its not we need to go back to the start to avoid clipping through the graph
                    if (!path.Contains(currentTarget))
                    {
                        Movement.Target = current_node.Value.transform.position;
                        currentTarget = current_node;
                    }
                }
            }

        }

        try
        {
            if (Movement.HasArrived)
            {
                current_node = path[current_path_node_index];
                Movement.Target = path[++current_path_node_index].Value.transform.position;
                currentTarget = path[current_path_node_index];
            }
        }

        catch
        {
            end_of_path = true;
        }
        

        base.Update();
    }
}