using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUIObject : MonoBehaviour
{
    public GameObject towerPrefab;

    [Header("Details")]
    public string towerName;
    public int towerCost;
    public string towerType;

    GameObject playerRef;

    public Button buildButton;
    public Text towerNameUI;
    public Text costUI;
    public Image towerImageUI;

    //stats
    [Header("Tower Stats")]
    public Text damageText;
    public Text healthText;
    public Text rangeText;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");

        towerCost = towerPrefab.GetComponent<BuildingStats>().price;
        towerName = towerPrefab.GetComponent<BuildingStats>().name;
        Sprite towerImage = towerPrefab.GetComponent<BuildingStats>().towerImage;

        towerNameUI.text = "" + towerName;
        costUI.text = "Cost: " + towerCost;

        if (towerPrefab.GetComponent<TowerAttack>() != null)
        {
            damageText.text = "Damage: " + towerPrefab.GetComponent<TowerAttack>().damage;
            rangeText.text = "Range: " + towerPrefab.GetComponent<TowerAttack>().range;
        }
        else if (towerPrefab.GetComponent<DamageArea>()!=null)
        {
            damageText.text = "Damage: " + towerPrefab.GetComponent<DamageArea>().damage;
            rangeText.text = "Range: " + towerPrefab.GetComponent<DamageArea>().range;
        }
        else
        {
            damageText.text = "Damage: -";
            rangeText.text = "Range: -";
        }
        healthText.text = "Health: " + towerPrefab.GetComponent<BuildingStats>().health;
        towerImageUI.sprite = towerImage;


        CheckIfAffordable();
    }

    public void CheckIfAffordable()
    {
        if (towerCost > playerRef.GetComponent<PlayerMovement>().gold)
        {
            buildButton.interactable = false;
        }
    }

    public void BuildTower()
    {
        //Debug.Log("Building towering my friending");

        // build the actual tower
        GameObject[]arr=GameObject.FindGameObjectsWithTag("Map");
        GameObject gridObject=arr[0];
        foreach (GameObject gb in arr)
        {
            if (gb.transform.name == "Floor")
            {
                gridObject = gb;
                break;
            }
        }
        gridObject.GetComponent<GenerateGrid>().PlaceTower(towerPrefab, GameObject.FindGameObjectWithTag("BuildMenu").GetComponent<BuildMenu>().spawnPos);

        // deduct money
        playerRef.GetComponent<PlayerMovement>().RemoveGold(towerCost);

        GameObject.FindGameObjectWithTag("BuildMenu").GetComponent<BuildMenu>().CancelButton();
        SFXManager.instance.Play("BuildSound");
        
    }
}
