﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    [Tooltip("This script contains fields common to all buildings as well as getters for each value.")]
    public int price, tier;
    public float health;
    public Dictionary<string, int> multipliers = new Dictionary<string, int>();
    public string description;

    [Header("Upgrades if any")]
    public GameObject upgrade;

    private void Start()
    {
        multipliers.Add("health", 1);
        multipliers.Add("price", 1);
    }

    public int GetPrice()
    {
        return price*multipliers["price"];
    }

    public int GetTier()
    {
        return tier;
    }

    public float GetHealth()
    {
        return health*multipliers["price"];
    }

    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    /// A function to cause damage to the tower
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(float damage)
    {
        this.health -= damage;
        if (this.health <= 0.0f)
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
            gridObject.GetComponent<GenerateGrid>().DestroyTower(this.gameObject, 0);
        }
    }
}
