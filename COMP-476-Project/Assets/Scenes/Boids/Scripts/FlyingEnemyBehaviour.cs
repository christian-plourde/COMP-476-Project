using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyBehaviour : MonoBehaviour
{
    [SerializeField] private bool m_OutputDebugLogs;
    /// <summary>
    /// A string for debugging the current state
    /// </summary>
    string m_DebugMessage = "";

    private Transform m_Player;

    /// <summary>
    /// The decision tree describing the movement behaviours of the enemy. See the diagram at https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit
    /// </summary>
    DecisionTree m_DecisionTree = null;

    /// <summary>
    /// A reference to our enemy attributes.
    /// </summary>
    private EnemyAttributes m_EnemyAttributes;

    /// <summary>
    /// A pseudoconstructor to allow us to easily spawn and initialize enemy AI movement types
    /// Refer to this diagram https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit
    /// </summary>
    public  void Initialize()
    {
        //this.m_Player = FindObjectOfType<PlayerMovement>().gameObject.transform;

        /*
        The convention for the naming of the nodes is as follows: r => row, n => number, where: r1n1 means "the first node on the first row"
        This corresponds to the diagram referred to in this Initialize function's associated summary.
         */

        //Set our decision tree
        //Conditions
        DTNode.ConditionNode r1n1 = new DTNode.ConditionNode(IsAlive);
        DTNode.ConditionNode r2n1 = new DTNode.ConditionNode(SeesOtherFlyingEnemies);
        DTNode.ConditionNode r3n1 = new DTNode.ConditionNode(AreOtherFlyingEnemiesBusy);
        DTNode.ConditionNode r3n2 = new DTNode.ConditionNode(SeesLandEnemies);
        DTNode.ConditionNode r4n3 = new DTNode.ConditionNode(AreLandEnemiesAttackingATower);
        DTNode.ConditionNode r4n4 = new DTNode.ConditionNode(SeesPlayer);

        //Actions
        DTNode.ActionNode r2n2 = new DTNode.ActionNode(BeDead);
        DTNode.ActionNode r4n1 = new DTNode.ActionNode(ImitateOtherFlyingEnemies);
        DTNode.ActionNode r4n2 = new DTNode.ActionNode(AssimilateOtherFlyingEnemies);
        DTNode.ActionNode r5n1 = new DTNode.ActionNode(DistractTower);
        DTNode.ActionNode r5n2 = new DTNode.ActionNode(ProtectLandEnemies);
        DTNode.ActionNode r5n3 = new DTNode.ActionNode(DiveBomb);
        DTNode.ActionNode r5n4 = new DTNode.ActionNode(Wander);
        

        //Assign the order of the tree, per row in our tree diagram (https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit)
        //Row 1
        //Are you alive?
        r1n1.affirmative = r2n1;//then do you see flying enemies?
        r1n1.negative = r2n2;//then be dead
        //Row 2
        //Do you see flying enemies?
        r2n1.affirmative = r3n1;//then are the flying enemies doing something other than wandering?
        r2n1.negative = r3n2;//then do you see land enemies?

        //Row 3
        //Are other flying enemies doing something other than wandering?
        r3n1.affirmative = r4n1;//Then do what they're doing
        r3n1.negative = r4n2;//Then assimilate them
        //Do you see any land enemies?
        r3n2.affirmative = r4n3;//Then are the land enemies attacking a tower?
        r3n2.negative = r4n4;//Then do you see the player?

        //Row 4
        //Are land enemies attacking a tower?
        r4n3.affirmative = r5n1;//then distract the tower
        r4n3.negative = r5n2;//then protect the land enemies

        //do you see the player?
        r4n4.affirmative = r5n3;//then dive bomb them
        r4n4.negative = r5n4;//then wander

        this.m_DecisionTree = new DecisionTree(r1n1);
    }

    #region Condition Functions
    //Condition functions
    /// <summary>
    /// Tells us whether the enemy is still alive; this will in turn determine whether the rest of the decision tree should be considered at all.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsAlive()
    {
        //As this is the first node in our decision tree, we can reset our debug string here.
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage = "";
        }

        bool result = false;
        try
        {
            result = this.m_EnemyAttributes.health > 0.0f;
        }

        catch
        {
            result = true;
        }

        if (m_OutputDebugLogs)
        {
            string m = ("C| Enemy " + (result ? "IS" : "ISN'T") + " alive!");
            this.m_DebugMessage += m + " \n -> ";
        }
        return result;
    }

    /// <summary>
    /// Return true on sighting other flying enemies
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesOtherFlyingEnemies()
    {
        bool result = false;


        if (m_OutputDebugLogs)
        {
            string m = ("C| Enemy " + (result ? "DOES" : "DOESN'T") + " see other flying enemies!");
            this.m_DebugMessage += m + " \n -> ";
        }
        return result;
    }

    /// <summary>
    /// Return true if other flying enemies are doing something other than wandering.
    /// </summary>
    /// <returns></returns>
    protected virtual bool AreOtherFlyingEnemiesBusy()
    {
        bool result = false;


        if (m_OutputDebugLogs)
        {
            string m = ("C| Flying enemies " + (result ? "ARE" : "AREN'T") + " doing anything but wandering!");
            this.m_DebugMessage += m + " \n -> ";
        }
        return result;
    }

    /// <summary>
    /// Return true if enemy sees fellow land enemies.
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesLandEnemies()
    {
        bool result = false;


        if (m_OutputDebugLogs)
        {
            string m = ("C| Flying enemies " + (result ? "DO" : "DON'T") + " see land enemies!");
            this.m_DebugMessage += m + " \n -> ";
        }
        return result;
    }

    /// <summary>
    /// Return true for land enemies currently being engaged in the ATTACK_TOWER movement pattern
    /// </summary>
    /// <returns></returns>
    protected virtual bool AreLandEnemiesAttackingATower()
    {
        bool result = false;


        if (m_OutputDebugLogs)
        {
            string m = ("C| Land enemies " + (result ? "ARE" : "AREN'T") + " attacking a tower!");
            this.m_DebugMessage += m + " \n -> ";
        }
        return result;
    }

    /// <summary>
    /// Return true for the flying enemy spotting the player
    /// </summary>
    /// <returns></returns>
    protected virtual bool SeesPlayer()
    {


        bool result = false;


        if (m_OutputDebugLogs)
        {
            string m = ("C| Flying enemies " + (result ? "DO" : "DON'T") + " see the player!");
            this.m_DebugMessage += m + " \n -> ";
        }
        return result;
    }
    #endregion

    #region Action Functions
    //Actions

    /// <summary>
    /// A function to run on death
    /// </summary>
    protected virtual void BeDead()
    {
        //Do nothing
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is being dead!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    /// <summary>
    /// Imitate whatever the other flying enemies are doing
    /// </summary>
    protected virtual void ImitateOtherFlyingEnemies()
    {
        //Do nothing
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is imitating other flying enemies!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    /// <summary>
    /// Assimilate the other flying enemies
    /// </summary>
    protected virtual void AssimilateOtherFlyingEnemies()
    {
        //Do nothing
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is assimilating other flying enemies!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    /// <summary>
    /// A function to have the flying enemy draw the fire of the towers from the land enemies.
    /// Run on: no flying enemies seen -> land enemies seen -> they're attacking a tower.
    /// </summary>
    protected virtual void DistractTower()
    {
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is distracting tower!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    /// <summary>
    /// A function to have the flying enemy draw the fire of the towers from the land enemies.
    /// Run on: no flying enemies seen -> land enemies seen -> they're NOT attacking a tower.
    /// </summary>
    protected virtual void ProtectLandEnemies()
    {
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is protecting land enemies!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    /// <summary>
    /// A function to have the flying enemy attack the player
    /// Run on: no flying enemies seen -> land enemies NOT seen -> player seen
    /// </summary>
    protected virtual void DiveBomb()
    {
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is divebombing the player!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    /// <summary>
    /// A function to have the flying enemy attack the player
    /// Run on: no flying enemies seen -> land enemies NOT seen -> player NOT seen
    /// </summary>
    protected virtual void Wander()
    {
        if (this.m_OutputDebugLogs)
        {
            this.m_DebugMessage += "A| Is wandering!";
            Debug.Log(this.m_DebugMessage);
        }
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (this.m_DecisionTree != null)
        {
            this.m_DecisionTree.Execute();
        }
    }
}
