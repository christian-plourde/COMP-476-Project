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

    PlayerMovement movementScriptRef;
    CombatBehavior combatScriptRef;

    private void Start()
    {
        movementScriptRef = transform.parent.GetComponent<PlayerMovement>();
        combatScriptRef = transform.parent.GetComponent<CombatBehavior>();
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


}
