using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastardSword : MonoBehaviour
{
    [Range(0, 100)]
    public int criticalChance;
    public int baseDMG=4;

    public GameObject popUpDMG_Prefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            // roll critical
            int dmg = baseDMG;
            int r = Random.Range(0, 100);
            bool crit=false;
            if (r < criticalChance)
            {
                dmg += baseDMG * criticalChance;
                crit = true;
            }

            other.GetComponent<EnemyAttributes>().DealDamage(dmg);
            GameObject gb=Instantiate(popUpDMG_Prefab,transform.position, Quaternion.identity);
            //gb.transform.position = transform.position;
            //gb.GetComponent<RectTransform>().position = transform.position;

            gb.GetComponent<PopupDMG>().SetDMG(dmg,crit);
        }
    }
}
