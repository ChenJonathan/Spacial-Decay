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
	private float fireDelay = 0.3f;
	private float fireCooldownReset = MAX_FIRE_COOLDOWN;

    private bool isShooting = false;
	private bool hasAimed = false;
    private double previousRotation = 0.0;

	private LineRenderer laser;
	private Vector3 laserStart = new Vector3 (0, 0, 0);
	private Vector3 laserEnd = new Vector3 (0, 50, 0);

	public override void Start()
    {
		laser = gameObject.AddComponent<LineRenderer>();
		laser.SetPosition (0, laserStart);
		laser.SetPosition (1, laserEnd);
		laser.SetWidth (0.05f, 0.05f);
		laser.material = new Material(Shader.Find("Particles/Additive"));
		laser.SetColors (Color.red, Color.red);
        laser.useWorldSpace = false;
        laser.enabled = false;
		// Sets the aim in the direction the enemy if facing

		Vector2 aim = (this.transform.position + this.transform.right);

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
		fireData.Towards(aim);
        fireData.WithSpeed(100);
	}
	
	public void Update()
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

    public override void FixedUpdate()
    {
        base.FixedUpdate();

		laserStart = this.transform.position;
		laserEnd = laserStart + (this.transform.up * 5);

        if(!isShooting)
        {
			laser.enabled = false;
            Vector3 direction = Player.transform.position - (transform.position);
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
            if(direction.magnitude <= 5)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, Player.transform.position - transform.position), Time.fixedDeltaTime * 5);
        }
        if (isShooting)
        {
			laser.enabled = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			if (!hasAimed) {
				Vector2 aim = (this.transform.position + this.transform.up);
				fireData.Towards(aim);
				fireCooldownReset = Random.Range(1.0f, MAX_FIRE_COOLDOWN - (0.75f * Difficulty));
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