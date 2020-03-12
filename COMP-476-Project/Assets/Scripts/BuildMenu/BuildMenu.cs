using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{

    public GameObject selectedTower;

    public Button cancelButton;

    void Start()
    {
        //instantiate all prefabs of towers
    }

    void CancelButton()
    {
        Destroy(this.gameObject);
    }

    

}
