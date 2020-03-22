using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Tooltip("Sets target, handles attacking and rotating towards the target.\nAttackSpeed is the attack cooldown, in seconds.\nRotationSpeed is how long it takes for the tower to turn 360 degrees, in seconds.\nPossibleTargets can be air, ground, or both.\nProjectileType can be homing, ballistic, or instant.\nHeight limit is the height at which this tower can no longer hit the target ; if left to 0, this tower has no height limitation.\nInaccuracy is a radius within which the projectile may be shot from the target's current position ; only used by ballistic projectiles.")]
    
    public float attackSpeed, damage, range, rotationSpeed, heightLimit, inaccuracy;
    public GameObject projectile;
    public Vector3 projectileSpawnPointOffset;
    public string possibleTargets, projectileType;

    //TODO : make these fields private
    private float attackTimer, attackCooldown, targetScanTimer, rallyingCallMultiplier;
    private GameObject target;
    private bool targetLinedUp = false;
    private Dictionary<string, int> multipliers = new Dictionary<string, int>();
    private Vector3 adjustedRotation = new Vector3(0,0,0);
    private Vector3 adjustedProjectileSpawnPoint;

    public float RallyingCallMultiplier
    {
        get { return rallyingCallMultiplier; }
        set { rallyingCallMultiplier = value; }
    }

    void Start()
    {
        RallyingCallMultiplier = 1.0f;
        attackCooldown = attackSpeed * 60;
        attackTimer = attackCooldown;
        targetScanTimer = 20f;
        if (rotationSpeed != 0) rotationSpeed = 360 / (rotationSpeed*60);
        adjustedProjectileSpawnPoint = projectileSpawnPointOffset;
        multipliers.Add("attackSpeed", 1);
        multipliers.Add("damage", 1);
        multipliers.Add("range", 1);
    }

    void Update()
    {
        attackCooldown = attackSpeed * 60 * rallyingCallMultiplier;

        if (attackTimer < attackCooldown) attackTimer++;
        if (target && validTarget(target) == true)
        {
            if (rotationSpeed > 0)
            {
                Aim();
            }
            if ((rotationSpeed == 0 || targetLinedUp == true) && attackTimer >= attackCooldown)
            {
                attackTimer = 0;
                Attack(target);
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
        GameObject spawnedProjectile = Instantiate(projectile);
        spawnedProjectile.transform.position = transform.position + adjustedProjectileSpawnPoint;
        spawnedProjectile.GetComponent<ProjectileMovement>().Spawn(projectileTarget, damage, projectileType, inaccuracy);
    }

    void Aim()
    {
        adjustedRotation = Vector3.RotateTowards(transform.forward, target.transform.position - transform.position, rotationSpeed * Time.deltaTime, 0);
        adjustedRotation.y = 0; // prevent tower from leaning back if its height is different from its target
        transform.rotation = Quaternion.LookRotation(adjustedRotation);
        adjustedProjectileSpawnPoint = projectileSpawnPointOffset;
        adjustedProjectileSpawnPoint.y = 0; // nullified during magnitude calculation so that only X and Z components are considered
        adjustedProjectileSpawnPoint.x = adjustedProjectileSpawnPoint.magnitude * transform.forward.x;
        adjustedProjectileSpawnPoint.z = adjustedProjectileSpawnPoint.magnitude * transform.forward.z;
        adjustedProjectileSpawnPoint.y = projectileSpawnPointOffset.y;

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
            if (col && validTarget(col.gameObject) == true)
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

    bool validTarget(GameObject potentialTarget){
        if (potentialTarget.tag == "Enemy" && 
            potentialTarget.GetComponent<EnemyAttributes>().GetIsDead() == false &&
            (potentialTarget.transform.position-transform.position).magnitude < range &&
            (potentialTarget.transform.position.y < heightLimit || heightLimit == 0)){
            return true;
        }
        return false;
    }
}
