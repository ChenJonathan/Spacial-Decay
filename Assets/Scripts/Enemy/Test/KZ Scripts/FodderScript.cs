using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class FodderScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;
    private float timer = 15f;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(5 + Difficulty * 2);

    }

    public override void Update()
    {
        base.Update();

        if(!LevelController.Singleton.Paused)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0)
            {
                fireData.Fire();
                fireCooldown = MAX_FIRE_COOLDOWN / (1 + Difficulty / 4);
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!LevelController.Singleton.Paused)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Die();
            }
            Vector3 direction = new Vector3(-1.0f, 0.0f);
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
        }

    }
}