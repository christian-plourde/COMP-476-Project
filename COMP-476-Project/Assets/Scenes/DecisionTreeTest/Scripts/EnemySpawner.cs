using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

/// <summary>
/// This represents a deserialized json object containing info about one enemy's level
/// </summary>
[System.Serializable]
public class EnemyLevel
{
    public string name;
    public int level;
}

/// <summary>
/// Contains a list of all enemies levels.
/// </summary>
[System.Serializable]
public class EnemyLevelInfo
{
    public EnemyLevel[] enemies;

    //this function returns a random enemy name with the level passed as param from list of enemies
    public string GetRandomEnemy(int level)
    {
        //first pull out all the enemies from our list that match the right level
        List<EnemyLevel> es = new List<EnemyLevel>();

        foreach(EnemyLevel e in enemies)
        {
            if(e.level == level)
            {
                es.Add(e);
            }
        }

        //now we need to pick a random one from this list

        return es[UnityEngine.Random.Range(0, es.Count)].name;
    }
}

/// <summary>
/// Represents info about what should be spawned in one wave. Extracted from JSON.
/// </summary>
[System.Serializable]
public class Wave
{
    public int[] enemies; //the levels of all the enemies in the wave (in order of spawning)
    private int current_index = 0; //the current index we are at in terms of spawning enemies
    
    public int CurrentEnemyLevel
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

/// <summary>
/// Extracted from JSON. Contains the list of all waves for the game.
/// </summary>
[System.Serializable]
public class WaveManager
{
    public Wave[] waves;
    private int currentWaveIndex = 0;

    public int CurrentWaveIndex
    {
        get { return currentWaveIndex; }
    }

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
    public TextAsset EnemyLevelFile; //the json file containing all enemy level info
    private WaveManager waveManager; //this contains an array of waves, each wave contains a list of enemies
    private EnemyLevelInfo enemy_level_info; //this contains info about each enemy and its level
    public Text wave_ui_text;

    private GameObject GetCurrentEnemy()
    {
        //to load the current enemy, we first need to get a random enemy of appropriate level from the
        //enemy_level_info
        string curr_en_name = enemy_level_info.GetRandomEnemy(waveManager.CurrentWave.CurrentEnemyLevel);

        //then we need to find the matching enemy from the prefabs list
        foreach(GameObject o in enemies)
        {
            if(o.name == curr_en_name)
            {
                return o;
            }
        }

        return null;
    }

    void SetWaveUIText()
    {
        wave_ui_text.text = (waveManager.CurrentWaveIndex + 1).ToString() + "/" + waveManager.waves.Length;
    }

    void Start()
    {
        //load all of the waves from the json file
        waveManager = JsonUtility.FromJson<WaveManager>(WaveConfigurationFile.text);

        //load all info about enemies levels
        enemy_level_info = JsonUtility.FromJson<EnemyLevelInfo>(EnemyLevelFile.text);

        //load all the enemies from our folder
        enemies = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        SetWaveUIText();

        InvokeRepeating("InitializeEnemies", timeBetweenWaves, spawnerInterval);
    }

    void InitializeEnemies()
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
                SetWaveUIText();
                CancelInvoke("InitializeEnemies");
                InvokeRepeating("InitializeEnemies", timeBetweenWaves, spawnerInterval);
            }
            
        }

        catch 
        {
            CancelInvoke("InitializeEnemies");
            //Debug.Log("The game has ended.");
        }
        
    }
}
