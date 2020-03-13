using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateAreaController : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject PlayerRef;
    Vector3 velocity = Vector3.zero;

    bool targetSet;

    public Material targetSetMat;

    public float MovementSmoothness = 0.3f;
    public GameObject arrowPrefab;

    float takeDamageTimer = 0f;
    float timer=0f;
    float howOftenArrows=0.1f;

    float scaleFactor;

    void Start()
    {
        // on instantiate, enable control lock
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerRef.GetComponent<PlayerMovement>().controlLock = true;

        scaleFactor = PlayerRef.transform.localScale.x;
        //transform.localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetSet)
        {
            UltimateAreaControls();
            ActivationControls();
        }
        else
        {
            RainFireArrows();
        }

        if (PlayerRef.GetComponent<PlayerMovement>().isDead && !targetSet)
        {
            Destroy(this.gameObject);
        }
    }

    void UltimateAreaControls()
    {
        float mouseX = Input.GetAxis("Mouse X")*2;
        float mouseY = Input.GetAxis("Mouse Y")*2;

        Vector3 pos = new Vector3(transform.position.x+mouseX, 0 , transform.position.z + mouseY);
        //transform.position = pos;
        if(Vector3.Distance(pos,PlayerRef.transform.position)<35)
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, MovementSmoothness);
    }

    void ActivationControls()
    {
        if (Input.GetMouseButton(0))
        {
            targetSet = true;
            PlayerRef.GetComponent<PlayerMovement>().controlLock = false;
            GetComponent<Renderer>().material = targetSetMat;
            GameObject.FindGameObjectWithTag("Player").GetComponent<CombatBehavior>().ultimateCooldown = true;
            SFXManager.instance.Play("ArcherUltimate");
            Destroy(this.gameObject, 13f);
        }
        if (Input.GetMouseButtonDown(1))
        {
            PlayerRef.GetComponent<PlayerMovement>().controlLock = false;
            Destroy(this.gameObject);
        }
    }

    void RainFireArrows()
    {
        Vector3 pos = transform.position;
        Vector3 spawnPos = pos;
        spawnPos.y += 30;

        float randomXoffset;
        float randomZoffset;

        randomXoffset = Random.Range(-3f, 3f);
        randomZoffset = Random.Range(-3f, 3f);

        timer += Time.deltaTime;

        spawnPos.x += randomXoffset;
        spawnPos.z += randomZoffset;

        if (timer > howOftenArrows && takeDamageTimer<11)
        {
            timer = 0;
            GameObject gb= Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
            gb.GetComponent<Arrow>().SetArrowDamage(20);
            gb.transform.Rotate(new Vector3(90, 0, 0));
            gb.GetComponent<Rigidbody>().AddForce(Vector3.down*7.5f,ForceMode.Impulse);
        }

        takeDamageTimer += Time.deltaTime;
     
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy" && targetSet && takeDamageTimer>1.9f)
        {
            int r = Random.Range(0, 101);
            if (r < 4)
            {
                // hurt
                other.GetComponent<EnemyAttributes>().DealDamage(2);
            }
        }
        if (other.tag == "Player" && targetSet && takeDamageTimer > 1.9f)
        {
            int r = Random.Range(0, 101);
            if (r < 4)
            {
                // hurt
                other.GetComponent<PlayerMovement>().DealDamage(5);
            }
        }
    }
}
