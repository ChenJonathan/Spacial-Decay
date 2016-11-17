using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class SplitEnemyBig : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = .4f;
    private static readonly float MAX_BURST_COOLDOWN = 4f;
    private int burstNum = 5;
    private float burstCooldown = MAX_BURST_COOLDOWN;
    public bool fireTowardsPlayer;
    
    private Vector2 direction;
    private bool startup = true;    
    public Enemy enemyPrefab;    

    public override void Start()
    {        
        direction = new Vector2(Random.Range(-20f, 20f), Random.Range(-10f, 10f)) - (Vector2)transform.position;
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        if (fireTowardsPlayer)
        {
            fireData.Towards(Player.transform);
        }
        fireData.WithSpeed(4 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(45, 8 + Difficulty, 0, 0));
        burstNum += Difficulty;      
    }

    public void Update()
    {
        burstCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            if(!fireTowardsPlayer)
            {
                fireData.Towards(direction + (Vector2)transform.position);
            }
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN;
            burstNum--;
            if(burstNum == 0)
            {
                burstCooldown = MAX_BURST_COOLDOWN + MAX_FIRE_COOLDOWN;
                burstNum = 5 + Difficulty;
            }
        }
        if(burstCooldown <= 0)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(startup)
        {
            SetRotation(direction);
            if(Mathf.Abs(transform.position.x) < 18 && Mathf.Abs(transform.position.y) < 9)
            {
                startup = false;
            }
        }
        else
        {
            float x = transform.position.x;
            float y = transform.position.y;
            if(Mathf.Abs(x) > 18)
            {
                direction = new Vector2(GetComponent<Rigidbody2D>().velocity.x * -1, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if(Mathf.Abs(y) > 9)
            {
                direction = new Vector2(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y * -1);
            }
            else
            {
                SetRotation(direction);
            }
        }
        GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * (3 + Difficulty);
    }

    public override void Die()
    {
        Wave.EnemyData spawn;
        spawn.Prefab = enemyPrefab;
        spawn.Data.Location = transform.position;
        spawn.Data.Time = 0;
        spawn.Data.Parameters = new float[0];
        Enemy temp = Wave.SpawnEnemy(spawn);
        temp.StartCoroutine(temp.SetInvincible(INVINCIBILITY_ON_HIT));
        temp = Wave.SpawnEnemy(spawn);
        temp.StartCoroutine(temp.SetInvincible(INVINCIBILITY_ON_HIT));
        
        base.Die();
    }
}