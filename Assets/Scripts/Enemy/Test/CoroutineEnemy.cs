using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class CoroutineEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private float speed;

	public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new CircularBurstModifier(45, 2, 0, 0));

        speed = -5;
        if (parameters.Length > 0) {
            speed *= parameters[0];
        }

        base.Start();
	}

    protected override IEnumerator Run()
    {   

        for(int i = 0; i < 3; i++)
        {
            // Moving left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(speed, 0);
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for(int j = 0; j < 10; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }
        }
        FacePlayer = true;
        rigidbody2d.velocity = new Vector2(speed, 0);
        yield return new WaitForSeconds(2);
        Die();
    }
}