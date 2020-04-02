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

    public Transform viewPointOfBase;

    [Header("UI References")]
    public Image healthBar;
    public Text integrity;
    public Text healthCount;

    [Header("DefeatMenu")]
    public GameObject defeatMenu;
    public GameObject quitMenu;

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
        else if (healthPercentage > 0.0)
            str = "Failing!!!";
        else
            str = "Destroyed!!!";
             


            integrity.text = "Base Integrity: "+str;
    }


    void PlayerDefeat()
    {
        // close all menus
        if (quitMenu.activeSelf)
            quitMenu.SetActive(false);

        GameObject gb = GameObject.FindGameObjectWithTag("BuildMenu");
        if (gb!=null)
        {
            Destroy(gb.gameObject);
        }
        gb = GameObject.FindGameObjectWithTag("ManageMenu");
        if (gb!=null)
        {
            Destroy(gb.gameObject);
        }
        gb = GameObject.FindGameObjectWithTag("RespawnMenu");
        if (gb != null)
        {
            Destroy(gb.gameObject);
        }



        defeatMenu.SetActive(true);
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        Player.GetComponent<PlayerMovement>().controlLock = true;
        Player.GetComponent<PlayerMovement>().invincible = true;
        Player.GetComponent<PlayerMovement>().defeated = true;

        // switch camera target to base
        Camera.main.GetComponent<CameraBehavior>().Target = viewPointOfBase;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !defeated)
        {
            BaseHealth -= other.GetComponent<EnemyAttributes>().damageToBase;
            
            if (BaseHealth < 0)
            {
                defeated = true;
                BaseHealth = 0;
                // Call defeat Condition
                PlayerDefeat();
                
            }
            UpdateUI();

        }
    }
}
