using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{

    public Button cancelButton;

    public Transform spawnPos;
    public GameObject listTransform;

    void Start()
    {

        // control lock on player
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlLock = true ;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().building = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopWalkingAnim();

    }

    public void CancelButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlLock = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().building = false;

        Destroy(this.gameObject);
    }

    /// <summary>
    /// This function checks if player can now afford any towers, so that if a player gets enough gold while the menu was open, the button to buy that 
    /// tower would be clickable.
    /// </summary>
    public void UpdateIfCanAfford()
    {
        int currentGold = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().gold;

        int count = 0;
        for (int i = 0; i < listTransform.transform.childCount; i++)
        {
            GameObject temp = listTransform.transform.GetChild(i).GetChild(0).gameObject;
            if (temp.GetComponent<TowerUIObject>().towerCost <= currentGold)
            {
                temp.GetComponent<TowerUIObject>().buildButton.interactable = true;
            }
        }
    }

    

}
