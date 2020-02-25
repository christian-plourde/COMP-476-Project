using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombieMovement : EnemyMovement
{

    //For now I'll assume that if the player is somewhat in front of us, even if it's behind a wall, we can see it.
    //Tells us whether the player is within sight of the zombie; to be used in the decision tree
    protected override bool SeesPlayer()
    {
        //Debug.Log("ZombieMovement : SeesPlayer");
        return base.SeesPlayer();
    }

    //For now I'll assume that if the tower is somewhat in front of us, even if it's behind a wall, we can see it.
    //Tells us whether a tower is within sight of the zombie; to be used in the decision tree
    protected override bool SeesTower()
    {
        //Debug.Log("ZombieMovement : SeesTower");
        return base.SeesTower();
    }

    //Tells us whether a tower is within some distance of the zombie; to be used in the decision tree
    protected override bool TowerNearby()
    {
        //Debug.Log("ZombieMovement : TowerNearby");
        return base.TowerNearby();
    }

    //Tells us whether the player is within sight of the zombie; to be used in the decision tree
    protected override bool PlayerNearby()
    {
        //Debug.Log("ZombieMovement : PlayerNearby");
        return base.PlayerNearby();
    }

    //Our decision tree action nodes

    protected override void AttackTower()
    {
        //Debug.Log("ZombieMovement : Attacking tower!");
        base.AttackTower();
    }

    protected override void AttackPlayer()
    {
        //Debug.Log("ZombieMovement : Attacking player!");
        base.AttackPlayer();
    }

    protected override void MoveToTower()
    {
        //Debug.Log("ZombieMovement : Moving to tower!");
        base.MoveToTower();
    }

    protected override void MoveToPlayer()
    {
        //Debug.Log("ZombieMovement : Moving to player!");
        base.MoveToPlayer();
    }

    protected override void Wander()
    {
        //Debug.Log("ZombieMovement : Wandering!");
        base.Wander();
    }

}
