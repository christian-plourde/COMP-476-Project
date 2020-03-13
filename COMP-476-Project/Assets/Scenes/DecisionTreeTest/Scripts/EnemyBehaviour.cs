using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBehaviour : MonoBehaviour
{
    /// <summary>
    /// Our decision tree; manages whichever movement we should be doing, and manages transitions between decisions and behaviours internally.
    /// </summary>
    protected DecisionTree m_DecisionTree = null;

    /// <summary>
    /// A reference to the player transform, so we know what the player's position is.
    /// </summary>
    [HideInInspector]
    public Transform m_Player;

    /// <summary>
    /// list of all target_tower_node objects in the game
    /// </summary>
    private List<GameObject> tower_nodes;

    /// <summary>
    /// List of all level node scripts in the game
    /// </summary>
    private List<LevelNode> tower_level_nodes;

    /// <summary>
    /// The game object representation of the node (the green disc).
    /// </summary>
    private GameObject target_tower_node;

    /// <summary>
    /// The level node script on the target_tower_node.
    /// </summary>
    private LevelNode target_tower_level_node;

    /// <summary>
    /// The public accessible property corresponding to our target tower level node
    /// </summary>
    public GameObject m_TargetTowerNode 
    { 
        get { return target_tower_node; }
        set { target_tower_node = value; 
            try
            {
                target_tower_level_node = target_tower_node.GetComponent<LevelNode>();
            }

            catch
            {

            }
        }
    }

    /// <summary>
    /// A reference to the global game grid generator (who contains the actual grid)
    /// </summary>
    [HideInInspector]
    public GenerateGrid grid_generator;

    /// <summary>
    /// What we consider to be nearby.
    /// </summary>
    public float m_WhatIsNearby;
    /// <summary>
    /// What our error margin should be, for what's considered to be in front of us.
    /// </summary>
    public float m_VisionErrorMargin;
    /// <summary>
    /// How far we can see
    /// </summary>
    public float m_RangeOfVision;
    /// <summary>
    /// The frequency at which the enemy should be able to attack.
    /// </summary>
    public float m_AttackFrequency = 1.5f;
    private float m_AttackTimer = 0.0f;
    /// <summary>
    /// The rate at which we increase our timer variable
    /// </summary>
    private const float DELTA_T = 0.025f;

    /// <summary>
    /// A reference to the character component associated to this gameobject. Initialized in Start()
    /// </summary>
    protected Character m_Character;


    //Our decision tree condition nodes

    /// <summary>
    /// Tells us whether the player is within sight of the zombie; to be used in the decision tree.
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesPlayer()
    {
        Vector3 enemypos = this.transform.position;
        Vector3 playerpos;
        try
        {
           playerpos  = this.m_Player.position;
        }

        catch
        {
            return false;
        }
        
        float dot = Vector3.Dot(this.transform.forward, (playerpos - enemypos).normalized);
        //If our dot product is exactly 1, then the player is exactly in front of us
        float desired_result = 1.0f;
        bool facing_player = (desired_result - m_VisionErrorMargin <= dot && dot <= desired_result + m_VisionErrorMargin);
        //string message = (facing_player) ? "Player spotted!" : "Player NOT spotted!";
        //Debug.Log(message);

        //if we're facing the target, then we can consider a raycast
        if (facing_player)
        {
            //TEMPORARY
            return true;

            //RaycastHit hit;
            //if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.forward), out hit, this.m_RangeOfVision))
            //{
            //    if (hit.transform.tag == "Player")
            //    {
            //        return true;
            //    }
            //}
        }
        //Debug.Log(message + " (" + dot + ")");//More specific logs
        return false;
    }

    //For now I'll assume that if the tower is somewhat in front of us, even if it's behind a wall, we can see it.
    /// <summary>
    /// Tells us whether a tower is within sight of the enemy; condition function to be used in the decision tree.
    /// Note: also implements a failsafe distance check and arbitrarily returns true if the tower is close enough to the enemy. This is to compensate for the tower being progressively harder to see as you get closer to it.
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesTower()
    {
        Vector3 enemypos = this.transform.position;

        foreach(GameObject tower_node in tower_nodes)
        {
            float dot = Vector3.Dot(this.transform.forward, (tower_node.transform.position - enemypos).normalized);
            //If our dot product is exactly 1, then the tower is exactly in front of us
            float desired_result = 1.0f;
            bool facing_tower = (desired_result - m_VisionErrorMargin <= dot && dot <= desired_result + m_VisionErrorMargin);

            //Debug.Log(message + " (" + dot + ")");//More specific logs
            //string message = (facing_tower) ? "Tower spotted" : "Tower NOT spotted";
            //Debug.Log(message);
            //if we're facing the target, then we can consider a raycast
            if (facing_tower)
            {
                
                //TEMPORARY
                m_TargetTowerNode = tower_node;
                //Debug.Log(message);
                return true;

                //RaycastHit hit;
                //if(Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.forward), out hit, this.m_RangeOfVision))
                //{
                //    if you hit a graph node and it's closed, it has a tower, and that means you saw a tower
                //if (hit.transform.tag == "GraphNode")
                //{
                //    if (!hit.transform.gameObject.getComponent<LevelNode>().Open)
                //    {
                //        target_tower = g;
                //        return true;
                //    }
                //    return false
                //}
                //}
                //return false;
            }//end if

            //The closer you are, the harder it gets to see a tower.
            //As a failsafe, if you come within some minimum distance of a tower, just attack it, you're blind and it's next to you.
            float distance = (tower_node.transform.position - this.transform.position).magnitude;
            if (distance <= m_WhatIsNearby)
            {
                //Debug.Log("Enemy doesn't see tower but is passing next to one; attacking!");
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tells us whether a tower is within some distance of the enemy AI; to be used in the decision tree
    /// </summary>
    /// <returns></returns>
    protected virtual bool TowerNearby()
    {
        //For each level node containing a tower...
        foreach(LevelNode n in tower_level_nodes)
        {
            //...if the node we're at is a neighbor to this node where there is a tower...
            if(this.m_Character.CurrentNode.Neighbors.Contains(n.GraphNode))
            {
                //...then we consider this tower to be nearby
                m_TargetTowerNode = n.gameObject;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tells us whether the player is within some distance [m_WhatIsNearby] of the enemy AI; to be used in the decision tree
    /// </summary>
    /// <returns></returns>
    protected virtual bool PlayerNearby()
    {
        Vector3 enemypos = this.transform.position;
        Vector3 playerpos = this.m_Player.position;
        bool result = (enemypos - playerpos).magnitude <= m_WhatIsNearby;
        //string message = (result) ? "Player nearby!" : "Player NOT nearby!";
        //Debug.Log(message);
        return (result);
    }

    /// <summary>
    /// The function to be executed on SeesTower & TowerNearby; for use in the decision tree
    /// </summary>
    protected virtual void AttackTower()
    {
       // Debug.Log("Attacking tower!");
        //Make enemy face player in order to force continuous attack
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.ATTACK_TOWER;
        //if our attack timer is negative, we haven't started attacking yet
        if (m_AttackTimer == 0.0f)
        {
            Debug.Log("Dealing damage to tower");
            //Attack Tower
            float attack_damage = this.GetComponent<EnemyAttributes>().damage;
            this.m_TargetTowerNode.GetComponent<LevelNode>().Tower.GetComponent<BuildingStats>().Damage(attack_damage);
            this.m_AttackTimer += DELTA_T;
        }
        else if (m_AttackTimer >= this.m_AttackFrequency)
        {
            this.m_AttackTimer = 0;
        }
        else if (this.m_AttackTimer < this.m_AttackFrequency)
        {
            this.m_AttackTimer += DELTA_T;
        }
    }

    /// <summary>
    /// The function to be executed on SeesPlayer & PlayerNearby; for use in the decision tree
    /// </summary>
    protected virtual void AttackPlayer()
    {
        //Debug.Log("Attacking player!");
        //Make enemy face player in order to force continuous attack
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.ATTACK_PLAYER;

        if (m_AttackTimer == 0.0f)
        {
            Debug.Log("Dealing damage to player");
            //Attack player
            float attack_damage = this.GetComponent<EnemyAttributes>().damage;
            this.m_Player.GetComponent<PlayerMovement>().DealDamage(attack_damage);
            this.m_AttackTimer += DELTA_T;
        }
        else if (m_AttackTimer >= this.m_AttackFrequency)
        {
            this.m_AttackTimer = 0;
        }
        else if (this.m_AttackTimer < this.m_AttackFrequency)
        {
            this.m_AttackTimer += DELTA_T;
        }
    }

    /// <summary>
    /// The function to be executed on SeesTower & !TowerNearby; for use in the decision tree.
    /// We set the Character behaviour type from this function, which manages movement in response to the behaviour.
    /// </summary>
    protected virtual void MoveToTower()
    {
        //Debug.Log("Moving to tower!");
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.MOVE_TO_TOWER;
    }

    /// <summary>
    /// The function to be executed on SeesPlayer & !PlayerNearby; for use in the decision tree.
    /// We set the Character behaviour type from this function, which manages movement in response to the behaviour.
    /// </summary>
    protected virtual void MoveToPlayer()
    {
        //Debug.Log("Moving to player!");
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.MOVE_TO_PLAYER;
    }

    /// <summary>
    /// Our default behaviour; have the enemy move towards the player's base
    /// </summary>
    protected virtual void Default()
    {
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.BASE_SEEK;
    }

    /// <summary>
    /// A pseudoconstructor to allow us to easily spawn and initialize enemy AI movement types
    /// </summary>
    public void Initialize()
    {
        grid_generator = FindObjectOfType<GenerateGrid>();
        this.m_Player = FindObjectOfType<PlayerMovement>().gameObject.transform;

        //Set our decision tree
        //Conditions
        DTNode.ConditionNode r1n1 = new DTNode.ConditionNode(SeesTower);
        DTNode.ConditionNode r2n1 = new DTNode.ConditionNode(TowerNearby);
        DTNode.ConditionNode r2n2 = new DTNode.ConditionNode(SeesPlayer);
        DTNode.ConditionNode r3n3 = new DTNode.ConditionNode(PlayerNearby);

        //Actions
        DTNode.ActionNode r3n1 = new DTNode.ActionNode(AttackTower);
        DTNode.ActionNode r3n2 = new DTNode.ActionNode(MoveToTower);
        DTNode.ActionNode r3n4 = new DTNode.ActionNode(Default);
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
    }

    // Update is called once per frame
    protected void Update()
    {
        tower_nodes = new List<GameObject>();
        tower_level_nodes = new List<LevelNode>();
        if(grid_generator == null || grid_generator.Graph.Nodes == null)
        {
            grid_generator = FindObjectOfType<GenerateGrid>();
            return;
        }

        foreach(Graph.GraphNode<LevelNode> n in grid_generator.Graph.Nodes)
        {
            //if (!n.Value.Open)
            //    towers.Add(n.Value.Tower);
            if (!n.Value.Open)
            {
                tower_nodes.Add(n.Value.gameObject);
                tower_level_nodes.Add(n.Value);
            }
                
        }

        //If the decision tree is initialized, then manage its execution
        if (this.m_DecisionTree != null)
        {            
            this.m_DecisionTree.Execute();
        }
    }

    private void Start()
    {
        this.m_Character = this.GetComponent<Character>();
    }
}
