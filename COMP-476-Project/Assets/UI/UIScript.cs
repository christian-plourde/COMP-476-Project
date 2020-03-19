using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public Sprite wAbility1;
    public Sprite wAbility2;
    public Sprite wAbility3;
    public Sprite aAbility1;
    public Sprite aAbility2;
    public Sprite aAbility3;
    public Sprite wAbility2G;
    public Sprite wAbility3G;
    public Sprite aAbility2G;
    public Sprite aAbility3G;
    public Sprite fightMode;
    public Sprite buildMode;

    public Image health;
    public Image mode;
    public Image ability1;
    public Image ability2;
    public Image ability3;
    public Image ability2Cd;
    public Image ability3Cd;

    public Text gold;
    public Text wave;

    bool isArcher;

    PlayerMovement playerRef;
    CombatBehavior archerRef;
    WarriorCombatBehavior warriorRef;

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GetComponent<PlayerMovement>();
        if(playerRef.playerClass == "Archer")
        {
            isArcher = true;
            archerRef = GetComponent<CombatBehavior>();
            ability1.sprite = aAbility1;
            ability2.sprite = aAbility2;
            ability3.sprite = aAbility3;
            ability2Cd.sprite = aAbility2G;
            ability3Cd.sprite = aAbility3G;
        }
        else
        {
            isArcher = false;
            warriorRef = GetComponent<WarriorCombatBehavior>();
            ability1.sprite = wAbility1;
            ability2.sprite = wAbility2;
            ability3.sprite = wAbility3;
            ability2Cd.sprite = wAbility2G;
            ability3Cd.sprite = wAbility3G;
        }

        ability2Cd.fillAmount = 0;
        ability3Cd.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        gold.text = playerRef.gold.ToString();
        health.fillAmount = playerRef.health / playerRef.maxHealth;
        if (playerRef.inBuildMode)
        {
            mode.sprite = buildMode;
            ability1.enabled = false;
            ability2.enabled = false;
            ability3.enabled = false;
        }
        else
        {
            mode.sprite = fightMode;
            ability1.enabled = true;
            ability2.enabled = true;
            ability3.enabled = true;
        }

        if (isArcher)
        {
            if(archerRef.secondaryArrowCooldown)
                ability2Cd.fillAmount =(archerRef.howLongSecondaryCooldown - archerRef.secondaryArrowCooldownTimer) / archerRef.howLongSecondaryCooldown;
            if (archerRef.ultimateCooldown)
                ability3Cd.fillAmount = (archerRef.howLongUltimateCooldown - archerRef.ultimateCooldownTimer) / archerRef.howLongUltimateCooldown;
        }
        else
        {
            if(warriorRef.secondaryCooldown)
                ability2Cd.fillAmount = (warriorRef.howLongSecondaryCooldown - warriorRef.secondaryCooldownTimer) / warriorRef.howLongSecondaryCooldown;
            if(warriorRef.ultimateCooldown)
                ability3Cd.fillAmount = (warriorRef.howLongUltimateCooldown - warriorRef.ultimateCooldownTimer) / warriorRef.howLongUltimateCooldown;
        }
    }
}
