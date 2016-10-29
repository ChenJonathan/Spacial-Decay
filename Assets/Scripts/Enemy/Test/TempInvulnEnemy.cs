using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class TempInvulnEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;
    public bool fireTowardsPlayer;

    private int invuln = 0;
    private float invulnTime = 3f;
    private Vector2 direction = new Vector2(0,0);

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        if (fireTowardsPlayer)
        {
            fireData.Towards(Player.transform);
        }
        fireData.WithSpeed(4 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(10 * Difficulty, 1 + Difficulty, 0, 0));
        invulnTime += Difficulty;
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            if(!fireTowardsPlayer)
            {
                fireData.Towards(direction);
            }
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(invuln == 1)
        {
            // While invulnerable, repeatedly fade sprite
            invulnTime -= Time.fixedDeltaTime;
            Renderer renderer = GetComponent<Renderer>();
            Color color = renderer.material.color;
            if(invulnTime % 0.05f > (invulnTime + Time.fixedDeltaTime) % 0.05f)
            {
                color.a = 1.01f - color.a;
                renderer.material.color = color;
            }
            else
            {
                color.a = 1;
                renderer.material.color = color;
            }
        }
        if(invulnTime <= 0)
        {
            invuln = 2;
        }

        if((Mathf.Abs(transform.position.x) > 18) || (Mathf.Abs(transform.position.y) > 9))
        {
            direction = -1 * transform.position;
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
        SetRotation(direction);
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