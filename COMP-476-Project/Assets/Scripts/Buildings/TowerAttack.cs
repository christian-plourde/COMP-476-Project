using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Tooltip("Sets target, handles attacking and rotating towards the target.\nAttackSpeed is the attack cooldown, in seconds.\nRotationSpeed is how long it takes for the tower to turn 360 degrees, in seconds.\nPossibleTargets can be air, ground, or both.")]
    public float attackSpeed, damage, range, rotationSpeed;
    public GameObject projectile;
    public Vector3 projectileSpawnPointOffset;
    public string possibleTargets;

    public float attackTimer, attackCooldown;
    public GameObject target;
    public bool targetLinedUp = false;
    public int targetScanTimer = 20;
    public Dictionary<string, int> multipliers = new Dictionary<string, int>();
    public Vector3 adjustedRotation = new Vector3(0,0,0);
    public Vector3 adjustedProjectileSpawnPoint;

    void Start()
    {
        attackCooldown = attackSpeed * 60;
        attackTimer = attackCooldown;
        if (rotationSpeed != 0) rotationSpeed = 360 / (rotationSpeed*60);
        multipliers.Add("attackSpeed", 1);
        multipliers.Add("damage", 1);
        multipliers.Add("range", 1);
    }

    void Update()
    {
        if (attackTimer < attackCooldown) attackTimer ++;
        if (target && (target.transform.position - transform.position).magnitude < range && target.GetComponent<EnemyAttributes>().GetIsDead() == false)
        {
            if (rotationSpeed > 0)
            {
                Aim();
            }
            if ((rotationSpeed == 0 || targetLinedUp == true) && attackTimer >= attackCooldown)
            {
                Attack(target);
                attackTimer = 0;
            }
        } else
        {
            targetScanTimer++;
            if (targetScanTimer >= 20)
            {
                targetScanTimer = 0;
                SeekTarget();
            }
        }
    }

    void Attack(GameObject projectileTarget)
    {
        Debug.Log(transform.forward);

        GameObject spawnedProjectile = Instantiate(projectile);
        spawnedProjectile.transform.position = transform.position + adjustedProjectileSpawnPoint;
        spawnedProjectile.GetComponent<HomingProjectileMovement>().Spawn(projectileTarget, damage, gameObject);
    }

    void Aim()
    {
        adjustedRotation = Vector3.RotateTowards(transform.forward, target.transform.position - transform.position, rotationSpeed * Time.deltaTime, 0);
        adjustedRotation.y = 0;
        transform.rotation = Quaternion.LookRotation(adjustedRotation);
        Debug.Log(transform.forward);
        //Debug.Log(adjustedRotation);
        adjustedProjectileSpawnPoint.x = projectileSpawnPointOffset.x * transform.forward.x; // TODO fix
        adjustedProjectileSpawnPoint.z = projectileSpawnPointOffset.z * transform.rotation.z;
        if (Vector3.Angle(transform.forward, target.transform.position-transform.position) < 10f)
        {
            targetLinedUp = true;
        } else
        {
            targetLinedUp = false;
        }
    }

    void SeekTarget()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, range);
        foreach (Collider col in collidersInRange)
        {
            if (col && col.tag == "Enemy" && (col.gameObject.GetComponent<EnemyAttributes>().GetIsDead() == false))
            {
                target = col.gameObject;
                break;
            }
        }
    }

    public void ClearTarget()
    {
        target = null;
    }
}
