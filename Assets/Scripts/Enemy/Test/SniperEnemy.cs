using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class SniperEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = 2.5f;
	private float fireDelay = 0.2f;
	private float fireCooldownReset = MAX_FIRE_COOLDOWN;

    private bool isShooting = false;
	private bool hasAimed = false;
    private double previousRotation = 0.0;

	public override void Start()
    {
		//sets the aim in the direction the enemy if facing
		Vector2 aim = (this.transform.position + this.transform.right);

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
		fireData.Towards(aim);
        fireData.WithSpeed(100);
	}
	
	public void Update()
    {
        if(!LevelController.Singleton.Paused)
        {
            fireCooldown -= Time.deltaTime;
			if (fireCooldown <= fireDelay)
            {
                isShooting = true;
            }
            if(fireCooldown <= 0)
            {
                fireData.Fire();
				fireCooldown = fireCooldownReset;
            }
			if (fireCooldown <= (fireCooldownReset - fireDelay) && fireCooldown > fireDelay)
            {
                isShooting = false;
				hasAimed = false;
            }
        }
	}

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(!LevelController.Singleton.Paused && !isShooting)
        {
            Vector3 direction = Player.transform.position - (transform.position);
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
            if(direction.magnitude <= 5)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, Player.transform.position - transform.position), Time.fixedDeltaTime * 5);
        }
        if (!LevelController.Singleton.Paused && isShooting)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			if (!hasAimed) {
				Vector2 aim = (this.transform.position + this.transform.up);
				fireData.Towards(aim);
				fireCooldownReset = Random.Range(1.0f, MAX_FIRE_COOLDOWN);
				Debug.Log (fireCooldown);
				hasAimed = true;
        	}
    	}
	}

    public double AbsVal(double num)
    {
        if (num < 0.0)
        {
            return -num;
        }
        return num;
    } 
}