using UnityEngine;
using DanmakU;
using DanmakU.Modifiers;

public class CircularDiamondAttackBehavior : Enemy.AttackBehavior
{
    private DanmakuPrefab bullet;

    public bool targetPlayer;
    public Vector2 target;
    public DynamicFloat fireSpeed;
    public DynamicFloat angle;
    public DynamicInt diamondSize;
    
    private FireBuilder fireData;
    private CircularBurstModifier modifier;
    private bool increasingSize;
    private float fireDelay;

    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);
        increasingSize = true;
        fireDelay = 0;

        this.fireData = new FireBuilder(bullet, enemy.Field);
        fireData.From(enemy);
        if(targetPlayer)
            target = player.transform.position;
        fireData.Towards(target);
        fireData.WithSpeed(fireSpeed);
        modifier = new CircularBurstModifier(0, 1, 0, 0);
        fireData.WithModifier(modifier);
    }

    public override void BehaviorUpdate(Enemy enemy)
    {
        base.BehaviorUpdate(enemy);

        if (fireDelay <= 0)
        {
            fireDelay = duration / diamondSize;
            fireData.Fire();
            if (modifier.Count == diamondSize)
                increasingSize = false;
            if (increasingSize)
            {
                modifier.Count += 1;
                modifier.Range += angle;
            }
            else
            {
                modifier.Count -= 1;
                modifier.Range -= angle;
            }
        }
        fireDelay -= Time.deltaTime;
    }
}