using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class DarkEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 0.3f;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(3);
        fireData.WithAngularSpeed(45);
        fireData.WithModifier(new CircularBurstModifier(340, new DynamicInt(10, 20), 0, 0));
        fireData.WithController(new AccelerationController(3));

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2);
        SetRotation(0);
    }

    public override void Update()
    {
        base.Update();

        if(!LevelController.Singleton.Paused)
        {
            fireCooldown -= Time.deltaTime;
            if(fireCooldown <= 0)
            {
                fireData.Fire();
                fireCooldown = MAX_FIRE_COOLDOWN;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(!LevelController.Singleton.Paused)
        {
            if(transform.position.y <= -8)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2);
                SetRotation(0);
            }
            else if(transform.position.y >= 8)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2);
                SetRotation(180);
            }
        }
    }
}