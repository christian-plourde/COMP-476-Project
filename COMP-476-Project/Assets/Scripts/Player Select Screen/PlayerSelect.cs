﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{

    public GameObject raycastTarget=null;

    [Header("UI References")]
    public Text ArcherAbilityName;
    public Text ArcherAbilityDetails;

    public Text WarriorAbilityName;
    public Text WarriorAbilityDetails;

    [Header("Starting References")]
    public GameObject ArcherA1;
    public GameObject WarriorA1;

    [Header("GameInfo ref")]
    public GameObject info;

    bool gameLaunched;

    private void Start()
    {
        ArcherAbilityName.text = ArcherA1.GetComponent<AbilityDetail>().abilityName;
        ArcherAbilityDetails.text = ArcherA1.GetComponent<AbilityDetail>().abilityDetails;

        WarriorAbilityName.text=WarriorA1.GetComponent<AbilityDetail>().abilityName;
        WarriorAbilityDetails.text=WarriorA1.GetComponent<AbilityDetail>().abilityDetails;

        GameObject gb = GameObject.FindGameObjectWithTag("MusicPlayer");
        if (gb != null)
        {
            gb.GetComponent<MusicPlayer>().PlayPlayerSelectionMusic();
        }
    }

    private void Update()
    {
        if(!gameLaunched)
            RayCastInput();
    }

    void RayCastInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && raycastTarget.transform.name!=hit.transform.name)
        {
            raycastTarget.GetComponent<AbilityDetail>().hovering = false;
            raycastTarget = hit.transform.gameObject;

            // change details
            AbilityDetail aDetails = hit.transform.GetComponent<AbilityDetail>();

            if (aDetails.whichAbility == "Archer")
            {
                ArcherAbilityName.text = aDetails.abilityName;
                ArcherAbilityDetails.text = aDetails.abilityDetails;
                raycastTarget.GetComponent<AbilityDetail>().hovering = true;
                raycastTarget.GetComponent<AbilityDetail>().AbilityBG.color = Color.yellow;
            }
            else
            {
                WarriorAbilityName.text = aDetails.abilityName;
                WarriorAbilityDetails.text = aDetails.abilityDetails;
                raycastTarget.GetComponent<AbilityDetail>().hovering = true;
                raycastTarget.GetComponent<AbilityDetail>().AbilityBG.color = Color.yellow;

            }
        }
    }

    // buttons
    public void GoBack()
    {
        GameObject gb=GameObject.FindGameObjectWithTag("MusicPlayer");
        if (gb != null)
        {
            gb.GetComponent<MusicPlayer>().StopCurrentAudio();
            Destroy(gb.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
        Destroy(info.gameObject);
    }

    public void StartGameArcher()
    {
        if (!gameLaunched)
        {
            info.GetComponent<GameInfo>().playerClass = "Archer";
            try
            {
                AudioManager.instance.Play("StartGame");
            }
            catch { }
            gameLaunched = true;
            Invoke("LaunchGame", 1f);
            StopMusic();
        }
    }

    public void StartGameWarrior()
    {
        if (!gameLaunched)
        {
            info.GetComponent<GameInfo>().playerClass = "Warrior";
            try
            {
                AudioManager.instance.Play("StartGame");
            }
            catch { }
            gameLaunched = true;
            Invoke("LaunchGame", 1f);
            StopMusic();
        }
        
    }

    void LaunchGame()
    {
        SceneManager.LoadScene("Grid");
    }

    void StopMusic()
    {
        GameObject gb = GameObject.FindGameObjectWithTag("MusicPlayer");
        if (gb != null)
            gb.GetComponent<MusicPlayer>().StopCurrentAudio();
    }
}
