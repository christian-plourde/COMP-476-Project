using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        // load player class selecter menu
        GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<MusicPlayer>().StopCurrentAudio();
        SceneManager.LoadScene("Grid");
    }

    public void StartCredits()
    {
        //load credits scene
    }

    public void Options()
    {

    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
