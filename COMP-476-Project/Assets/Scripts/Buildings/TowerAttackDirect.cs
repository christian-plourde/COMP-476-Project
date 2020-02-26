using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackDirect : MonoBehaviour
{
    [Tooltip("AttackSpeed is the attack cooldown, in seconds.\nRotationSpeed is how long it takes for the tower to turn 360 degrees, in seconds.")]
    public float attackSpeed, damage, range, rotationSpeed;
    public GameObject projectile;
    public Vector3 projectileSpawnPoint;

    public float attackTimer, attackCooldown;
    public GameObject target;
    public bool targetLinedUp = false;
    public int targetScanTimer = 20;

    void Start()
    {
        attackCooldown = attackSpeed * 60;
        attackTimer = attackCooldown;
        if (rotationSpeed != 0) rotationSpeed = 360 / rotationSpeed;
    }

    void Update()
    {
        if (attackTimer < attackCooldown) attackTimer ++;
        if (target && (target.transform.position - transform.position).magnitude < range)
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
        GameObject arrow = Instantiate(projectile);
        arrow.transform.position = transform.position + projectileSpawnPoint;
        arrow.GetComponent<HomingProjectileMovement>().Spawn(projectileTarget, damage, gameObject);
    }

    void Aim()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, rotationSpeed * Time.deltaTime);
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
