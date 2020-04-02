﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit_Endgame : MonoBehaviour
{
    public GameObject quitMenu;
    public GameObject endGameMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        quitMenu.SetActive(false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controlLock = false;
    }

    public void QuitGame()
    {
        //destroy audio objects
        // load main menu

        GameObject gb=GameObject.FindGameObjectWithTag("SFXManager");
        if (gb != null)
            Destroy(gb);

        gb = GameObject.FindGameObjectWithTag("AudioManager");
        if (gb != null)
            Destroy(gb);

        SceneManager.LoadScene("MainMenu");

    }
}
