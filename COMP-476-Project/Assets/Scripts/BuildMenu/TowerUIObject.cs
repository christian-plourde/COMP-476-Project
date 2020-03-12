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

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");

        towerCost = towerPrefab.GetComponent<BuildingStats>().price;
        towerName = towerPrefab.GetComponent<BuildingStats>().name;

        towerNameUI.text = "" + towerName;
        costUI.text = towerCost + " Gold";
        CheckIfAffordable();
    }

    public void CheckIfAffordable()
    {
        if (towerCost > playerRef.GetComponent<PlayerMovement>().gold)
        {
            buildButton.interactable = false;
        }
    }
}
