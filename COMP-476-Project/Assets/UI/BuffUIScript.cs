using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIScript : MonoBehaviour
{
    PlayerBuffManager bManager;
    PlayerMovement playerRef;
    List<Buff> bList;

    public GameObject Build;
    public GameObject Run;
    public GameObject Attack;
    public Text toolTip;

    public List<Buff> BList
    {
        set { bList = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRef = FindObjectOfType<PlayerMovement>();
        playerRef.building = true;

        bManager = GameObject.FindObjectOfType<PlayerBuffManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(bList[0].category == "BUILD")
        {
            Build.SetActive(true);
        }
        else if(bList[0].category == "RUN")
        {
            Run.SetActive(true);
        }
        else if(bList[0].category == "ATTACK")
        {
            Attack.SetActive(true);
        }
    }

    public void Levelup(int uid)
    {
        Buff temp = bList[0];
        foreach(Buff b in bList)
        {
            if (b.uid == uid)
            {
                temp = b;
                break;
            }
        }

        if (temp.CanUpgrade())
        {
            bManager.LevelUp(uid);
            playerRef.building = false;
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Can't Upgrade");
        }
    }

    public void ShowToolTip(int uid)
    {
        if(uid == -1)
        {
            toolTip.text = "This upgrade is currently unavailable \n\nComing soon with the Ice and Fire DLC";
        }
        else
        {
            Buff temp = bList[0];
            foreach (Buff b in bList)
            {
                if (b.uid == uid)
                {
                    temp = b;
                    break;
                }
            }

            string color;
            if (temp.CanUpgrade())
            {
                color = "#05ff44";
            }
            else
            {
                color = "#ff0505";
            }

            toolTip.text = temp.name + "(" + temp.Level + ")" + "\n\n" + temp.description + "\n\n<color=" + color + ">Prerequisites: " + temp.PrerequisitesToString() + "</color>";
        }
        
    }

    public void DeleteToolTip()
    {
        toolTip.text = "";
    }
}
