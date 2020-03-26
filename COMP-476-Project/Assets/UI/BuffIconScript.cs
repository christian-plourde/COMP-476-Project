using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconScript : MonoBehaviour
{
    Sprite icon;
    Sprite gIcon;
    Buff buff;

    public Image childColor;
    public Image childGray;


    public Sprite Icon
    {
        set { icon = value; }
    }

    public Sprite GIcon
    {
        set { gIcon = value; }
    }

    public Buff Buff
    {
        set { buff = value; }
    }

    // Start is called before the first frame update
    void Start()
    {

        childColor.GetComponent<Image>().sprite = icon;
        childGray.GetComponent<Image>().sprite = gIcon;
    }

    // Update is called once per frame
    void Update()
    {
        if(buff.uid == 6)
        {
            if (buff.BuffMethods.Fastest_man_alive_charged)
            {
                childColor.fillAmount = 1;
            }
            else
            {
                childColor.fillAmount = buff.BuffMethods.FastestManAliveChargeProgress;
            }
        }
        else
        {
            if (buff.Active)
            {
                childColor.gameObject.SetActive(true);
                childGray.gameObject.SetActive(false);
            }
            else
            {
                childColor.gameObject.SetActive(false);
                childGray.gameObject.SetActive(true);
            }
        }
        
    }
}
