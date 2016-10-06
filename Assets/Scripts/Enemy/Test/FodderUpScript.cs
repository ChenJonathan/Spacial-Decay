using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class FodderUpScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private bool alive = true;
    private int start = 5;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new CircularBurstModifier(100 + 40 * Difficulty, 20 + 5 * Difficulty, 0, 0));

        base.Start();
    }

    protected override IEnumerator Run()
    {
        do
        {
            // Down Left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(4 + start, 4 + start);
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
            rigidbody2d.velocity = new Vector2(4, -4);
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
            rigidbody2d.velocity = new Vector2(-4, -4);
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
            rigidbody2d.velocity = new Vector2(-4, 4);
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