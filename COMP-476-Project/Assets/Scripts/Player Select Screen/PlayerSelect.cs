using System.Collections;
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

    private void Start()
    {
        ArcherAbilityName.text = ArcherA1.GetComponent<AbilityDetail>().abilityName;
        ArcherAbilityDetails.text = ArcherA1.GetComponent<AbilityDetail>().abilityDetails;

        WarriorAbilityName.text=WarriorA1.GetComponent<AbilityDetail>().abilityName;
        WarriorAbilityDetails.text=WarriorA1.GetComponent<AbilityDetail>().abilityDetails;

        GameObject gb = GameObject.FindGameObjectWithTag("MusicPlayer");
        if (gb != null)
        {
            //play selection music
        }
    }

    private void Update()
    {
        RayCastInput();
    }

    void RayCastInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && raycastTarget.transform.name!=hit.transform.name)
        {
            raycastTarget = hit.transform.gameObject;

            // change details
            AbilityDetail aDetails = hit.transform.GetComponent<AbilityDetail>();

            if (aDetails.whichAbility == "Archer")
            {
                ArcherAbilityName.text = aDetails.abilityName;
                ArcherAbilityDetails.text = aDetails.abilityDetails;
            }
            else
            {
                WarriorAbilityName.text = aDetails.abilityName;
                WarriorAbilityDetails.text = aDetails.abilityDetails;
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
    }

    public void StartGameArcher()
    {
        AudioManager.instance.Play("StartGame");
    }

    public void StartGameWarrior()
    {
        AudioManager.instance.Play("StartGame");
    }

    void LaunchGame(GameObject playerPrefab)
    {

    }
}
