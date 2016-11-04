using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class BulletChaseScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(6);
        fireData.WithModifier(new CircularBurstModifier(100, 2, 0, 0));
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        
    }
}
