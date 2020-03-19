using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnButton : MonoBehaviour
{
    [Header("UI References")]
    public Button respawnButton;
    public Text TextTimer;


    [Header("Variables")]
    GameObject Player;
    float respawnTimer;
    public float respawnTime;
    public int requiredBuyBackGold;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        respawnButton.transform.GetChild(0).GetComponent<Text>().text = "Respawn Now (" + requiredBuyBackGold + " Gold)";
        if (Player.GetComponent<PlayerMovement>().gold < requiredBuyBackGold)
        {
            respawnButton.interactable = false;
        }
    }

    void Update()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer > respawnTime)
        {
            AutoRespawn();
        }
        TextTimer.text = "" + (int)(respawnTime - respawnTimer);

    }

    public void RespawnPlayer()
    {
        //Debug.Log("Clicked Respawn");
        Player.GetComponent<PlayerMovement>().RespawnPlayer(true);
        Player.GetComponent<PlayerMovement>().RemoveGold(requiredBuyBackGold);
        Destroy(this.gameObject);

    }

    public void AutoRespawn()
    {
        Player.GetComponent<PlayerMovement>().RespawnPlayer(false);
        Destroy(this.gameObject);

    }

    public void UpdateIfCanAfford(int gold)
    {
        if (requiredBuyBackGold <= gold)
        {
            respawnButton.interactable = true;
        }
    }
}
