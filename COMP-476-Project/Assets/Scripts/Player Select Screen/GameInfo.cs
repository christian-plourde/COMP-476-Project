using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public string playerClass;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
