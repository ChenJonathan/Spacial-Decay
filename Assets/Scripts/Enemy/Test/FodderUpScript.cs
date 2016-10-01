using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class FodderUpScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = 0;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;
    private float deathTimer = 2f;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(20);

    }

    public void Update()
    {
        if (!LevelController.Singleton.Paused)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0 && deathTimer > 1)
            {
                fireData.Fire();
                fireCooldown = MAX_FIRE_COOLDOWN / 4;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!LevelController.Singleton.Paused)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0)
            {
                Die();
            }
            FacePlayer = true;
            Vector3 direction = new Vector3(-1.0f, 1.0f);
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 20;
        }
    }
}