using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffShowcaseScript : MonoBehaviour
{
    public List<Sprite> buffIconList;
    public List<Sprite> gBuffIconList;
    public GameObject iconPrefab;

    PlayerBuffManager bManager;
    List<Buff> bList = new List<Buff>();

    // Start is called before the first frame update
    void Start()
    {
        bManager = GameObject.FindObjectOfType<PlayerBuffManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToList(int uid)
    {
        bool isIn = false;
        foreach(Buff b in bList)
        {
            if (b.uid == uid)
            {
                isIn = true;
                break;
            }
        }

        if (!isIn)
        {
            Buff temp = bManager.Buffs[0];
            foreach(Buff b in bManager.Buffs)
            {
                if(b.uid == uid)
                {
                    temp = b;
                    break;
                }
            }

            Vector3 pos = new Vector3(iconPrefab.transform.position.x + (bList.Count * 30), iconPrefab.transform.position.y, iconPrefab.transform.position.z);
            GameObject gb = Instantiate(iconPrefab, pos, Quaternion.identity);
            gb.transform.SetParent(this.gameObject.transform, false);
            gb.GetComponent<BuffIconScript>().Icon = buffIconList[uid];
            gb.GetComponent<BuffIconScript>().GIcon = gBuffIconList[uid];
            gb.GetComponent<BuffIconScript>().Buff = temp;

            bList.Add(temp);
        }
    }
}
