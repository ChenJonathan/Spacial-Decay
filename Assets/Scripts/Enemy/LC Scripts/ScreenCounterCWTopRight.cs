﻿using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class ScreenCounterCWTopRight : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private bool alive = true;
    private int start = -5;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>(); 

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new CircularBurstModifier(100 + 40 * Difficulty, new DynamicInt(10 + 5 * Difficulty, 20 + 10 * Difficulty), 0, 0));

        base.Start();
    }

    protected override IEnumerator Run()
    {
        do
        {
            // Up
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(0, 6 + Mathf.Abs(start));
            start = 0;
            yield return new WaitForSeconds(3);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }

            // Left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(-10, 0);
            yield return new WaitForSeconds(3);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }

            // Down
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(0, -6 + start);
            start = 0;
            yield return new WaitForSeconds(3);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }

            // Right
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(10, 0);
            yield return new WaitForSeconds(3);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }                    
        } while (alive);
        Die();
    }
}