using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class KamiKazeScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Vector3 direction;
    private float timer = 0.5f;
    private float detonateTimer = 0f;
    private bool detonating = false;
    private bool timeReset = false;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithController(new AutoDeactivateController(10 + Difficulty * 2));
    }

    public void Update()
    {
        if (direction.magnitude <= 4 + Difficulty)
        {
            detonating = true;
        }
        if (detonateTimer >= 1f)
        {
            for (int i = 0; i < 250; i++)
            {
                fireData.WithSpeed(new DynamicInt(30, 50));
                fireData.Facing(new Vector2(new DynamicInt(-100, 100), new DynamicInt(-100, 100)));
                fireData.Fire();
            }
            Die();
            // Add bullet removal method here
        }
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();


        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        direction = Player.transform.position - transform.position;
            
        if (detonating)
        {
            if (!timeReset)
            {
                timer = 0.1f;
                timeReset = true;
            }
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            detonateTimer += Time.deltaTime;
        } else
        {
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * (3 + Difficulty);
        }

        if (timer >= (detonating ? 0.05f : 0.25f))
        {
            color = new Color(1, 0, 0, 1);
            renderer.material.color = color;
        }
        else if (timer > 0f)
        {
            color = new Color(1, 1, 1, 1);
            renderer.material.color = color;
        }
        else if (timer <= 0f)
        {
            timer = (detonating ? 0.1f : 0.5f);
        }
        timer -= Time.deltaTime;
    }
}
