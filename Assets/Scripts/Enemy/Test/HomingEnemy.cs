using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class HomingEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = maxFireCooldown;
    private static float maxFireCooldown = 2;

	public void Start()
    {
        fireData = new FireBuilder(bulletPrefab, field);
        fireData.From(transform);
        fireData.Towards(player.transform);
        fireData.WithRotation(90);
        fireData.WithSpeed(10);

        HomingController hc = new HomingController();
        hc.Target = player.transform;
        fireData.WithController(hc);
        fireData.WithController(new AccelerationController(3));
	}
	
	public override void NormalUpdate()
    {
        fireCooldown -= Time.deltaTime;
	    if(fireCooldown <= 0)
        {
            fireData.Fire();
            fireCooldown = maxFireCooldown;
        }
    }

    public override void NormalFixedUpdate()
    {
        Vector3 direction = player.transform.position - transform.position;
        GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
        if(direction.magnitude <= 5)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}