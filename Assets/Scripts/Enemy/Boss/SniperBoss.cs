using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;
using System.Collections.Generic;

public class SniperBoss : Enemy
{
    public DanmakuPrefab BulletPrefab;
    public DanmakuPrefab CirclePrefab;
    public DanmakuPrefab LaserPrefab;
    public SniperTurretTower turretPrefab;
    private List<SniperTurretTower> turrets;

    private FireBuilder fireDataBullet;
    private FireBuilder fireDataCircle;
    private FireBuilder fireDataLaser;

    private bool stillMovement = false;
    private bool stillRotation = false;
    private Vector3 direction;
    private Vector3 dest;
    private float prepTime = 3f;
    private bool dashing = false;
    private Vector3[] spawnPos = new Vector3[3];
    int spawnCount = 0;
    Wave.EnemyData spawn;
    private bool enraged = false;
    private int enragedDashCounter = 0;
    private Coroutine attackCircular = null;
    private Coroutine attackTargeted = null;

    public override void Start()
    {
        fireDataBullet = new FireBuilder(BulletPrefab, Field);
        fireDataBullet.From(transform);
        fireDataBullet.WithSpeed(3);
        fireDataBullet.WithAngularSpeed(45);
        fireDataBullet.WithModifier(new CircularBurstModifier(340, new DynamicInt(8, 12), 0, 0));
        fireDataBullet.WithController(new AccelerationController(3));

        fireDataCircle = new FireBuilder(CirclePrefab, Field);
        fireDataCircle.From(transform);
        fireDataCircle.WithSpeed(12);
        fireDataCircle.WithModifier(new CircularBurstModifier(45, 1, 0, 0));

        fireDataLaser = new FireBuilder(LaserPrefab, Field);
        fireDataLaser.From(transform);
        fireDataLaser.WithSpeed(0);
        fireDataLaser.WithRotation(transform);
        fireDataLaser.WithController(new AutoDeactivateController(0.25f));

        direction = Vector3.zero - transform.position;
        dest = Vector3.zero;
        turrets = new List<SniperTurretTower>();
        SetRotation(180);
        Dash();        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (dashing) // Start moving after its pause time has ended
        {
            prepTime -= Time.fixedDeltaTime;
            if (prepTime <= 0)
            {
                stillMovement = false;
                stillRotation = false;
                dashing = false;
            }
        }
        if (!stillMovement && !stillRotation)
        {
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * (enraged ? 40: 20);
            if (spawnCount < 3 && Vector3.Distance(transform.position, spawnPos[spawnCount]) < 1)
            {
                // Spawn a turret when the boss reaches a turret spawn location
                SpawnTurret(turretPrefab);
                spawnCount++;
            }
            if (Vector3.Distance(transform.position, dest) < 1)
            {
                stillMovement = true;
                dashing = false;
                if (enragedDashCounter > 0)
                {
                    Dash();
                    enragedDashCounter--;
                }
                else
                {
                    attackTargeted = StartCoroutine(AttackTargeted());
                    attackCircular = StartCoroutine(AttackCircular());
                }
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public override void Damage(int damage)
    {                       
        if (Health == MaxHealth / 2)
        {
            enraged = true;
            enragedDashCounter = 2 + Difficulty;
            Dash();
        }
        base.Damage(damage);
    }

    protected void SpawnTurret(Enemy turretPrefab)
    {
        spawn.Prefab = turretPrefab;
        spawn.Data.Time = 0;
        spawn.Data.Location = (Vector2)transform.position;
        turrets.Add((SniperTurretTower)Wave.SpawnEnemy(spawn));
        turrets[turrets.Count - 1].sBoss = this;
    }

    public void UnregisterTower(SniperTurretTower turret)
    {
        turrets.Remove(turret);
        if (turrets.Count == 0)
        {
            Dash();
        }
    }

    protected IEnumerator AttackTargeted()
    {
        while (true)
        {
            Vector3 position;
            int direction;
            for (int i = 0; i < 3; i++)
            {
                FacePlayer = false;
                SetRotation(180);
                position = Player.transform.position;
                direction = Random.Range(0, 2) * 2 - 1; // -1 or 1
                for (int j = 0; j < 16; j++)
                {
                    if (enraged)
                    {
                        fireDataCircle.Towards(position);
                        fireDataCircle.WithRotation((12f * Mathf.Sqrt((16 - j) * 24) - 80) * direction);
                        fireDataCircle.Fire();
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.5f);
            }

            for (int i = 0; i < (enraged ? 5 : 3); i++)
            {
                stillRotation = false;
                FacePlayer = true;
                yield return new WaitForSeconds(0.5f);

                stillRotation = true;
                FacePlayer = false;
                TargetRotation = transform.rotation;
                fireDataLaser.Fire();
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    protected IEnumerator AttackCircular()
    {
        while (true)
        {
            fireDataBullet.Fire();
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected void Dash()
    {
        System.Random rnd = new System.Random(); // Generate a random new position
        int newX = rnd.Next(-1, 2) * 12;
        int newY = rnd.Next(-1, 2) * 6;
        dest = new Vector3(newX, newY);
        while (Vector3.Distance(transform.position, dest) < 13) // Makes sure it dashes far away
        {
            newX = rnd.Next(-1, 2) * 12;
            newY = rnd.Next(-1, 2) * 6;
            dest = new Vector3(newX, newY);
        }
        direction = dest - transform.position;

        // Set spawn positions of turrets
        spawnPos[0] = transform.position;
        spawnPos[1] = new Vector3((dest.x - transform.position.x) / 3 + transform.position.x, (dest.y - transform.position.y) / 3 + transform.position.y);
        spawnPos[2] = new Vector3((dest.x - transform.position.x) * 2 / 3 + transform.position.x, (dest.y - transform.position.y) * 2 / 3 + transform.position.y);
        spawnCount = 0;

        if(attackCircular != null)
            StopCoroutine(attackCircular);
        if(attackTargeted != null)
            StopCoroutine(attackTargeted);

        prepTime = 2.5f; // Pause before dashing
        FacePlayer = false;
        SetRotation(direction);
        dashing = true;
    }

    public override void Die()
    {
        while(turrets.Count > 0)
        {
            turrets[0].Die();
        }
        base.Die();
    }
}