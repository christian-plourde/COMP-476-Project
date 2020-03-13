using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWolf : MonoBehaviour
{
    float timer;
    public float speed=11f;

    void Start()
    {
        GetComponent<Animator>().SetBool("Chasing",true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 11)
            Destroy(this.gameObject);
        transform.Translate(transform.forward*speed*Time.deltaTime);   

    }
}
