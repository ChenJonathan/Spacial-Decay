using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class DarkEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 0.3f;

    public void Start()
    {
        fireData = new FireBuilder(bulletPrefab, field);
        fireData.From(transform);
        fireData.Towards(player.transform);
        fireData.WithSpeed(6);
        fireData.WithModifier(new CircularBurstModifier(360, new DynamicInt(10, 20), 0, 0));

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2);
        transform.RotateAround(transform.position, transform.forward, 180f);
    }

    public override void NormalUpdate()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN;
        }
    }

    public override void NormalFixedUpdate()
    {
        if(transform.position.y <= -10)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2);
            transform.RotateAround(transform.position, transform.forward, 180f);
        }
        else if(transform.position.y >= 10)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2);
            transform.RotateAround(transform.position, transform.forward, 180f);
        }
    }
}