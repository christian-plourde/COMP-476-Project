using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Wave
{
    public string[] enemies; //the names of all the enemies in the wave (in order of spawning)
    private int current_index = 0; //the current index we are at in terms of spawning enemies
    
    public string CurrentEnemyName
    {
        get { return enemies[current_index]; }
    }

    public void IncrementIndex()
    {
        current_index++;
        if (current_index >= enemies.Length)
            throw new Exception();
    }
}

[System.Serializable]
public class WaveManager
{
    public Wave[] waves;
    private int currentWaveIndex = 0;

    public Wave CurrentWave
    {
        get { return waves[currentWaveIndex]; }
    }

    public void IncrementWaveIndex()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length)
            throw new Exception();
    }
}

public class EnemySpawner : MonoBehaviour
{
    public int timeBetweenWaves = 2;
    public int spawnerInterval = 10;
    private GameObject[] enemies;
    public TextAsset WaveConfigurationFile; //the json file containing information about the waves
    private WaveManager waveManager; //this contains an array of waves, each wave contains a list of enemies

    private GameObject GetCurrentEnemy()
    {
        foreach(GameObject o in enemies)
        {
            if(o.name == waveManager.CurrentWave.CurrentEnemyName)
            {
                return o;
            }
        }

        return null;
    }

    void Start()
    {
        //load all of the waves from the json file
        waveManager = JsonUtility.FromJson<WaveManager>(WaveConfigurationFile.text);

        //load all the enemies from our folder
        enemies = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        InvokeRepeating("InitializeZombies", timeBetweenWaves, spawnerInterval);
    }

    void InitializeZombies()
    {
        try
        {
            GameObject new_enemy = GameObject.Instantiate(GetCurrentEnemy()); //load random enemy
            ZombieBehaviour zm = new_enemy.GetComponent<ZombieBehaviour>();
            zm.Initialize();
            try
            {
                waveManager.CurrentWave.IncrementIndex();
            }

            catch
            {
                waveManager.IncrementWaveIndex();
                CancelInvoke("InitializeZombies");
                InvokeRepeating("InitializeZombies", timeBetweenWaves, spawnerInterval);
            }
            
        }

        catch 
        {
            CancelInvoke("InitializeZombies");
            Debug.Log("The game has ended.");
        }
        
    }
}
