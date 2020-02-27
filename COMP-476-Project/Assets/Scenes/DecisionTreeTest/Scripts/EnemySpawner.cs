using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject m_ZombiePrefab;

    private List<GameObject> m_Zombies = new List<GameObject>();

    public int initial_monster_count = 2;
    public int initial_start_delay_seconds = 2;
    public int spawner_interval_seconds = 10;

    void InitializeZombies()
    {
        m_Zombies.Clear();

        for (int i = 0; i < initial_monster_count; i++)
        {
            this.m_Zombies.Add(GameObject.Instantiate(this.m_ZombiePrefab));
        }

        foreach (GameObject g in m_Zombies)
        {
            ZombieMovement zm = g.GetComponent<ZombieMovement>();
            zm.Initialize();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("InitializeZombies", initial_start_delay_seconds, spawner_interval_seconds);
    }

}
