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
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0 && !invincible)
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

        GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * (3 + Difficulty) * (invincible ? 2 + Difficulty : 1);
    }
}