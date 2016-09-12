using UnityEngine;

public class RelativeMovementBehavior : Enemy.MovementBehavior
{
    public Vector2 direction;

    private Vector2 start;

    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);
        start = enemy.transform.position;
    }

    public override void BehaviorUpdate(Enemy enemy)
    {
        base.BehaviorUpdate(enemy);
        
        enemy.transform.position = Vector2.Lerp(start, start + direction, time / duration);
    }
}