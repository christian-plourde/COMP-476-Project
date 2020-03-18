using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    // checks if grid scene was loaded from main menu, if so, only keep the appropriate player object in scene.
    // if grid scene was played by itself, just delete self, dont do anything (so that we can test easily)

    [Header("Player refs")]
    public GameObject archerRef;
    public GameObject warriorRef;

    void Awake()
    {
        GameObject gameInfo = GameObject.FindGameObjectWithTag("GameInfo");
        if (gameInfo == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            if (gameInfo.GetComponent<GameInfo>().playerClass == "Archer")
            {
                Destroy(warriorRef.gameObject);
                Camera.main.transform.GetComponent<CameraBehavior>().Target = archerRef.transform;
                archerRef.SetActive(true);
            }
            else
            {
                Destroy(archerRef.gameObject);
                Camera.main.transform.GetComponent<CameraBehavior>().Target = warriorRef.transform;
                warriorRef.SetActive(true);
            }
            Destroy(gameInfo.gameObject);
            Destroy(this.gameObject);
        }

        // music
        GameObject music = GameObject.FindGameObjectWithTag("MusicPlayer");
        if (music != null)
        {
            music.GetComponent<MusicPlayer>().StartGameplayTrack();
        }
    }

  
}
