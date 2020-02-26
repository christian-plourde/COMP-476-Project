using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAOE : MonoBehaviour
{
    public float rateOfExpansion;
    public float rotationMultiplier;

    private void Update()
    {
        RandomRotation();
        UpScale();
    }

    void UpScale()
    {
        Vector3 scale= transform.localScale;
        scale.x += scale.x * rateOfExpansion;

        transform.localScale= new Vector3(scale.x,scale.x,scale.x);

        transform.GetChild(0).GetComponent<Light>().range += transform.GetChild(0).GetComponent<Light>().range * (rateOfExpansion*0.5f);
    }

    void RandomRotation()
    {
        float xSpin = Random.Range(0, 360);
        float ySpin = Random.Range(0, 360);
        float zSpin = Random.Range(0, 360);
 
        transform.rotation = Quaternion.Euler(xSpin*rotationMultiplier, ySpin*rotationMultiplier,zSpin*rotationMultiplier );
    }

    public void SetDeletion(float value)
    {
        Destroy(this.gameObject,value);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            int r = Random.Range(0, 101);
            if (r < 4)
            {
                // hurt
                other.GetComponent<EnemyAttributes>().DealDamage(8);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            
                other.GetComponent<EnemyAttributes>().DealDamage(3);
        }
    }


}
