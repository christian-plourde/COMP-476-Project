using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public int initial_start_delay_seconds = 2;
    public int spawner_interval_seconds = 10;
    GameObject[] enemies;

    void Start()
    {
        //load all the enemies from our folder
        enemies = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        InvokeRepeating("InitializeZombies", initial_start_delay_seconds, spawner_interval_seconds);
    }

    void InitializeZombies()
    {
        int enemy_index = Random.Range(0, enemies.Length); //generate random index for enemy spawn
        GameObject new_enemy = GameObject.Instantiate(enemies[enemy_index]); //load random enemy
        ZombieBehaviour zm = new_enemy.GetComponent<ZombieBehaviour>();
        zm.Initialize();
    }
}
