using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{

    public Button cancelButton;
    public List<GameObject> listOfTowers = new List<GameObject>();

    public float xoffset=-2;

    void Start()
    {
        //instantiate all prefabs of towers
        /*
        foreach (GameObject gb in listOfTowers)
        {
            //Instantiate(gb, transform.position,Quaternion.identity,transform.GetChild(0));
            Instantiate(gb, transform.position,Quaternion.identity,transform.GetChild(0));
            xoffset += 2;
        }
        */
    }

    public void CancelButton()
    {
        Destroy(this.gameObject);
    }

    

}
