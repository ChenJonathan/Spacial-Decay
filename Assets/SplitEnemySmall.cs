﻿using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class SplitEnemySmall : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = .4f;
    private static readonly float MAX_BURST_COOLDOWN = 2f;
    private int burstNum = 4;
    private float burstCooldown = MAX_BURST_COOLDOWN;
    public bool fireTowardsPlayer;

    private Vector2 direction;
    private bool startup = true;
    private bool invuln = true;
    private float invulnTime = 2f;    

    public override void Start()
    {
        direction = new Vector2(Random.Range(-20f, 20f), Random.Range(-10f, 10f)) - (Vector2)transform.position;
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        if (fireTowardsPlayer)
        {
            fireData.Towards(Player.transform);
        }
        fireData.WithSpeed(9 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(45, 1, 0, 0));
        burstNum += Difficulty;
    }

    public override void Update()
    {
        base.Update();

        if (!LevelController.Singleton.Paused)
        {
            burstCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
                if (!fireTowardsPlayer)
                {
                    fireData.Towards(direction + (Vector2)transform.position);
                }
                fireData.Fire();
                fireCooldown = MAX_FIRE_COOLDOWN;
                burstNum--;
                if (burstNum == 0)
                {
                    burstCooldown = MAX_BURST_COOLDOWN + MAX_FIRE_COOLDOWN;
                    burstNum = 4 + Difficulty;
                }
            }
            if (burstCooldown <= 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!LevelController.Singleton.Paused)
        {
            if (invuln)
            {
                // While invulnerable, repeatedly fade sprite
                invulnTime -= Time.fixedDeltaTime;
                Renderer renderer = GetComponent<Renderer>();
                Color color = renderer.material.color;
                if (invulnTime % 0.05f > (invulnTime + Time.fixedDeltaTime) % 0.05f)
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

            if (invulnTime <= 0)
            {
                invuln = false;
            }

            if (startup)
            {
                // direction = direction - (Vector2)transform.position;
                SetRotation(direction);
                if (Mathf.Abs(transform.position.x) < 18 && Mathf.Abs(transform.position.y) < 9)
                {
                    startup = false;
                }
            }
            else
            {
                float x = transform.position.x;
                float y = transform.position.y;
                if (Mathf.Abs(x) > 18)
                {
                    direction = new Vector2(GetComponent<Rigidbody2D>().velocity.x * -1, GetComponent<Rigidbody2D>().velocity.y);
                }
                else if (Mathf.Abs(y) > 9)
                {
                    direction = new Vector2(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y * -1);
                }
                else
                {
                    SetRotation(direction);
                }
            }
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * (6 + Difficulty);
        }
    }    

    public override void Damage(int damage)
    {
        if (invuln)
        {
            damage = 0;
        }
        base.Damage(damage);
    }
}