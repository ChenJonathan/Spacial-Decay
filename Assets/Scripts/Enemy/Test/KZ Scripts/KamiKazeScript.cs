using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class KamiKazeScript : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Vector3 direction;
    private float timer = 1f;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
    }

    public override void Update()
    {
        base.Update();

        if (!LevelController.Singleton.Paused)
        {
           if (direction.magnitude <= 2 + Difficulty)
            {
                for (int i = 0; i < 500; i++)
                {
                    fireData.WithSpeed(new DynamicInt(30, 50));
                    fireData.Facing(new Vector2(new DynamicInt(-100, 100), new DynamicInt(-100, 100)));
                    fireData.Fire();
                }
                Die();
                // Add bullet removal method here
            }
            timer -= Time.deltaTime;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!LevelController.Singleton.Paused)
        {

            Renderer renderer = GetComponent<Renderer>();
            Color color = renderer.material.color;
            direction = Player.transform.position - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
            if (timer >= 0.5)
            {
                color = new Color(1, 0, 0);
                renderer.material.color = color;
            } else if(timer > 0)
            {
                color = new Color(1, 1, 1);
                renderer.material.color = color;
            } else if(timer <= 0)
            {
                timer = 1;
            }
        }
    }
}
