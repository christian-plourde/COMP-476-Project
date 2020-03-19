using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDetail : MonoBehaviour
{
    // when raycaster hits an object, read its details and put it in canvas
    public string whichAbility;

    public string abilityName;
    public int abilityCoolDown;
    [TextArea]
    public string abilityDetails;

    public Image AbilityBG;

    Color ogColor;
    public bool hovering;

    private void Start()
    {
        ogColor = AbilityBG.color;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if(!hovering)
            AbilityBG.color = ogColor;
    }
}
