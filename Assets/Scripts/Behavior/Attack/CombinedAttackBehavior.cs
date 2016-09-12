using System.Collections.Generic;

public class CombinedAttackBehavior : Enemy.AttackBehavior
{
    // TODO Make varying duration behaviors work

    public List<Enemy.AttackBehavior> attackBehaviors;

    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);
        foreach (Enemy.AttackBehavior attackBehavior in attackBehaviors)
        {
            attackBehavior.BehaviorStart(enemy);
        }
    }

    public override void BehaviorUpdate(Enemy enemy)
    {
        foreach (Enemy.AttackBehavior attackBehavior in attackBehaviors)
        {
            attackBehavior.BehaviorUpdate(enemy);
        }
    }

    public override void BehaviorFixedUpdate(Enemy enemy)
    {
        foreach(Enemy.AttackBehavior attackBehavior in attackBehaviors)
        {
            attackBehavior.BehaviorFixedUpdate(enemy);
        }
    }
}