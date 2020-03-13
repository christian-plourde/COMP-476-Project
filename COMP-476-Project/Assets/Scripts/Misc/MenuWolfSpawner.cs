using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWolfSpawner : MonoBehaviour
{
    public float timer;
    float spawnTimer;
    bool spawning;

    public GameObject wolfPrefab;
    public Transform spawnLocation;
    

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 85 && !spawning)
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
                Instantiate(wolfPrefab, spawnLoc, Quaternion.identity);
            }
        }
    }
}
