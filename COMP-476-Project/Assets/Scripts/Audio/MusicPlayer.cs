using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    // gameplay track id = r+3, so gameplay 1 will be index 4

    public string curPlayingAudio;

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
    
    public void PlayPlayerSelectionMusic()
    {
        StopCurrentAudio();
        AudioManager.instance.Play("PlayerSelect");
        curPlayingAudio = "PlayerSelect";
    }

    public void StopCurrentAudio()
    {
        AudioManager.instance.Stop(curPlayingAudio);
    }

    public void StartGameplayTrack()
    {
        StopCurrentAudio();

        int r = Random.Range(1, 7);
        string str = "Gameplay";
        str += r+"";

        AudioManager.instance.Play(str);
        curPlayingAudio = str;
        //curLength = AudioManager.instance.sounds[r + 3].clip.length + 3.5f;
    }




}
