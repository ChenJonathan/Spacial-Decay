using UnityEngine;

public class LinearMovementBehavior : Enemy.MovementBehavior
{
    public Vector2 target;

    private Vector2 start;
    
    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);
        start = enemy.transform.position;
    }

    public override void BehaviorUpdate(Enemy enemy)
    {
        base.BehaviorUpdate(enemy);
        
        enemy.transform.position = Vector2.Lerp(start, target, time / duration);
	}
}
