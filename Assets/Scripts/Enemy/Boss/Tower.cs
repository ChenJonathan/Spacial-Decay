using UnityEngine;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class Tower : Enemy
{
    public DanmakuPrefab BulletPrefab;

    [HideInInspector]
    public TowerBoss Boss;

    private FireBuilder fireData;
    private SpriteRenderer sprite;

    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1;

    private float startCountdown = MAX_START_COUNTDOWN;
    private static readonly float MAX_START_COUNTDOWN = 4f;

    public override void Start()
    {
        TagFilter = "Bullet";
        RotateTowards(Vector3.zero);
        AddRotation(180);

        fireData = new FireBuilder(BulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6);
        fireData.WithAngularSpeed(30);
        fireData.WithModifier(new CircularBurstModifier(360, new DynamicInt(4, 8), 0, 0));
        fireData.WithController(new AccelerationController(3));

        sprite = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if(startCountdown == 0)
        {
            fireCooldown -= Time.deltaTime;
            if(fireCooldown <= 0)
            {
                fireData.Fire();
                fireCooldown = MAX_FIRE_COOLDOWN - .2f * Difficulty;
            }
        }
        else
        {
            startCountdown = Mathf.Max(startCountdown - Time.deltaTime, 0);
            Color temp = sprite.color;
            temp.a = 1 - startCountdown / MAX_START_COUNTDOWN;
            sprite.color = temp;
        }
    }

    public override void Die()
    {
        Boss.UnregisterTower(this);
        Destroy(gameObject);
    }
}