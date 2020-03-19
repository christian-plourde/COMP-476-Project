using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseIntegrity : MonoBehaviour
{
    // if base health falls zero or below, you lose the game.
    // base can be repaired (expensive) during prep phase only.

    public int BaseHealth;
    int maxBaseHealth;
    bool defeated;

    [Header("UI References")]
    public Image healthBar;
    public Text integrity;
    public Text healthCount;

    private void Start()
    {
        maxBaseHealth = BaseHealth;
        healthCount.text = maxBaseHealth+"";
    }

    void UpdateUI()
    {
        float healthPercentage = ((float)(BaseHealth) / (float)(maxBaseHealth));
        healthBar.fillAmount = healthPercentage;
        healthCount.text = BaseHealth+"";

        // integrity
        string str="";
        if (healthPercentage > 0.9)
            str = "Strong";
        else if (healthPercentage > 0.7)
            str = "Great";
        else if (healthPercentage > 0.5)
            str = "Holding";
        else if (healthPercentage > 0.3)
            str = "Weak";
        else if (healthPercentage > 0.1)
            str = "Critical!!";
        else
            str = "Failing!!!";
             


            integrity.text = "Base Integrity: "+str;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            BaseHealth -= other.GetComponent<EnemyAttributes>().damageToBase;
            UpdateUI();
            if (BaseHealth < 0)
            {
                BaseHealth = 0;
                // Call defeat Condition
            }
        }
    }
}
