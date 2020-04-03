using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject AllButtonsParent;
    public GameObject ControlsParent;

    public void StartGame()
    {
        // load player class selecter menu
        GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<MusicPlayer>().StopCurrentAudio();
        SceneManager.LoadScene("Player Select");
    }

    public void StartCredits()
    {
        //load credits scene
    }

    public void Controls()
    {
        AllButtonsParent.SetActive(false);
        ControlsParent.SetActive(true);
    }

    public void BackControls()
    {
        AllButtonsParent.SetActive(true);
        ControlsParent.SetActive(false);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
