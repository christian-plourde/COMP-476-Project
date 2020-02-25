using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorCombatBehavior : MonoBehaviour
{


    private Animator animator;
    public bool Attacking; // if in attack mode, if false, you can build

    //public GameObject ProjectilePrefab;
    //public Transform LaunchPoint;
    //public Transform AttackTarget;

    Transform PlayerMesh;

    [Header("Weapon Slots")]
    public GameObject BackSword;
    public GameObject HandSword;
    //public GameObject HeldArrow;
    //public GameObject AbilityCircle;

    //float mouseClickTime = 0f;

    PlayerMovement PlayerMovementRef;



    [HideInInspector] public bool ultimateCooldown;
    float ultimateCooldownTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        PlayerMesh = transform.GetChild(0);

        HandSword.SetActive(false);
        BackSword.SetActive(false);

        PlayerMovementRef = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
