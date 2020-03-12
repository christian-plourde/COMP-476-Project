using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageMenu : MonoBehaviour
{
    PlayerMovement playerScriptRef;

    public GameObject currentTower;

    [Header("UI References")]
    public Button CancelButton;
    public Button UpgradeButton;
    public Button DestroyButton;
    public Text upgradeCostText;
    public Text destroyRefundText;

    int refund;

    void Start()
    {
        playerScriptRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerScriptRef.controlLock = true;
    }

    public void InitializeMenu()
    {
        refund= (currentTower.GetComponent<BuildingStats>().price / 2);
        destroyRefundText.text = "Refund: " + refund; 
    }
    

    public void Cancel()
    {
        playerScriptRef.controlLock = false;
        playerScriptRef.managingTower = false;
        Destroy(this.gameObject);
    }

    public void Upgrade()
    { }

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






        playerScriptRef.controlLock = false;
        playerScriptRef.managingTower = false;
        Destroy(this.gameObject);
    }
}
