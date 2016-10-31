using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;
using System.Collections.Generic;

public class BossEnemy : Enemy
{
    public DanmakuPrefab BulletPrefab;
    public DanmakuPrefab CirclePrefab;
    public DanmakuPrefab LaserPrefab;
    public Tower TowerPrefab;

    private FireBuilder fireDataBullet;
    private FireBuilder fireDataCircle;
    private FireBuilder fireDataLaser;

    private List<Tower> towers;

    private bool stillMovement = false;
    private bool stillRotation = false;
    private bool enraged = false;

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
        fireDataCircle.WithModifier(new CircularBurstModifier(45, 7, 0, 0));

        fireDataLaser = new FireBuilder(LaserPrefab, Field);
        fireDataLaser.From(transform);
        fireDataLaser.WithSpeed(0);
        fireDataLaser.WithRotation(transform);
        fireDataLaser.WithController(new AutoDeactivateController(0.25f));

        towers = new List<Tower>();

        SetRotation(180);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(!stillMovement && !stillRotation)
        {
            Vector3 direction = Vector3.zero - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;

            if(Vector3.Distance(transform.position, Vector3.zero) < 0.1)
            {
                stillMovement = true;

                // Spawn towers
                Wave.EnemyData spawn;
                spawn.Prefab = TowerPrefab;
                spawn.Data.Time = 0;
                spawn.Data.Location = new Vector2(-5, 0);
                towers.Add((Tower)Wave.SpawnEnemy(spawn));
                spawn.Data.Location = new Vector2(5, 0);
                towers.Add((Tower)Wave.SpawnEnemy(spawn));
                spawn.Data.Location = new Vector2(0, -5);
                towers.Add((Tower)Wave.SpawnEnemy(spawn));
                spawn.Data.Location = new Vector2(0, 5);
                towers.Add((Tower)Wave.SpawnEnemy(spawn));

                foreach(Tower tower in towers)
                {
                    tower.Boss = this;
                }

                StartCoroutine(Attack());
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
    
    public override void Damage(int damage)
    {
        if(enraged)
            base.Damage(damage);
    }

    public void UnregisterTower(Tower tower)
    {
        towers.Remove(tower);
        if(towers.Count == 0)
        {
            fireDataCircle.WithSpeed(16);
            RotateSpeed = 16;
            enraged = true;

            StartCoroutine(EnragedAttack());
        }
    }

    protected IEnumerator Attack()
    {
        while(true)
        {
            Vector3 position;
            int direction;
            for(int i = 0; i < 3; i++)
            {
                FacePlayer = false;
                SetRotation(180);
                position = Player.transform.position;
                direction = Random.Range(0, 2) * 2 - 1; // -1 or 1
                for(int j = 0; j < 16; j++)
                {
                    fireDataCircle.Towards(position);
                    fireDataCircle.WithRotation((12f * Mathf.Sqrt((16 - j) * (enraged ? 72 : 24)) - 80) * direction);
                    fireDataCircle.Fire();
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.5f);
            }

            if(!enraged)
                yield return new WaitForSeconds(1);

            for(int i = 0; i < (enraged ? 5 : 3); i++)
            {
                stillRotation = false;
                FacePlayer = true;
                yield return new WaitForSeconds((enraged ? 0.25f : 0.5f));

                stillRotation = true;
                FacePlayer = false;
                TargetRotation = transform.rotation;
                fireDataLaser.Fire();
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    protected IEnumerator EnragedAttack()
    {
        while(true)
        {
            fireDataBullet.Fire();
            yield return new WaitForSeconds(0.5f);
        }
    }
}