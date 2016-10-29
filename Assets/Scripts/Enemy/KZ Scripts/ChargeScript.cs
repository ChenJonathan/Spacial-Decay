using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class ChargeScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private bool alive = true;
    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(10 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(100, 5, 0, 0));

        base.Start();
    }

    // Moves back and forth shooting behind them
    protected override IEnumerator Run()
    {
        // SPAWN ON RIGHT SIDE
        if(transform.position.x > 0)
        {
            rigidbody2d.velocity = new Vector2(-3, 0);
            rigidbody2d.rotation = 90;
            yield return new WaitForSeconds(2);
            rigidbody2d.velocity = Vector2.zero;

            while(alive)
            {
                fireData.Facing(Vector2.down);
                for(int j = 0; j < 1 + Difficulty; j++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = new Vector2(-14, 0);
                for(int i = 0; i < (4 * (Difficulty + 1)); i++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = Vector2.zero;
                rigidbody2d.MoveRotation(270);

                fireData.Facing(Vector2.up);
                for(int j = 0; j < 1 + Difficulty; j++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = new Vector2(14, 0);
                for(int i = 0; i < (4 * (Difficulty + 1)); i++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = Vector2.zero;
                rigidbody2d.MoveRotation(90);
            }

            //SPAWN ON LEFT SIDE
        }
        else if(transform.position.x < 0)
        {
            rigidbody2d.velocity = new Vector2(3, 0);
            rigidbody2d.rotation = 270;
            yield return new WaitForSeconds(2);
            rigidbody2d.velocity = Vector2.zero;

            while(alive)
            {
                fireData.Facing(Vector2.up);
                for(int j = 0; j < 1 + Difficulty; j++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = new Vector2(14, 0);
                for(int i = 0; i < (4 * (Difficulty + 1)); i++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = Vector2.zero;
                rigidbody2d.MoveRotation(90f);

                fireData.Facing(Vector2.down);
                for(int j = 0; j < 1 + Difficulty; j++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = new Vector2(-14, 0);
                for(int i = 0; i < (4 * (Difficulty + 1)); i++)
                {
                    fireData.Fire();
                    yield return new WaitForSeconds(0.5f / (Difficulty + 1f));
                }
                rigidbody2d.velocity = Vector2.zero;
                rigidbody2d.MoveRotation(270f);
            }
        }
    }
}
