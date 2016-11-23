using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class FodderScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown;
    private static readonly float MAX_FIRE_COOLDOWN = 2f;
    private float timer = 5f;

    /// <summary> Multiplier for the enemy's horizontal speed. </summary>
    private float speedMultiplier = 1;

    public override void Start()
    {
        fireCooldown = MAX_FIRE_COOLDOWN - .5f * Difficulty;
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(5 + Difficulty * 2);

        if (parameters.Length > 0) {
            speedMultiplier = parameters[0];
        }
        if (transform.position.x > 0)
        {
            SetRotation(90);
        } else
        {
            SetRotation(270);
        }
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN - .5f * Difficulty;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            if (transform.position.x > 19 || transform.position.x < -19)
            {
                Die();
            }
        }
        Vector3 direction = new Vector3(speedMultiplier, 0.0f);
        GetComponent<Rigidbody2D>().velocity = direction * 3;
    }
}