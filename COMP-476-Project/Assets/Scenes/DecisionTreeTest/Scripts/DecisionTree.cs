using System.Collections;
using System.Collections.Generic;
using System;

public class DTNode
{
    //The "current" node
    public DTNode root;
    //What to do if our condition evaluates to true (null on leaf)
    public DTNode affirmative = null;
    //What to do if our condition evaluates to false (null on leaf)
    public DTNode negative = null;

    public DTNode(DTNode root)
    {
        this.root = root;
    }

    //A function to be defined in ConditionNode and ActionNode
    public virtual bool Evaluate()
    {
        //To be overridden
        return false;
    }

    //Tells us whether the curent node is a leaf
    public bool isLeaf()
    {
        return this.affirmative == null && this.negative == null;
    }

    //A node type for all non-leaf nodes; evaluates a condition
    public class ConditionNode : DTNode
    {
        Func<bool> condition;

        public ConditionNode(Func<bool> condition)
            : base(null)
        {
            this.condition = condition;
        }

        public override bool Evaluate()
        {
            return this.condition();
        }
    }

    //A node type for all leaf nodes; executes an action
    public class ActionNode : DTNode
    {
        Action action;

        public ActionNode(Action action)
            : base(null)
        {
            this.action = action;
        }

        public override bool Evaluate()
        {
            this.action();
            return false;
        }
    }
}

//A decision tree made up of DT nodes
public class DecisionTree
{
    DTNode root;

    //Build the tree by setting the root, which should have its children set beforehand
    public DecisionTree(DTNode root)
    {
        this.root = root;
    }

    //Recursively evaluate all relevant conditions of the tree, based on the state we use to build the tree
    public void Execute()
    {
        this.ExecuteNode(root);
    }

    //A function to execute one node of the decision tree
    private void ExecuteNode(DTNode node)
    {
        //if the node is a leaf...
        if (node.isLeaf())
        {
            //...then we have an action to execute
            node.Evaluate();
        }
        //else if the node is not a leaf...
        else
        {
            //...then evaluate the condition...
            if (node.Evaluate())
            {
                //...if true, execute affirmative child
                ExecuteNode(node.affirmative);
            }
            else
            {
                //...if false, execute negative child
                ExecuteNode(node.negative);
            }
        }
    }
}



////Usage example
//public class Character
//{
//    public bool tired;
//    public bool hungry;

//    public bool IsHungry()
//    {
//        return this.hungry;
//    }

//    public bool IsTired()
//    {
//        return this.tired;
//    }

//    public void Sleep()
//    {
//        Debug.Log("Sleeping!");
//    }

//    public void Eat()
//    {
//        Debug.Log("Eating!");
//    }

//    public void EatAndSleep()
//    {
//        Debug.Log("Eating and sleeping!");
//    }
//}
//
//int main()
//{
//    Character character = new Character();
//    //Set character state; based on this, the tree executes
//    character.tired = false;
//    character.hungry = true;

//    DTNode.ConditionNode node = new DTNode.ConditionNode(character.IsTired);
//    DTNode.ConditionNode n2 = new DTNode.ConditionNode(character.IsHungry);
//    DTNode.ConditionNode n3 = new DTNode.ConditionNode(character.IsHungry);
//    DTNode.ActionNode n4 = new DTNode.ActionNode(character.Sleep);
//    DTNode.ActionNode n5 = new DTNode.ActionNode(character.Eat);
//    DTNode.ActionNode n6 = new DTNode.ActionNode(character.Sleep);
//    DTNode.ActionNode n7 = new DTNode.ActionNode(character.EatAndSleep);

//    node.negative = n2;
//    node.affirmative = n3;

//    n2.negative = n4;
//    n2.affirmative = n5;
//    n3.negative = n6;
//    n3.affirmative = n7;

//    DecisionTree tree = new DecisionTree(node);
//    tree.Execute();
//}