using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class RapidFireScript : Enemy
{
    public DanmakuPrefab bulletPrefab;
    public DanmakuPrefab aimPrefab;

    private FireBuilder fireData;
    private FireBuilder aimData;
    private float fireCooldown = 3f;
    private float timer = 2f;
    private int bullets = 0;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        fireData.WithModifier(new RandomizeAngleModifier(360));

        aimData = new FireBuilder(aimPrefab, Field);
        aimData.From(transform);
        aimData.Towards(Player.transform);
        aimData.WithSpeed(3 + 3 * Difficulty);
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(fireCooldown < 0)
        {
            fireData.Fire();
            if (bullets % 30 == 0)
            {
                aimData.Fire();
            }
            fireCooldown = .001f - .0003f * Difficulty;
            bullets++;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer -= Time.deltaTime;
        if(timer > 0)
        {
            Vector3 direction = new Vector3(-1.0f, 0.0f);
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 10;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(0.0f, 0.0f);
        }
    }
}