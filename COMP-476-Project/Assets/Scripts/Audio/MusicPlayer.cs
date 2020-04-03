using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    // gameplay track id = r+3, so gameplay 1 will be index 4

    public string curPlayingAudio;
    bool gamePlay;
    public float timer;
    public float curLength;

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

    private void Update()
    {
        
    }

    public void PlayPlayerSelectionMusic()
    {
        StopCurrentAudio();
        AudioManager.instance.Play("PlayerSelect");
        curPlayingAudio = "PlayerSelect";
    }

    public void StopCurrentAudio()
    {
        AudioManager.instance.Stop(curPlayingAudio);
        timer = 0;
    }

    public void StartGameplayTrack()
    {
        gamePlay = true;
        StopCurrentAudio();

        int r = Random.Range(1, 6);
        string str = "Gameplay";
        str += r+"";

        AudioManager.instance.Play(str);
        curPlayingAudio = str;
        curLength = AudioManager.instance.sounds[r + 3].clip.length + 3.5f;
    }




}
