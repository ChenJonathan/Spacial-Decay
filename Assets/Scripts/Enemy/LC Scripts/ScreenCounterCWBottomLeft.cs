using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class ScreenCounterCWBottomLeft : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private bool alive = true;
    private int start = -5;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>(); 

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new CircularBurstModifier(30 + 40 * Difficulty, new DynamicInt(5 + 2 * Difficulty, 10 + 5 * Difficulty), 0, 0));

        base.Start();
    }

    protected override IEnumerator Run()
    {
        do
        {
            // Down
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(0, -6 + start);
            start = 0;
            yield return new WaitForSeconds(3);

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

            // Right
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(10, 0);
            yield return new WaitForSeconds(3);

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

            // Up
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(0, 6);
            yield return new WaitForSeconds(3);

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

            // Left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(-10 + start, 0);
            start = 0;
            yield return new WaitForSeconds(3);

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