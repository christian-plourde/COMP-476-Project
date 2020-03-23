using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDMG : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * 0.2f * Time.deltaTime);
    }

    public void SetDMG(float value, bool critical)
    {
        transform.GetChild(0).GetComponent<Text>().text = ""+value;
        if (critical)
        {
            transform.GetChild(0).GetComponent<Text>().color = Color.red;
        }
    }
}
