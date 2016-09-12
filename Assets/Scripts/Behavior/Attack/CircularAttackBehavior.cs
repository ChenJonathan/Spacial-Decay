using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Controllers;
using DanmakU.Modifiers;
using System.Collections.Generic;

public class CircularAttackBehavior : Enemy.AttackBehavior
{
    public DanmakuPrefab bullet;

    public bool targetPlayer;
    public DynamicFloat fireSpeed;
    public DynamicFloat fireRate;
    public DynamicFloat range;
    public DynamicInt count;
    public DynamicFloat deltaSpeed;
    public DynamicFloat deltaAngularSpeed;

    private FireBuilder fireData;
    private float fireDelay = 0;
    
    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);

        this.fireData = new FireBuilder(bullet, enemy.Field);
        fireData.From(enemy);
        if(targetPlayer)
            fireData.Towards(enemy.Player);
        fireData.WithSpeed(fireSpeed);
        
        CircularBurstModifier cbm = new CircularBurstModifier(range, count, deltaSpeed, deltaAngularSpeed);
        fireData.WithModifier(cbm);
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