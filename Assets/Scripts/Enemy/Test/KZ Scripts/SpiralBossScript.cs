﻿using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class SpiralBossScript : Enemy
{
    public DanmakuPrefab crossPrefab;
    public DanmakuPrefab circlePrefab;

    private FireBuilder fireDataCross1;
    private FireBuilder fireDataCross2;
    private FireBuilder fireDataCircle;
    private bool still = false;
    private bool enraged = false;
    private bool weaponized = false;
    private bool finalForm = false;
    private int bulletCount = 0;

    public override void Start()
    {
        fireDataCross1 = new FireBuilder(crossPrefab, Field);
        fireDataCross1.From(transform);
        fireDataCross1.WithSpeed(12);
        fireDataCross1.WithModifier(new CircularBurstModifier(180, 2, 0, 0));

        fireDataCross2 = new FireBuilder(crossPrefab, Field);
        fireDataCross2.From(transform);
        fireDataCross2.WithSpeed(12);
        fireDataCross2.WithModifier(new CircularBurstModifier(180, 2, 0, 0));

        fireDataCircle = new FireBuilder(circlePrefab, Field);
        fireDataCircle.From(transform);
        fireDataCircle.WithSpeed(5 + 2 * Difficulty);
        fireDataCircle.Towards(new Vector2(new DynamicInt(-100, 100), new DynamicInt(-100, 100)));

        SetRotation(90);

        StartCoroutine(Attack());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!LevelController.Singleton.Paused)
        {
            if (!still && Vector3.Distance(transform.position, Vector3.zero) > 0.1)
            {
                Vector3 direction = Vector3.zero - transform.position;
                GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 10;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                weaponized = true;
            }
        }
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if (Health <= 400)
        {
            enraged = true;
        }

        if (Health <= 200)
        {
            finalForm = true;
        }
    }

    private IEnumerator Attack()
    {
        int cross1x = 50 - 15 * Difficulty;
        int cross1y = 0;
        int cross2x = 0;
        int cross2y = 50 - 15 * Difficulty;
        int change1x = -1;
        int change1y = 1;
        int change2x = -1;
        int change2y = -1;
        int turnSpeed = 50 - 15 * Difficulty;
        bool spedUp = false;
        while (true)
        {
            if (weaponized)
            {
                if (cross1x >= turnSpeed)
                {
                    change1x = -1;
                }
                else if (cross1x <= -turnSpeed)
                {
                    change1x = 1;
                }

                if (cross1y >= turnSpeed)
                {
                    change1y = -1;
                }
                else if (cross1y <= -turnSpeed)
                {
                    change1y = 1;
                }

                if (cross2x >= turnSpeed)
                {
                    change2x = -1;
                }
                else if (cross2x <= -turnSpeed)
                {
                    change2x = 1;
                }

                if (cross2y >= turnSpeed)
                {
                    change2y = -1;
                }
                else if (cross2y <= -turnSpeed)
                {
                    change2y = 1;
                }
                if (enraged && !spedUp)
                {
                    turnSpeed = turnSpeed - 10;
                    cross1x = turnSpeed;
                    cross1y = 0;
                    cross2x = 0;
                    cross2y = turnSpeed;
                    spedUp = true;
                }

                fireDataCross1.Facing(new Vector2(cross1x, cross1y));
                fireDataCross2.Facing(new Vector2(cross2x, cross2y));
                fireDataCircle.Facing(new Vector2(new DynamicInt(-100, 100), new DynamicInt(-100, 100)));
                fireDataCross1.Fire();
                fireDataCross2.Fire();
                if (finalForm)
                {
                    fireDataCircle.Fire();
                }
                
                cross1x += change1x;
                cross1y += change1y;
                cross2x += change2x;
                cross2y += change2y;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
