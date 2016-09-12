using UnityEngine;
using DanmakU;
using System.Collections.Generic;

public class ConstantAttackBehavior : Enemy.AttackBehavior
{
    public DanmakuPrefab bullet;
    public bool targetPlayer, trackPlayer;
    public Vector2 target;
    public DynamicFloat fireSpeed;
    public DynamicFloat fireRate;

    private FireBuilder fireData;
    private float fireDelay;
    
    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);
        fireDelay = 0;

        fireData = new FireBuilder(bullet, enemy.Field);
        fireData.From(enemy);
        if (trackPlayer)
            fireData.Towards(player);
        else
        {
            if (targetPlayer)
                target = player.transform.position;
            fireData.Towards(target);
        }
        fireData.WithSpeed(fireSpeed);
    }

    public override void BehaviorUpdate(Enemy enemy)
    {
        base.BehaviorUpdate(enemy);
        
        if (fireDelay <= 0)
        {
            fireDelay = 1 / fireRate;
            fireData.Fire();
        }
        fireDelay -= Time.deltaTime;
    }
}