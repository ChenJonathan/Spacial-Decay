using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class RegularLineScript1 : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;
    private float timer = 0;
    private bool up = false;
    private bool left = false;
    private bool down = false;
    private bool right = false;

    //Arrive on Scene
    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(3 + Difficulty);
        fireData.WithModifier(new LinearBurstModifier(7, 0, 0));

        if (transform.position.x > 18)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
            SetRotation(90);
            fireData.Facing(Vector2.up);
        }
        else if (transform.position.y <= -10)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 10);
            SetRotation(0);
            fireData.Facing(Vector2.right);
        }
        else if (transform.position.x <= -18)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
            SetRotation(270);
            fireData.Facing(Vector2.down);
        }
        else if (transform.position.y >= 10)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
            SetRotation(180);
            fireData.Facing(Vector2.left);
        }
    }

    // Static shooting
    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0 && timer > 2)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN - (.49f * Difficulty);
        }
    }

    // Move around the edge
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer += Time.deltaTime;
        if(timer > 1)
        {
            if(transform.position.x > 16 && !up)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2 + Difficulty);
                SetRotation(0);
                fireData.Facing(Vector2.up);
                up = true;
                down = false;
            }
            else if(transform.position.y >= 8 && !left)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-2 - Difficulty, 0);
                SetRotation(90);
                fireData.Facing(Vector2.left);
                left = true;
                right = false;
            }
            else if(transform.position.x <= -16 && !down)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2 - Difficulty);
                SetRotation(180);
                fireData.Facing(Vector2.down);
                down = true;
                up = false;
            }
            else if(transform.position.y <= -8 && !right)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(2 + Difficulty, 0);
                SetRotation(270);
                fireData.Facing(Vector2.right);
                right = true;
                left = false;
            }
        }
    }
}

