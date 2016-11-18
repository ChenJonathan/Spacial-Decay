using UnityEngine;
using DanmakU;

public class SwarmEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 0.025f;

    private int direction;
    private int fireCount = 0;
    private float moveTimer = 2;

    private Rigidbody2D rigidbody2d;

	public override void Start()
    {
        direction = transform.position.x < 0 ? 1 : -1;
        transform.Rotate(Vector3.forward * -90 * direction);

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(new Vector2(0, transform.position.y));
        fireData.WithSpeed(3 + Difficulty);
        fireData.WithRotation(-30, 30);

        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.velocity = new Vector2(direction * 4, 0);
	}
	
	public void Update()
    {
        if(moveTimer <= 0)
        {
            rigidbody2d.velocity = Vector2.zero;

            fireCooldown -= Time.deltaTime;
            if(fireCooldown <= 0)
            {
                if(fireCount < 100)
                {
                    fireCount++;
                    fireData.Fire();
                    fireCooldown = MAX_FIRE_COOLDOWN;
                }
                else
                {
                    moveTimer = 10;
                    direction = -direction;
                    rigidbody2d.velocity = new Vector2(direction, 0);
                    transform.Rotate(Vector3.forward * 180);
                }
            }
        }
        else
        {
            moveTimer -= Time.deltaTime;

            if(moveTimer <= 0 && fireCount == 100)
                Die();
        }
    }
}