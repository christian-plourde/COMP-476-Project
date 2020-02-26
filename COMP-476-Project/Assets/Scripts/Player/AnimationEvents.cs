using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    /*
     * Player Object structure:
     * Parent ---> just a holder, hard coded not to rotate on Y.
     * Child object --> mesh with colliders, animator component
     * 
     * Movement and combat scripts are placed on the Parent (Holder)
     * This script is placed on the child (Mesh) to use animation events.
     */
    public int playerClass;


    PlayerMovement movementScriptRef;
    CombatBehavior combatScriptRef;
    WarriorCombatBehavior warriorCombatRef;
    Animator animator;

    GameObject SwordRef;

    private void Start()
    {
        movementScriptRef = transform.parent.GetComponent<PlayerMovement>();
        if (playerClass == 1)
            combatScriptRef = transform.parent.GetComponent<CombatBehavior>();
        else
        {
            warriorCombatRef = transform.parent.GetComponent<WarriorCombatBehavior>();
            SwordRef = warriorCombatRef.HandSword;
        }
        animator = GetComponent<Animator>();
    }

    // just methods for animation events
    public void ArcherShoot()
    {
        combatScriptRef.ArcherShoot();
    }

    public void ArcherEquipArrow()
    {
        combatScriptRef.ArcherArrowEquip();
    }

    public void ArcherUnequipArrow()
    {
        combatScriptRef.ArcherArrowSheath();
    }



    // Warrior
    public void EndOfFastAttack1()
    {
        warriorCombatRef.attackingSword = false;
        //Debug.Log("Animation Event called");
        animator.SetLayerWeight(2, 0);
        animator.SetBool("FastAttack1",false);

        
        movementScriptRef.controlLock = false;

        SwordColliderOff();
        warriorCombatRef.attackingSword = false;
        warriorCombatRef.attackTimer = 0;

        warriorCombatRef.fastAttack2 = false;
    }

    public void EndOfFastAttack2()
    {
        warriorCombatRef.attackingSword = false;
        //Debug.Log("Animation Event called");
        animator.SetLayerWeight(2, 0);
        animator.SetBool("FastAttack1", false);
        animator.SetBool("FastAttack2", false);
        animator.SetBool("FastAttack3", false);


        movementScriptRef.controlLock = false;

        SwordColliderOff();
        warriorCombatRef.attackingSword = false;
        warriorCombatRef.attackTimer = 0;

        warriorCombatRef.fastAttack2 = false;
        warriorCombatRef.fastAttack3 = false;
    }

    public void EndOfFastAttack3()
    {
        warriorCombatRef.attackingSword = false;
        //Debug.Log("Animation Event called");
        animator.SetLayerWeight(2, 0);
        animator.SetBool("FastAttack1", false);
        animator.SetBool("FastAttack2", false);
        animator.SetBool("FastAttack3", false);


        movementScriptRef.controlLock = false;

        SwordColliderOff();
        warriorCombatRef.attackingSword = false;
        warriorCombatRef.attackTimer = 0;

        warriorCombatRef.fastAttack2 = false;
        warriorCombatRef.fastAttack3 = false;
    }

    public void EndOfUltimateSmash()
    {
        movementScriptRef.controlLock = false;
        movementScriptRef.warriorUltimate = false;
        
        // reset variables
        warriorCombatRef.ultimateCooldown = true;
        warriorCombatRef.usingUltimate = false;
        warriorCombatRef.ultimateTimer = 0;

        //reset speed
        movementScriptRef.ResetSpeed();
        animator.SetLayerWeight(2, 0);

        animator.SetBool("Ultimate", false);
        animator.SetBool("UltimateSmash", false);
    }


    public void SwordColliderOn()
    {
        SwordRef.GetComponent<BoxCollider>().enabled = true;
    }
    public void SwordColliderOff()
    {
        SwordRef.GetComponent<BoxCollider>().enabled = false;

    }

    public void ChainFastAttack2()
    {
        warriorCombatRef.fastAttack2 = true;
    }

    public void ChainFastAttack3()
    {
        warriorCombatRef.fastAttack3 = true;
    }
}
