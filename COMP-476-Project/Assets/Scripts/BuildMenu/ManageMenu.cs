using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageMenu : MonoBehaviour
{
    PlayerMovement playerScriptRef;

    public GameObject currentTower;
    public bool hasUpgrade;
    public int upgradeCost;

    [Header("UI References")]
    public Button CancelButton;
    public Button UpgradeButton;
    public Button DestroyButton;
    public Text upgradeCostText;
    public Text destroyRefundText;

    int refund;

    void Start()
    {
        //playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        //playerScriptRef.controlLock = true;
    }

    public void InitializeMenu()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerScriptRef.controlLock = true;

        refund = (currentTower.GetComponent<BuildingStats>().price / 2);
        destroyRefundText.text = "Refund: " + refund;

        // see if theres an upgrade or if player can afford it.
        GameObject upgradedTower = currentTower.GetComponent<BuildingStats>().upgrade;
        if (upgradedTower != null)
        {
            hasUpgrade = true;
            upgradeCost = upgradedTower.GetComponent<BuildingStats>().price;

            upgradeCostText.text = "Cost: " + upgradeCost;
            if (playerScriptRef.gold >= upgradedTower.GetComponent<BuildingStats>().price)
            {
                UpgradeButton.interactable = true;
                
            }
        }
        else
        {
            upgradeCostText.text = "No Upgrades";
        }
    }
    

    public void Cancel()
    {
        CloseMenu();
    }

    public void Upgrade()
    {
        playerScriptRef.RemoveGold(currentTower.GetComponent<BuildingStats>().upgrade.GetComponent<BuildingStats>().price);

        //replace towers on that node.
        Transform TowerNode = currentTower.transform.parent;

        GameObject newTower=Instantiate(currentTower.GetComponent<BuildingStats>().upgrade.gameObject, currentTower.transform.position, Quaternion.identity);
        newTower.transform.SetParent(TowerNode);

        Destroy(currentTower.gameObject);

        CloseMenu();
    }

    public void DestroyTower()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Map");
        GameObject gridObject = arr[0];
        foreach (GameObject gb in arr)
        {
            if (gb.transform.name == "Floor")
            {
                gridObject = gb;
                break;
            }
        }
        gridObject.GetComponent<GenerateGrid>().DestroyTower(currentTower, refund);





        CloseMenu();
    }

    void CloseMenu()
    {

        playerScriptRef.controlLock = false;
        playerScriptRef.managingTower = false;
        Destroy(this.gameObject);
    }

    public void UpdateIfCanAfford()
    {
        if (hasUpgrade)
        {
            if (playerScriptRef.gold >= upgradeCost)
            {
                UpgradeButton.interactable = true;
            }
        }
    }
}
