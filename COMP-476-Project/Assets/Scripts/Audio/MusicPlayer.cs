using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{

    public string curPlayingAudio;
    public float timer;
    public float stopTime;

    void Start()
    {
        Scene curScene = SceneManager.GetActiveScene();
        if (curScene.name == "MainMenu")
        {
            AudioManager.instance.Play("Titlescreen");
            curPlayingAudio = "Titlescreen";
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void StopCurrentAudio()
    {
        AudioManager.instance.Stop(curPlayingAudio);
        timer = 0;
    }
    




}
