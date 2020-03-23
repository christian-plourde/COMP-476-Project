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
    int repairCost;

    [Header("UI References")]
    public Button CancelButton;
    public Button UpgradeButton;
    public Button DestroyButton;
    public Button RepairButton;
    public Text RepairCostText;
    public Text upgradeCostText;
    public Text destroyRefundText;
    public Text TowerName;
    public Text LiveHealth;

    [Header("Comparision Text UI")]
    public Text OldDMG;
    public Text OldHP;
    public Text OldRange;
    public Text OldTier;
    public Text NewDMG;
    public Text NewHP;
    public Text NewRange;
    public Text NewTier;

    int refund;

    void Start()
    {
        LiveHealth.text = "Health: " + currentTower.GetComponent<BuildingStats>().health;
    }

    public void InitializeMenu()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerScriptRef.controlLock = true;
        playerScriptRef.StopWalkingAnim();

        float dmgPercentage = 1-(currentTower.GetComponent<BuildingStats>().health / currentTower.GetComponent<BuildingStats>().MaxHealth*1f);
        refund = (int)((currentTower.GetComponent<BuildingStats>().price / 2) - (currentTower.GetComponent<BuildingStats>().price / 2) * dmgPercentage);
        destroyRefundText.text = "Refund: " + refund;
        string str= "" + currentTower.GetComponent<BuildingStats>().name;
        //TowerName.text = "" + currentTower.GetComponent<BuildingStats>().name;
        TowerName.text = str.Remove(str.Length-7,7);      // get rid of clone in the name


        // comparison old
        if (currentTower.GetComponent<TowerAttack>() != null)
        {
            OldDMG.text = currentTower.GetComponent<TowerAttack>().damage + "";
            OldRange.text = currentTower.GetComponent<TowerAttack>().range + "";
        }
        else if (currentTower.GetComponent<DamageArea>() != null)
        {
            OldDMG.text = currentTower.GetComponent<DamageArea>().damage + "";
            OldRange.text = currentTower.GetComponent<DamageArea>().range + "";
        }
        OldHP.text = currentTower.GetComponent<BuildingStats>().MaxHealth+"";
        OldTier.text = currentTower.GetComponent<BuildingStats>().tier + "";

        // see if theres an upgrade or if player can afford it.
        GameObject upgradedTower = currentTower.GetComponent<BuildingStats>().upgrade;
        if (upgradedTower != null)
        {
            hasUpgrade = true;
            upgradeCost = upgradedTower.GetComponent<BuildingStats>().price;

            upgradeCostText.text = "Cost: " + upgradeCost;

            // comparison new
            if (currentTower.GetComponent<TowerAttack>() != null)
            {
                NewDMG.text = upgradedTower.GetComponent<TowerAttack>().damage + "";
                NewRange.text = upgradedTower.GetComponent<TowerAttack>().range + "";
            }
            else if (currentTower.GetComponent<DamageArea>() != null)
            {
                NewDMG.text = upgradedTower.GetComponent<DamageArea>().damage + "";
                NewRange.text = upgradedTower.GetComponent<DamageArea>().range + "";
            }
            NewHP.text = upgradedTower.GetComponent<BuildingStats>().health + "";
            NewTier.text = upgradedTower.GetComponent<BuildingStats>().tier + "";


            if (playerScriptRef.gold >= upgradedTower.GetComponent<BuildingStats>().price)
            {
                UpgradeButton.interactable = true;
                
            }
        }
        else
        {
            upgradeCostText.text = "No Upgrades";
        }

        if(currentTower.GetComponent<BuildingStats>().health < currentTower.GetComponent<BuildingStats>().MaxHealth)
        {
            repairCost = (int)Mathf.Ceil(currentTower.GetComponent<BuildingStats>().price * (1-(currentTower.GetComponent<BuildingStats>().health / currentTower.GetComponent<BuildingStats>().MaxHealth)));
            repairCost -= (int)(repairCost * 0.25f);              // reduction in repair cost so repairing is economic then building new ones

            RepairCostText.text = "Repair: " + repairCost;
            RepairButton.interactable = true;
        }
    }

    private void Update()
    {
        //currentTower
        LiveHealth.text = "Health: " + currentTower.GetComponent<BuildingStats>().health;

        // dynamically update the repair cost
        if (currentTower.GetComponent<BuildingStats>().health < currentTower.GetComponent<BuildingStats>().MaxHealth)
        {
            repairCost = (int)Mathf.Ceil(currentTower.GetComponent<BuildingStats>().price * (1 - (currentTower.GetComponent<BuildingStats>().health / currentTower.GetComponent<BuildingStats>().MaxHealth)));
            repairCost -= (int)(repairCost * 0.25f);              // reduction in repair cost so repairing is economic then building new ones
            RepairCostText.text = "Repair: " + repairCost;
            RepairButton.interactable = true;
        }

        // refund cost
        float dmgPercentage = 1 - (currentTower.GetComponent<BuildingStats>().health / currentTower.GetComponent<BuildingStats>().MaxHealth * 1f);
        refund = (int)((currentTower.GetComponent<BuildingStats>().price / 2) - (currentTower.GetComponent<BuildingStats>().price / 2)*dmgPercentage);
        destroyRefundText.text = "Refund: " + refund;
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

        //GameObject newTower=Instantiate(currentTower.GetComponent<BuildingStats>().upgrade.gameObject, currentTower.transform.position, Quaternion.identity);
        GameObject newTower=Instantiate(currentTower.GetComponent<BuildingStats>().upgrade.gameObject, currentTower.transform.position, currentTower.transform.rotation);
        newTower.transform.SetParent(TowerNode);

        Destroy(currentTower.gameObject);

        CloseMenu();
        SFXManager.instance.Play("BuildingUpgrade");
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




        SFXManager.instance.Play("Destroy");
        CloseMenu();
    }

    public void RepairTower()
    {
        playerScriptRef.RemoveGold(repairCost);

        currentTower.GetComponent<BuildingStats>().health = currentTower.GetComponent<BuildingStats>().MaxHealth;

        CloseMenu();
        SFXManager.instance.Play("BuildSound");
    }

    void CloseMenu()
    {

        playerScriptRef.controlLock = false;
        playerScriptRef.managingTower = false;
        Destroy(this.gameObject);
    }

    public void UpdateIfCanAfford(int gold)
    {
        if (hasUpgrade)
        {
            if (gold >= upgradeCost)
            {
                UpgradeButton.interactable = true;
            }
        }
        if (gold >= repairCost && currentTower.GetComponent<BuildingStats>().health < currentTower.GetComponent<BuildingStats>().MaxHealth)
        {
            RepairButton.interactable = true;
        }
    }
}
