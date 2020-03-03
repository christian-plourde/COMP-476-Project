using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickScript : MonoBehaviour
{
    public int dmg=1;
    GameObject Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //check if they are type heavy, then kick wont work.
            other.GetComponent<EnemyAttributes>().DealDamage(dmg);
            Vector3 kickDir = Player.transform.GetChild(0).transform.forward;
            kickDir.y += 0.02f;

            other.GetComponent<Rigidbody>().AddForce(kickDir* 305f, ForceMode.Impulse);
        }
    }
}
