using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{

    public Button cancelButton;
    public List<GameObject> listOfTowers = new List<GameObject>();

    public Transform spawnPos;

    void Start()
    {

        // control lock on player
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlLock = true ;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().building = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopWalkingAnim();

    }

    public void CancelButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlLock = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().building = false;

        Destroy(this.gameObject);
    }


    

}
