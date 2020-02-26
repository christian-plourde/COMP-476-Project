using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectileMovement : MonoBehaviour
{
    [Tooltip("MoveSpeed is units travelled per second.")]
    public float moveSpeed;
    public GameObject target;

    private float movePerFrame;
    public float damage;
    public Vector3 aimAtTheirChest;
    public GameObject parent;

    void Start()
    {
        movePerFrame = moveSpeed / 60;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position + aimAtTheirChest, movePerFrame);
        transform.rotation = Quaternion.LookRotation(target.transform.position + aimAtTheirChest - transform.position);
        if (target.transform.position.x - transform.position.x < 0.25 && target.transform.position.z - transform.position.z < 0.25)
        {
            Destroy(gameObject);
            if (target.GetComponent<EnemyAttributes>().GetHealth() <= damage)
            {
                parent.GetComponent<TowerAttack>().ClearTarget();
            }
            target.GetComponent<EnemyAttributes>().DealDamage((int)damage);   
        }
    }

    public void Spawn(GameObject passedTarget, float passedDamage, GameObject passedParent)
    {
        target = passedTarget;
        damage = passedDamage;
        parent = passedParent;
        aimAtTheirChest = new Vector3(0, target.GetComponent<EnemyAttributes>().GetHeightOffset(), 0) ;

    }
}
