using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class BorderEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private float fireCooldown = MAX_FIRE_COOLDOWN;
    private static readonly float MAX_FIRE_COOLDOWN = .1f;
    
    private Vector3 direction;
    private static readonly float MAX_PAUSE_COOLDOWN = 2f;
    private float pauseCooldown;
    public float dieTime = 50f;
    Vector3 vectorToTarget;
    Quaternion q;    

    public override void Start()
    {
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.Towards(Player.transform);
        fireData.WithSpeed(6 + Difficulty);
        fireData.WithModifier(new CircularBurstModifier(30, 3 + Difficulty, 0, 0));
        pauseCooldown = MAX_PAUSE_COOLDOWN / (Difficulty + 1);
    }

    public void Update()
    {
        fireCooldown -= Time.deltaTime;
        pauseCooldown -= Time.deltaTime;
        if(fireCooldown <= 0 && pauseCooldown <= 0)
        {
            fireData.Fire();
            fireCooldown = MAX_FIRE_COOLDOWN;
        }

        if(pauseCooldown <= -.33f + -.33f * Difficulty)
        {
            pauseCooldown = MAX_PAUSE_COOLDOWN / (Difficulty + 1);
        }
    }

    public override void FixedUpdate()
    {        
        base.FixedUpdate();

        dieTime -= Time.deltaTime;
        if(dieTime >= 0)
        {
            if(transform.position.y > 8) // Travel to quadrant 1
            {
                direction = new Vector3(20, 8, 0) - transform.position;
            }
            else if(transform.position.x < -20) // To quadrant 2
            {
                direction = new Vector3(-20, 8, 0) - transform.position;
            }
            else if(transform.position.y < -8) // To quadrant 3
            {
                direction = new Vector3(-20, -8, 0) - transform.position;
            }
            else if(transform.position.x > 20) // To quadrant 4
            {
                direction = new Vector3(20, -8, 0) - transform.position;
            }
        }
        else
        {
            if(dieTime <= -5f)
            {
                Die();
            }
        }

        GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 6;
        vectorToTarget = direction;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5);
    }
}        