﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombieMovement : MonoBehaviour
{
    //Our zombie decision tree
    public class ZombieDT
    {
        public Func<bool> seesTower;
        public Func<bool> seesPlayer;
        public Func<bool> towerNearby;
        public Func<bool> playerNearby;

        public Action attackTower;
        public Action attackPlayer;
        public Action moveToTower;
        public Action moveToPlayer;
        public Action wander;
    }

    //Our decision tree; manages whichever movement we should be doing
    private DecisionTree m_DecisionTree = null;

    //A placeholder tower variable. Later, this will likely need to be a list
    private Transform m_Tower;
    //A reference to the player variable, to be set on instantiation of the ZombieMovement
    private Transform m_Player;

    //What we consider to be nearby
    public float m_WhatIsNearby;
    //What our error margin should be, for what's considered to be in front of us
    public float m_VisionErrorMargin;

    //Our decision tree condition nodes

    //For now I'll assume that if the player is somewhat in front of us, even if it's behind a wall, we can see it.
    //Tells us whether the player is within sight of the zombie; to be used in the decision tree
    private bool SeesPlayer()
    {
        Vector3 zombiepos = this.transform.position;
        Vector3 playerpos = this.m_Player.position;
        float dot = Vector3.Dot(this.transform.forward, (playerpos - zombiepos).normalized);
        //If our dot product is exactly 1, then the player is exactly in front of us
        float desired_result = 1.0f;
        bool result = (desired_result - m_VisionErrorMargin <= dot && dot <= desired_result + m_VisionErrorMargin);
        string message = (result) ? "Player spotted!" : "Player NOT spotted!";
        Debug.Log(message);
        //Debug.Log(message + " (" + dot + ")");//More specific logs
        return (result);
    }

    //For now I'll assume that if the tower is somewhat in front of us, even if it's behind a wall, we can see it.
    //Tells us whether a tower is within sight of the zombie; to be used in the decision tree
    private bool SeesTower()
    {
        Vector3 zombiepos = this.transform.position;
        Vector3 towerpos = this.m_Tower.position;
        float dot = Vector3.Dot(this.transform.forward, (towerpos - zombiepos).normalized);
        //If our dot product is exactly 1, then the tower is exactly in front of us
        float desired_result = 1.0f;
        bool result = (desired_result - m_VisionErrorMargin <= dot && dot <= desired_result + m_VisionErrorMargin);
        string message = (result) ? "Tower spotted" : "Tower NOT spotted";
        Debug.Log(message);
        //Debug.Log(message + " (" + dot + ")");//More specific logs
        return (result);
    }

    //Tells us whether a tower is within some distance of the zombie; to be used in the decision tree
    private bool TowerNearby()
    {
        Vector3 zombiepos = this.transform.position;
        Vector3 towerpos = this.m_Tower.position;
        bool result = (zombiepos - towerpos).magnitude <= m_WhatIsNearby;
        string message = (result) ? "Tower nearby!" : "Tower NOT nearby!";
        Debug.Log(message);
        //Debug.Log(message + "(" + (zombiepos - towerpos).magnitude + ")");//More specific logs
        return (result);
    }

    //Tells us whether the player is within sight of the zombie; to be used in the decision tree
    private bool PlayerNearby()
    {
        Vector3 zombiepos = this.transform.position;
        Vector3 playerpos = this.m_Player.position;
        bool result = (zombiepos - playerpos).magnitude <= m_WhatIsNearby;
        string message = (result) ? "Tower nearby!" : "Tower NOT nearby!";
        Debug.Log(message);
        return (result);
    }

    //Our decision tree action nodes

    private void AttackTower()
    {
        Debug.Log("Attacking tower!");
    }
    private void AttackPlayer()
    {
        Debug.Log("Attacking player!");
    }
    private void MoveToTower()
    {
        Debug.Log("Moving to tower!");
    }
    private void MoveToPlayer()
    {
        Debug.Log("Moving to player!");
    }
    private void Wander()
    {
        Debug.Log("Wandering!");
    }

    //A pseudoconstructor to allow us to easily spawn and initialize zombie movement types
    public void Initialize(Transform tower, Transform player)
    {
        //Set our instance variables
        this.m_Player = player;
        this.m_Tower = tower;

        //Set our decision tree
        //Conditions
        DTNode.ConditionNode r1n1 = new DTNode.ConditionNode(SeesTower);
        DTNode.ConditionNode r2n1 = new DTNode.ConditionNode(TowerNearby);
        DTNode.ConditionNode r2n2 = new DTNode.ConditionNode(SeesPlayer);
        DTNode.ConditionNode r3n3 = new DTNode.ConditionNode(PlayerNearby);

        //Actions
        DTNode.ActionNode r3n1 = new DTNode.ActionNode(AttackTower);
        DTNode.ActionNode r3n2 = new DTNode.ActionNode(MoveToTower);
        DTNode.ActionNode r3n4 = new DTNode.ActionNode(Wander);
        DTNode.ActionNode r4n1 = new DTNode.ActionNode(AttackPlayer);
        DTNode.ActionNode r4n2 = new DTNode.ActionNode(MoveToPlayer);

        //Assign the order of the tree, per row in our tree diagram (https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit)
        //Row 1
        r1n1.affirmative = r2n1;
        r1n1.negative = r2n2;
        //Row 2
        r2n1.affirmative = r3n1;
        r2n1.negative = r3n2;
        r2n2.affirmative = r3n3;
        r2n2.negative = r3n4;
        //Row 3
        r3n3.affirmative = r4n1;
        r3n3.negative = r4n2;

        this.m_DecisionTree = new DecisionTree(r1n1);
        //Debug.Log("Decision tree built!");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.m_DecisionTree != null)
        {
            //Debug.Log("Executing decision tree!");
            this.m_DecisionTree.Execute();
        }
    }
}
