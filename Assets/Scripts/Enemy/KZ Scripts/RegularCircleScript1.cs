using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class RegularCircleScript1 : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 2f;
    private float timer = 0;

    //Arrive on the scene
    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(3 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(220, new DynamicInt(10, 15), 0, 0));

        if(transform.position.x > 18)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
            SetRotation(90);
            fireData.Facing(Vector2.up);
        } else if(transform.position.x <= -18)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
            SetRotation(270);
            fireData.Facing(Vector2.down);
        } else if(transform.position.y >= 10)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
            SetRotation(180);
            fireData.Facing(Vector2.left);
        } else if(transform.position.y <= -10)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 10);
            SetRotation(0);
            fireData.Facing(Vector2.right);
        }
    }

    // Static shooting
    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0 && timer > 2)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN - (.66f * Difficulty);
        }
    }

    // STOP!
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer += Time.deltaTime;
        if(timer > 1)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
