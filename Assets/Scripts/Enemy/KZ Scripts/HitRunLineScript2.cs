using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class HitRunLineScript2 : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float timer = 0;
    private int yVelocity = 0;
    private int xVelocity = 0;
    private int bullet = 0;

    //Arrive on the scene
    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithModifier(new CircularBurstModifier(50, 2, 0, 0));
        fireData.Towards(Player.transform);

        if (transform.position.x > 18)
        {
            xVelocity = -10;
        }
        if (transform.position.x <= -18)
        {
            xVelocity = 10;
        }
        if (transform.position.y >= 10)
        {
            yVelocity = -10;
        }
        if (transform.position.y <= -10)
        {
            yVelocity = 10;
        }
        SetRotation(new Vector2(xVelocity, yVelocity));
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);
        StartCoroutine(Attack());
    }

    //Shoot
    private IEnumerator Attack()
    {
        while (true)
        {
            if (timer > 1.1 && timer < 2)
            {
                for (int i = 0; i < 7 + 2 * Difficulty; i++)
                {
                    fireData.WithSpeed(10 + 2 * Difficulty - bullet);
                    fireData.Fire();
                    bullet++;
                }
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    // Die
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer += Time.deltaTime;
        if (timer > 1 && timer < 2)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        if (timer > 2)
        {
            SetRotation(new Vector2(-xVelocity, -yVelocity));
            GetComponent<Rigidbody2D>().velocity = new Vector2(-xVelocity, -yVelocity);
        }
        if (timer > 4)
        {
            Die();
        }
    }
}

