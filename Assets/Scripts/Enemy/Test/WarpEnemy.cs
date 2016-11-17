using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class WarpEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private float warning = -1;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;
    private static readonly float MAX_WARNING = 1f;
    private static readonly float speed = 1f;
    private bool onscreen = false;

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(4 + 2 * Difficulty);
        fireData.WithModifier(new CircularBurstModifier(100, new DynamicInt(4, 6) + Difficulty, 0, 0));
	}
	
	public void Update()
    {
        if(warning<=0)
            fireCooldown -= Time.deltaTime;
        if(fireCooldown <= 0)
        {
            onscreen = !onscreen;
            fireCooldown = MAX_FIRE_COOLDOWN;
            if (onscreen)
            {
                warning = MAX_WARNING/(1+Difficulty);
                GetComponent<Rigidbody2D>().position = new Vector2(Random.Range(-5,5), Random.Range(-5,5));
                float angle = Random.Range(0,2*Mathf.PI);
                GetComponent<Rigidbody2D>().velocity = new Vector2(speed*Mathf.Cos(angle),speed*Mathf.Sin(angle));
            }
            else
            {
                GetComponent<Rigidbody2D>().position = new Vector2(-100,-100);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            }
        }
        if(onscreen&&warning < 0)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Color color = renderer.color;
            color.a = fireCooldown/MAX_FIRE_COOLDOWN;
            renderer.color = color;
        }
        if (warning > 0)
        {
            warning -= Time.deltaTime;
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Color color = renderer.color;
            color.a = (((MAX_WARNING/(1+Difficulty))-warning)/(MAX_WARNING/(1+Difficulty)));
            renderer.color = color;
            if (warning <= 0)
            {
                fireData.Fire();
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 direction = Player.transform.position - transform.position;
    }
}