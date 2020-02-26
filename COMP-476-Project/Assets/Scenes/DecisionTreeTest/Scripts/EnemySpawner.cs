using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject m_ZombiePrefab;
    public GameObject m_PlayerObj;
    public GameObject m_TowerObj;

    private List<GameObject> m_Zombies = new List<GameObject>();

    public int initial_monster_count = 2;

    void InitializeZombies()
    {
        foreach(GameObject g in m_Zombies)
        {
            ZombieMovement zm = g.GetComponent<ZombieMovement>();
            zm.Initialize(this.m_PlayerObj.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < initial_monster_count; i++)
            this.m_Zombies.Add(GameObject.Instantiate(this.m_ZombiePrefab));
        //this.m_Zombies[0].transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        InitializeZombies();
    }

}
