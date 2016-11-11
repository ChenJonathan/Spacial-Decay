﻿using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class RapidFireScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = 3f;
    private float timer = 2f;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new RandomizeAngleModifier(360));
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown < 0)
        {
            fireData.Fire();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer -= Time.deltaTime;
        if(timer > 0)
        {
            Vector3 direction = new Vector3(-1.0f, 0.0f);
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 10;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(0.0f, 0.0f);
        }
    }
}