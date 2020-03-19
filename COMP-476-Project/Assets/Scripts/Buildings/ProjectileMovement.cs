using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [Tooltip("MoveSpeed is units travelled per second.\nImpact radius is the distance from a ballistic projectile's impact at which enemies will take normal damage.\nMovement Type can be homing or ballistic. For projectiles, it is overwritten on spawn, but might be useful if you need this script for something else.")]
    public float moveSpeed, impactRadius;
    public string movementType;
    
    private GameObject target;
    private float movePerFrame;
    private float damage;
  

    // Homing stuff
    private Vector3 aimAtTheirChest;

    // Ballistic stuff
    private Vector3 startPoint, controlPoint, endPoint, curve;
    private float lifeTime, bezierTime;

    // for sound effects
    public string projectileType;

    void Start()
    {
        movePerFrame = moveSpeed / 60;

        // Div by 2.5 for ballistic to give an approximate equivalence so that users can move speed in intuitive units per second and function handles the rest
        if (movementType == "ballistic") movePerFrame /= 2.5f;

        // sounds
        if (projectileType == "Arrow")
        {
            SFXManager.instance.Play("Ballista");
        }
    }

    void Update()
    {
        switch(movementType){
            case "homing":

                // Movement
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position + aimAtTheirChest, movePerFrame);
                transform.rotation = Quaternion.LookRotation(target.transform.position + aimAtTheirChest - transform.position);

                // Behavior when reaching target
                if (target.transform.position.x - transform.position.x < 0.25 && target.transform.position.z - transform.position.z < 0.25)
                {
                    Destroy(gameObject);
                    target.GetComponent<EnemyAttributes>().DealDamage(damage);   
                }

                break;

            case "ballistic":

                // Movement
                bezierTime += Time.deltaTime*moveSpeed;
                curve.x = (((1 - bezierTime) * (1 - bezierTime)) * startPoint.x) + (2 * bezierTime * (1 - bezierTime) * controlPoint.x) + ((bezierTime * bezierTime) * endPoint.x);
                curve.y = (((1 - bezierTime) * (1 - bezierTime)) * startPoint.y) + (2 * bezierTime * (1 - bezierTime) * controlPoint.y) + ((bezierTime * bezierTime) * endPoint.y);
                curve.z = (((1 - bezierTime) * (1 - bezierTime)) * startPoint.z) + (2 * bezierTime * (1 - bezierTime) * controlPoint.z) + ((bezierTime * bezierTime) * endPoint.z);
                transform.position = curve;

                // Behavior when reaching target
                if ((endPoint - transform.position).magnitude < impactRadius)
                {
                    gameObject.GetComponent<ProjectileDeathEffect>().Run();
                    Destroy(gameObject);
                    // scan for enemies to damage
                    Collider[] collidersInRange = Physics.OverlapSphere(endPoint, impactRadius);
                    foreach (Collider col in collidersInRange)
                    {
                        if (col && col.tag == "Enemy" && (col.gameObject.GetComponent<EnemyAttributes>().GetIsDead() == false))
                        {
                            col.gameObject.GetComponent<EnemyAttributes>().DealDamage(damage);
                        }
                    }
                }

                break;
        } 

        
    }

    // Get all needed info from parent
    public void Spawn(GameObject passedTarget, float passedDamage, string passedMovementType, float passedInaccuracy)
    {
        target = passedTarget;
        damage = passedDamage;
        movementType = passedMovementType;

        if (passedMovementType == "homing")
        {
            aimAtTheirChest = new Vector3(0, target.GetComponent<EnemyAttributes>().GetHeightOffset(), 0);
        }
        if (passedMovementType == "ballistic"){
            startPoint = transform.position;
            endPoint = target.transform.position;
            Vector3 inaccuracyRandomizer = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
            inaccuracyRandomizer *= passedInaccuracy;
            endPoint += inaccuracyRandomizer;
            controlPoint = endPoint;
            controlPoint.y = startPoint.y;
        }
    }
}
