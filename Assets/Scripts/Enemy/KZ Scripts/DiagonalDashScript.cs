using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class DiagonalDashScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private bool alive = true;
    private int start = -5;
    /// <summary> Customizable multiplier for the enemy's velocity. </summary>
    private int velocityMultiplier = 1;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new CircularBurstModifier(30 + 40 * Difficulty, new DynamicInt(10 + 5 * Difficulty, 20 + 10 * Difficulty), 0, 0));

        if (parameters.Length > 0) {
            velocityMultiplier = (int) parameters[0];
        }
        start *= velocityMultiplier;

        base.Start();
    }

    protected override IEnumerator Run()
    {
        do
        {
            // Down Left
            FacePlayer = true;
            int baseSpeed = -4 * velocityMultiplier;
            rigidbody2d.velocity = new Vector2(baseSpeed + start, baseSpeed + start);
            start = 0;
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }

            //Up Left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(baseSpeed, -baseSpeed);
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }

            //Up Right
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(-baseSpeed, -baseSpeed);
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }

            //Down Right
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(-baseSpeed, baseSpeed);
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.2f);
            for (int j = 0; j < 3; j++)
            {
                fireData.Fire();
                yield return new WaitForSeconds(0.2f);
            }
        } while (alive);
        Die();
    }
}