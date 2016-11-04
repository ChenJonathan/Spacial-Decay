using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class TempInvulnEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 2f;
    public bool fireTowardsPlayer;

    private int invuln = 0; // 0 -> not invulnerable, 1 -> invulnerable period, 2 -> invuln duration ended, cannot invuln again
    private float invulnTime = 3f;
    private Vector2 direction = new Vector2(0, 0);

    public override void Start()
    {
        // Spawn outside of X bounds if you want horizontal movement and outside of Y bounds if you want vertical movement
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        if (fireTowardsPlayer)
        {
            fireData.Towards(Player.transform);
        }
        fireData.WithSpeed(6 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(10 * Difficulty, 3 + Difficulty, 0, 0));
        invulnTime += Difficulty;
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            if(!fireTowardsPlayer)
            {
                fireData.Towards(direction + (Vector2)transform.position);
            }
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN - Difficulty / 2;
        }
    }

    public override void FixedUpdate()
    {
        if(invuln == 1)
        {
            // While invulnerable, repeatedly fade sprite
            invulnTime -= Time.fixedDeltaTime;
            Renderer renderer = GetComponent<Renderer>();
            Color color = renderer.material.color;
            if(invulnTime % 0.05f > (invulnTime + Time.fixedDeltaTime) % 0.05f)
            {
                color.a = 1.25f - color.a;
                renderer.material.color = color;
            }

            if(invulnTime <= 0)
            {
                color.a = 1;
                renderer.material.color = color;
                invuln = 2;
            }
        }

        if(Mathf.Abs(transform.position.x) > 18)
        {
            direction = new Vector2(-1 * transform.position.x, transform.position.y) - (Vector2)transform.position;
            SetRotation(direction);
        }
        else if(Mathf.Abs(transform.position.y) > 9)
        {
            direction = new Vector2(transform.position.x, -1 * transform.position.y) - (Vector2)transform.position;
            SetRotation(direction);
        }

        int velMult = 1; // Increase velocity if invulnerable
        if(invuln == 1)
        {
            velMult = 2;
        }
        else
        {
            velMult = 1;
        }
        GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * (3 + Difficulty) * velMult;
    }

    public override void Damage(int damage)
    {            
        // Set invulnerable if health falls below half of max health
        if (invuln == 1)
        {
            damage = 0;
        }
        base.Damage(damage);
        if (invuln == 0 && Health <= MaxHealth / 2)
        {
            invuln = 1;
        }
    }
}