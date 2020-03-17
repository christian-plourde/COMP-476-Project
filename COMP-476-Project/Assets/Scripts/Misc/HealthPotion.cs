using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{

    public int healthGain=25;
    public float moveSpeed=5f;
    public float pickupRadius=1f;
    public float DeletionTimer=30f;

    private void Start()
    {
        Destroy(this.gameObject,DeletionTimer);
    }



    void HealPlayer(Transform playerRef)
    {
        playerRef.GetComponent<PlayerMovement>().HealPlayer(healthGain);
        SFXManager.instance.Play("HealPlayer");
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !other.GetComponent<PlayerMovement>().isDead && other.GetComponent<PlayerMovement>().health< other.GetComponent<PlayerMovement>().maxHealth)
        {
            Vector3 moveTo = (other.transform.position - transform.position).normalized;
            moveTo.y -= 0.3f;
            transform.Translate(moveTo*moveSpeed*Time.deltaTime);

            if (Vector3.Distance(transform.position, other.transform.position) < pickupRadius)
            {
                HealPlayer(other.transform);
            }

            moveSpeed += 0.02f;
        }
        
    }
}
