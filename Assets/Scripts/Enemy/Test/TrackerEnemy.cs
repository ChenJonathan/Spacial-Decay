using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class TrackerEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 1f;

	public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(6 + 2 * Difficulty);
        HomingController HomingController = new HomingController();
        HomingController.Target = Player.transform;
        fireData.WithController(HomingController);
        fireData.WithController(new AutoDeactivateController(6f + Difficulty));
    }
	
	public void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN;
        }
	}

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 direction = Player.transform.position - transform.position;
        GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
        if (direction.magnitude <= 5)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}