using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKiller : EnemyBehaviour
{

    /// <summary>
    /// A pseudoconstructor to allow us to easily spawn and initialize enemy AI movement types
    /// Refer to this diagram https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit
    /// </summary>
    public override void Initialize()
    {

        Debug.Log("Hello?");

        grid_generator = FindObjectOfType<GenerateGrid>();
        this.m_Player = FindObjectOfType<PlayerMovement>().gameObject.transform;

        /*
        The convention for the naming of the nodes is as follows: r => row, n => number, where: r1n1 means "the first node on the first row"
        This corresponds to the diagram referred to in this Initialize function's associated summary.
         */

        //Set our decision tree
        //Conditions
        DTNode.ConditionNode r1n1 = new DTNode.ConditionNode(IsAlive);
        DTNode.ConditionNode r2n1 = new DTNode.ConditionNode(SeesPlayer);
        DTNode.ConditionNode r3n1 = new DTNode.ConditionNode(IsPlayerNearby);
        DTNode.ConditionNode r3n2 = new DTNode.ConditionNode(HasSeenPlayerRecently);
        DTNode.ConditionNode r4n4 = new DTNode.ConditionNode(SeesTower);
        DTNode.ConditionNode r5n1 = new DTNode.ConditionNode(IsTowerNearby);
        DTNode.ConditionNode r5n2 = new DTNode.ConditionNode(HasSeenTowerRecently);

        //Actions
        DTNode.ActionNode r2n2 = new DTNode.ActionNode(BeDead);
        DTNode.ActionNode r4n1 = new DTNode.ActionNode(AttackPlayer);
        DTNode.ActionNode r4n2 = new DTNode.ActionNode(MoveToPlayer);
        DTNode.ActionNode r6n1 = new DTNode.ActionNode(AttackTower);
        DTNode.ActionNode r6n2 = new DTNode.ActionNode(MoveToTower);
        DTNode.ActionNode r6n3 = new DTNode.ActionNode(Default);

        //Assign the order of the tree, per row in our tree diagram (https://docs.google.com/drawings/d/1qOOZjceQnmuGH2RxBY521rWqPFpAjUt2HtGHsXuAP04/edit)
        //Row 1
        //Are you alive?
        r1n1.affirmative = r2n1;//Then do you see the player?
        r1n1.negative = r2n2;//Then be dead
        //Row 2
        //Do you see the player?
        r2n1.affirmative = r3n1;//Then is the player nearby?
        r2n1.negative = r3n2;//Have you seen the player recently?

        //Row 3
        //is the player nearby?
        r3n1.affirmative = r4n1; //then attack the player
        r3n1.negative = r4n2; //then move to the player

        //have you seen the player recently?
        r3n2.affirmative = r3n1;//Then is the player nearby?
        r3n2.negative = r4n4;//Then have you seen the player recently?

        //Row 4
        //do you see a tower?
        r4n4.affirmative = r5n1;//then is the tower nearby?
        r4n4.negative = r5n2;//then have you seen a tower recently?

        //Row 5
        //is the tower nearby?
        r5n1.affirmative = r6n1;//then attack the tower
        r5n1.negative = r6n2;//then move to the tower

        //Have you seen a tower recently?
        r5n2.affirmative = r5n1;//then is the tower nearby?
        r5n2.negative = r6n3;//then wander

        this.m_DecisionTree = new DecisionTree(r1n1);
    }
}
