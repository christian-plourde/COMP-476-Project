using System.Collections.Generic;
using UnityEngine;

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
    protected List<GameObject> tower_nodes;

    /// <summary>
    /// List of all level node scripts in the game
    /// </summary>
    protected List<LevelNode> tower_level_nodes;

    /// <summary>
    /// The game object representation of the node (the green disc).
    /// </summary>
    protected GameObject target_tower_node;

    /// <summary>
    /// The level node script on the target_tower_node.
    /// </summary>
    protected LevelNode target_tower_level_node;

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

    [SerializeField] protected bool m_OutputDebugLogs = false;

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
    /// <summary>
    /// the time for which the enemy should hunt the player after having seen them, given that the enemy no longer sees the player. In other words, after X seconds, lost interest in chasing the player.
    /// </summary>
    public float m_AttentionSpan = 2.0f;
    /// <summary>
    /// A timer to keep track of when the last time we attacked was.
    /// </summary>
    protected float m_AttackTimer = 0.0f;
    /// <summary>
    /// A timer to keep track of when we last saw the player.
    /// </summary>
    protected float m_PlayerLastSeenTimer = 0.0f;
    /// <summary>
    /// A timer to keep track of when we last saw a tower.
    /// </summary>
    protected float m_TowerLastSeenTimer = 0.0f;
    /// <summary>
    /// The rate at which we increase our timer variable
    /// </summary>
    private const float DELTA_T = 0.025f;

    /// <summary>
    /// A reference to the character component associated to this gameobject. Initialized in Start()
    /// </summary>
    protected Character m_Character;
    /// <summary>
    /// A reference to the enemy's attributes, i.e. health
    /// </summary>
    protected EnemyAttributes m_EnemyAttributes;

    #region Condition Functions

    /// <summary>
    /// Tells us whether the enemy is still alive; this will in turn determine whether the rest of the decision tree should be considered at all.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsAlive()
    {
        bool result = false;
        try
        {
            result = this.m_EnemyAttributes.health > 0.0f;
        }

        catch {
            result = true;        
        }
        
        if (m_OutputDebugLogs)
        {
            Debug.Log("Enemy " + (result ? "IS" : "ISN'T") + " alive!");
        }
        return result;
    }

    /// <summary>
    /// Tells us whether the player is within sight of the enemy; to be used in the decision tree.
    /// Reinitializes [m_PlayerLastSeenTimer] on enemy sees player.
    /// The closer the enemy is to the target, the more likely the enemy will see the target.
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesPlayer()
    {
        
        Vector3 enemypos = this.transform.position;
        Vector3 playerpos;
        try
        {
            playerpos = this.m_Player.position;
        }

        catch
        {
            if (m_OutputDebugLogs)
            {
                Debug.Log("Player position doesn't exist!");
            }
            return false;
        }

        float dot = Vector3.Dot(this.transform.forward, (playerpos - enemypos).normalized);
        //If our dot product is exactly 1, then the player is exactly in front of us
        float desired_result = 1.0f;
        float distance_to_player = (playerpos - enemypos).magnitude;
        float m = this.m_RangeOfVision / distance_to_player;
        if (m > 2.0f) { m = 2.0f; }
        //bool facing_player = (desired_result - m_VisionErrorMargin <= dot);
        bool facing_player = (desired_result - m <= dot);

        bool result = false;
        string message = "";
        //if we're facing the target, then we can consider a raycast
        if (facing_player)
        {
            message += "Enemy is facing the player. ";

            Vector3 height_offset = new Vector3(0.0f, 0.5f, 0.0f);
            Vector3 origin = this.transform.position + height_offset;
            Vector3 direction = (this.m_Player.transform.position - origin).normalized;
            //Put end [range of vision] units toward the player
            Vector3 end = origin + direction * this.m_RangeOfVision;
            if (m_OutputDebugLogs)
            {
                Debug.DrawLine(origin, end, Color.red, 1.0f);
            }

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, this.m_RangeOfVision))
            {
                if (hit.collider.gameObject.GetComponent<PlayerMovement>() != null)
                {
                    //Reset the timer on seeing the player
                    this.m_PlayerLastSeenTimer = 0.0f;
                    //Increment it just a touch, to get HasSeenPlayerRecently moving
                    this.m_PlayerLastSeenTimer += DELTA_T;

                    result = true;
                }
            }
        }//end if facing player
        if (m_OutputDebugLogs)
        {
            string template = " see the player.";
            message += (result ? " Enemy DOES": " Enemy DOES NOT") + template;
            Debug.Log(message);
        }
        return result;
    }

    //For now I'll assume that if the tower is somewhat in front of us, even if it's behind a wall, we can see it.
    /// <summary>
    /// Tells us whether a tower is within sight of the enemy; condition function to be used in the decision tree.
    /// The closer the enemy is to its target, the more likely the enemy is to see the target.
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesTower()
    {
        Vector3 enemypos = this.transform.position;
        bool result = false;

        //For each node where we have a tower
        foreach(GameObject tower_node in tower_nodes)
        {
            //get me the LevelNode of that node
            LevelNode node = tower_node.GetComponent<LevelNode>();
            //ensure the tower at our LevelNode exists
            if (node.Tower != null)
            {
                //if our tower exists, tell me whether we can actually see it
                Vector3 tower_height_offset = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 towerpos = node.Tower.transform.position + tower_height_offset;
                float dot = Vector3.Dot(this.transform.forward, (towerpos - enemypos).normalized);
                //If our dot product is exactly 1, then the tower is exactly in front of us
                float desired_result = 1.0f;
                float distance_to_tower = (towerpos - enemypos).magnitude;
                float m = this.m_RangeOfVision / distance_to_tower;
                if (m > 2.0f) { m = 2.0f; }

                //The closer we are, the lower our threshold for "facing" the target is. If we are very close to the target, even if we are facing away from the target, we will consider that we are "facing" the target.
                bool facing_tower = (desired_result - m <= dot);

                string message = "";
                //if we're facing the target, then we can consider a raycast
                if (facing_tower)
                {
                    message += "Enemy is facing the tower. ";

                    Vector3 height_offset = new Vector3(0.0f, 0.5f, 0.0f);
                    Vector3 origin = this.transform.position + height_offset;
                    Vector3 direction = (towerpos - origin).normalized;
                    //Put end [range of vision] units toward the tower
                    Vector3 end = origin + direction * this.m_RangeOfVision;
                    if (m_OutputDebugLogs)
                    {
                        Debug.DrawLine(origin, end, Color.blue, 1.0f);
                    }

                    RaycastHit hit;
                    //if we hit something with our raycast...
                    if (Physics.Raycast(origin, direction, out hit, this.m_RangeOfVision))
                    {
                        //...and if that something is a tower...
                        if (hit.collider.gameObject.GetComponent<BuildingStats>() != null)
                        {
                            //...then update our target tower node, to tell us where to run to
                            m_TargetTowerNode = tower_node;

                            this.m_TowerLastSeenTimer = 0.0f;
                            this.m_TowerLastSeenTimer += DELTA_T;

                            //return true and break us out of here.
                            result = true;
                        }
                    }
                }//end if facing player
                //output logs if needed, before breaking out
                if (m_OutputDebugLogs)
                {
                    string template = " see the tower.";
                    message += (result ? " Enemy DOES" : " Enemy DOES NOT") + template;
                    Debug.Log(message);
                }
                //if we have result is true, then return now.
                if (result) { return result; }
            }//end if tower exists
        }//end foreach

        return result;
    }

    /// <summary>
    /// Tells us whether a tower is within some distance of the enemy AI; to be used in the decision tree
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsTowerNearby()
    {
        bool result = false;
        //For each level node containing a tower...
        foreach(LevelNode n in tower_level_nodes)
        {
            //...if the node we're at is a neighbor to this node where there is a tower...
            if(this.m_Character.CurrentNode.Neighbors.Contains(n.GraphNode))
            {
                //...then we consider this tower to be nearby
                m_TargetTowerNode = n.gameObject;
                result = true;
                break;
            }
        }

        if (m_OutputDebugLogs)
        {
            Debug.Log("Tower " + (result ? "IS " : "ISN'T") + " nearby!");
        }
        return result;
    }

    /// <summary>
    /// Tells us whether or not the player's been seen in the last [ some amount of time
    /// </summary>
    /// <returns></returns>
    protected virtual bool HasSeenPlayerRecently()
    {
        //On seeing the player, we reset the timer to 0 and add DELTA T.
        bool timer_in_range = this.m_PlayerLastSeenTimer > 0.0f && this.m_PlayerLastSeenTimer < this.m_AttentionSpan;
        //if the timer is greater than 0, then keep incrementing it
        if (timer_in_range)
        {
            this.m_PlayerLastSeenTimer += DELTA_T;
            if (this.m_PlayerLastSeenTimer > this.m_AttentionSpan)
            {
                this.m_PlayerLastSeenTimer = 0.0f;
            }
        }
        if (m_OutputDebugLogs)
        {
            Debug.Log("Enemy " + (timer_in_range ? "HAS " : "HASN'T") + " seen player recently!");
        }
        return timer_in_range;
    }

    protected virtual bool HasSeenTowerRecently()
    {
        //On seeing tower, we reset the timer to 0 and add DELTA T.
        bool timer_in_range = this.m_TowerLastSeenTimer > 0.0f && this.m_TowerLastSeenTimer < this.m_AttentionSpan;

        if(timer_in_range)
        {
            this.m_TowerLastSeenTimer += DELTA_T;
            if(this.m_TowerLastSeenTimer > this.m_AttentionSpan)
            {
                this.m_TowerLastSeenTimer = 0.0f;
            }
        }
        return timer_in_range;
    }

    /// <summary>
    /// Tells us whether the player is within some distance [m_WhatIsNearby] of the enemy AI; to be used in the decision tree
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsPlayerNearby()
    {
        
        Vector3 enemypos = this.transform.position;
        Vector3 playerpos = this.m_Player.position;
        bool result = (enemypos - playerpos).magnitude <= m_WhatIsNearby;
        //string message = (result) ? "Player nearby!" : "Player NOT nearby!";
        //Debug.Log(message);
        if (m_OutputDebugLogs)
        {
            Debug.Log("Player " + (result ? "IS " : "ISN'T") + " nearby!");
        }
        return (result);
    }

    protected virtual bool IsSomethingBlockingTheWayToThePlayerBase()
    {
        foreach (LevelNode n in grid_generator.PlayerBaseNodes)
        {
            LinkedList<Graph.GraphNode<LevelNode>> path = grid_generator.Graph.ShortestPath(this.GetComponent<Character>().CurrentNode, n.GraphNode);
            if (path.Count == 0)
                return true;
        }

        return false;
    }

    #endregion

    #region Action Functions

    /// <summary>
    /// The function to be executed on !isAlive
    /// </summary>
    protected virtual void BeDead()
    {
        if (m_OutputDebugLogs)
        {
            Debug.Log("Being dead!");
        }
        return;
    }

    /// <summary>
    /// The function to be executed on SeesTower & TowerNearby; for use in the decision tree
    /// </summary>
    protected virtual void AttackTower()
    {
        if (m_OutputDebugLogs)
        {
            Debug.Log("Attacking tower!");
        }
        if (this.m_TargetTowerNode.GetComponent<LevelNode>().Tower == null)
        {
            return;
        }

        //Make enemy face player in order to force continuous attack
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.ATTACK_TOWER;
        //if our attack timer is negative, we haven't started attacking yet
        if (m_AttackTimer == 0.0f)
        {
            //Debug.Log("Dealing damage to tower");
            //Attack Tower
            float attack_damage = 0.0f;
            try
            {
                attack_damage = this.GetComponent<EnemyAttributes>().damage;
            }

            catch 
            {
                try
                {
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        attack_damage += this.transform.GetChild(i).gameObject.GetComponent<EnemyAttributes>().damage;
                    }
                }

                catch { }
                
            }
            //Debug.Log("attacking with damage: " + attack_damage);
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
        if (m_OutputDebugLogs)
        {
            Debug.Log("Attacking player!");
        }
        //Make enemy face player in order to force continuous attack
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.ATTACK_PLAYER;

        if (m_AttackTimer == 0.0f)
        {
            //Debug.Log("Dealing damage to player");
            //Attack player
            float attack_damage = 0.0f;
            try
            {
                attack_damage = this.GetComponent<EnemyAttributes>().damage;
            }

            catch 
            {
                try
                {
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        attack_damage += this.transform.GetChild(i).gameObject.GetComponent<EnemyAttributes>().damage;
                    }
                }

                catch { }
            }
            
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
        if (m_OutputDebugLogs)
        {
            Debug.Log("Moving to tower!");
        }
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.MOVE_TO_TOWER;
    }

    /// <summary>
    /// The function to be executed on SeesPlayer & !PlayerNearby; for use in the decision tree.
    /// We set the Character behaviour type from this function, which manages movement in response to the behaviour.
    /// </summary>
    protected virtual void MoveToPlayer()
    {
        if (m_OutputDebugLogs)
        {
            Debug.Log("Moving to player!");
        }
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.MOVE_TO_PLAYER;
    }

    /// <summary>
    /// Our default behaviour; have the enemy move towards the player's base
    /// </summary>
    protected virtual void Default()
    {
        if (m_OutputDebugLogs)
        {
            Debug.Log("Base seeking!");
        }
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.BASE_SEEK;
    }

    protected virtual void MoveToClosestTower()
    {
        double smallest_dist = double.MaxValue;

        foreach (LevelNode n in tower_level_nodes)
        {
            double dist = (n.transform.position - this.gameObject.transform.position).magnitude;
            if (dist < smallest_dist)
            {
                smallest_dist = dist;
                this.m_TargetTowerNode = n.gameObject;
            }
        }
        this.m_Character.BehaviourType = BEHAVIOUR_TYPE.MOVE_TO_TOWER;
    }

    #endregion

    /// <summary>
    /// A pseudoconstructor to allow us to easily spawn and initialize enemy AI movement types
    /// Refer to this diagram https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit
    /// </summary>
    public virtual void Initialize()
    {
        grid_generator = FindObjectOfType<GenerateGrid>();
        this.m_Player = FindObjectOfType<PlayerMovement>().gameObject.transform;

        /*
        The convention for the naming of the nodes is as follows: r => row, n => number, where: r1n1 means "the first node on the first row"
        This corresponds to the diagram referred to in this Initialize function's associated summary.
         */

        //Set our decision tree
        //Conditions
        DTNode.ConditionNode r1n1 = new DTNode.ConditionNode(IsAlive);
        DTNode.ConditionNode r2n1 = new DTNode.ConditionNode(SeesTower);
        DTNode.ConditionNode r2n3 = new DTNode.ConditionNode(HasSeenTowerRecently);
        DTNode.ConditionNode r3n1 = new DTNode.ConditionNode(IsTowerNearby);
        DTNode.ConditionNode r3n2 = new DTNode.ConditionNode(SeesPlayer);
        DTNode.ConditionNode r4n3 = new DTNode.ConditionNode(IsPlayerNearby);
        DTNode.ConditionNode r4n4 = new DTNode.ConditionNode(HasSeenPlayerRecently);
        DTNode.ConditionNode r5n3 = new DTNode.ConditionNode(IsPlayerNearby);
        DTNode.ConditionNode r6n4 = new DTNode.ConditionNode(IsSomethingBlockingTheWayToThePlayerBase);

        //Actions
        DTNode.ActionNode r2n2 = new DTNode.ActionNode(BeDead);
        DTNode.ActionNode r4n1 = new DTNode.ActionNode(AttackTower);
        DTNode.ActionNode r4n2 = new DTNode.ActionNode(MoveToTower);
        DTNode.ActionNode r5n1 = new DTNode.ActionNode(AttackPlayer);
        DTNode.ActionNode r5n2 = new DTNode.ActionNode(MoveToPlayer);
        DTNode.ActionNode r5n4 = new DTNode.ActionNode(Default);
        DTNode.ActionNode r6n1 = new DTNode.ActionNode(AttackPlayer);
        DTNode.ActionNode r6n2 = new DTNode.ActionNode(MoveToPlayer);
        DTNode.ActionNode r6n3 = new DTNode.ActionNode(MoveToClosestTower);


        //Assign the order of the tree, per row in our tree diagram (https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit)
        //Row 1
        //Are you alive?
        r1n1.affirmative = r2n1;//Then do you see a tower?
        r1n1.negative = r2n2;//Then do nothing
        //Row 2
        //Do you see a tower?
        r2n1.affirmative = r3n1;//Then is the tower nearby?
        r2n1.negative = r2n3;//Have you seen a tower recently?

        //Row 3
        //Have you seen a tower recently?
        r2n3.affirmative = r3n1; //Is tower nearby?
        r2n3.negative = r3n2; //Do you see the player?

        //Row 3
        //Is the tower nearby?
        r3n1.affirmative = r4n1;//Then attack the tower
        r3n1.negative = r4n2;//Then move to the tower
        //Do you see the player?
        r3n2.affirmative = r4n3;//Then is the player nearby?
        r3n2.negative = r4n4;//Then have you seen the player recently?

        //Row 4
        //Is the player nearby?
        r4n3.affirmative = r5n1;//Then attack the player
        r4n3.negative = r5n2;//Then move to the player

        //Have you seen the player recently?
        r4n4.affirmative = r5n3;//then is the player nearby?
        r4n4.negative = r6n4;//then base seek / default movement

        //is path blocked
        r6n4.affirmative = r6n3; //move to closest tower
        r6n4.negative = r5n4; //move to base/default movement

        //Row 5
        //Is the player nearby?
        r5n3.affirmative = r6n1;//then attack the player
        r5n3.negative = r6n2;//then move to the player

        this.m_DecisionTree = new DecisionTree(r1n1);
    }

    // Update is called once per frame
    private void Update()
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
        this.m_EnemyAttributes = this.GetComponent<EnemyAttributes>();
    }
}
