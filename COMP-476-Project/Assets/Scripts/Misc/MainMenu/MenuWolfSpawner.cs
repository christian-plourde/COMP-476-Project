using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWolfSpawner : MonoBehaviour
{
    public float timer;
    float spawnTimer;
    bool spawning;
    bool finishedIntro;


    public float spawnFrequency=0.95f;

    public GameObject wolfPrefab;
    public GameObject zombiePrefab;
    public GameObject halblitzerPrefab;
    public Transform spawnLocation;

    
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        

        if (timer > 85 && !spawning && !finishedIntro)
        {
            spawning = true;
        }

        if (spawning)
        {
            SpawnWolves();
        }

        void SpawnWolves()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnFrequency && timer<105)
            {
                spawnTimer = 0;
                Vector3 spawnLoc = spawnLocation.position;
                float offsetX = Random.Range(-1.5f,1.5f);
                spawnLoc.x += offsetX;

                int r = Random.Range(0, 10);
                if(r<=1)
                    Instantiate(halblitzerPrefab, spawnLoc, Quaternion.identity);
                else if(r<=5)
                    Instantiate(zombiePrefab, spawnLoc, Quaternion.identity);
                else
                    Instantiate(wolfPrefab, spawnLoc, Quaternion.identity);
            }

            if (timer > 120)
            {
                finishedIntro = true;
                Camera.main.GetComponent<Animator>().SetBool("FinalPosition",true);
            }
        }
    }
}
