using UnityEngine;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class Tower : Enemy
{
    public DanmakuPrefab BulletPrefab;

    [HideInInspector]
    public BossEnemy Boss;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1;

    public override void Start()
    {
        TagFilter = "Bullet";

        fireData = new FireBuilder(BulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6);
        fireData.WithAngularSpeed(30);
        fireData.WithModifier(new CircularBurstModifier(360, new DynamicInt(4, 8), 0, 0));
        fireData.WithController(new AccelerationController(3));
    }

    public override void Update()
    {
        base.Update();
        
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN;
        }
    }

    public override void Die()
    {
        Boss.UnregisterTower(this);
        Destroy(gameObject);
    }
}