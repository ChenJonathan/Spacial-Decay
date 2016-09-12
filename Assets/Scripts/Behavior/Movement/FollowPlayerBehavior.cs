using UnityEngine;
using System.Collections;

public class FollowPlayerBehavior : Enemy.MovementBehavior
{
    public float speed;
    public float deltaSpeed;
    public float distanceFromPlayer;

    private float initialSpeed;

    public override void BehaviorStart(Enemy enemy)
    {
        base.BehaviorStart(enemy);

        initialSpeed = speed;
    }

    public override void BehaviorFixedUpdate(Enemy enemy)
    {
        float distance = Vector2.Distance(player.transform.position, enemy.transform.position);
        if(distance > distanceFromPlayer)
        {
            Vector2 destination = (enemy.transform.position - player.transform.position) / distance * distanceFromPlayer + player.transform.position;
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, destination, speed * Time.fixedDeltaTime);
            speed += deltaSpeed * Time.fixedDeltaTime;
        }
        else
            speed = initialSpeed;
	}
}
