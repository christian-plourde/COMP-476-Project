using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : EnemyBehaviour
{
    #region Condition Functions
    protected virtual bool IsSomethingBlockingTheWayToThePlayerBase()
    {
        
        return false;
    }

    #endregion

    /// <summary>
    /// A pseudoconstructor to allow us to easily spawn and initialize enemy AI movement types
    /// Refer to this diagram https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit
    /// </summary>
    public override void Initialize()
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
        DTNode.ConditionNode r2n1 = new DTNode.ConditionNode(IsSomethingBlockingTheWayToThePlayerBase);
        DTNode.ConditionNode r3n1 = new DTNode.ConditionNode(SeesTower);
        DTNode.ConditionNode r4n1 = new DTNode.ConditionNode(IsTowerNearby);
        DTNode.ConditionNode r4n2 = new DTNode.ConditionNode(HasSeenTowerRecently);


        //Actions
        DTNode.ActionNode r2n2 = new DTNode.ActionNode(BeDead);
        DTNode.ActionNode r3n2 = new DTNode.ActionNode(Default);
        DTNode.ActionNode r5n1 = new DTNode.ActionNode(AttackTower);
        DTNode.ActionNode r5n2 = new DTNode.ActionNode(MoveToTower);

        //Assign the order of the tree, per row in our tree diagram (https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit)
        //Row 1
        //Are you alive?
        r1n1.affirmative = r2n1;//Then is there something blocking your way to the player base?
        r1n1.negative = r2n2;//Then be dead

        //Row 2
        //Is there something blocking your way to the player base?
        r2n1.affirmative = r3n1; //then do you see a tower?
        r2n1.negative = r3n2;//then base seek

        //Row 3
        //Do you see a tower?
        r3n1.affirmative = r4n1; //then is the tower nearby?
        r3n1.negative = r4n2;//then have you seen a tower recently?

        //Row 4
        //Is the tower nearby?
        r4n1.affirmative = r5n1;//then attack the tower
        r4n1.negative = r5n2;//then move to the tower

        //have you seen a tower recently?
        r4n2.affirmative = r4n1;//then is the tower nearby?
        r4n2.negative = r3n2;//then base seek

        this.m_DecisionTree = new DecisionTree(r1n1);
    }
}
