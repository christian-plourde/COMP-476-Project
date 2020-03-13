using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWolfSpawner : MonoBehaviour
{
    public float timer;
    float spawnTimer;
    bool spawning;
    bool finishedIntro;

    public GameObject wolfPrefab;
    public GameObject zombiePrefab;
    public Transform spawnLocation;

    Vector3 camOgPosition;
    Quaternion camOgRotation;


    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 74.95f && timer < 75.00f)
        {
            camOgPosition = Camera.main.transform.position;
            camOgRotation = Camera.main.transform.rotation;
        }

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
            if (spawnTimer > 0.75f && timer<140)
            {
                spawnTimer = 0;
                Vector3 spawnLoc = spawnLocation.position;
                float offsetX = Random.Range(-1.5f,1.5f);
                spawnLoc.x += offsetX;

                int r = Random.Range(1, 10);
                if(r<=3)
                    Instantiate(zombiePrefab, spawnLoc, Quaternion.identity);
                else
                    Instantiate(wolfPrefab, spawnLoc, Quaternion.identity);
            }

            if (timer > 155)
            {
                finishedIntro = true;
                Camera.main.GetComponent<Animator>().SetBool("FinalPosition",true);
            }
        }
    }
}
