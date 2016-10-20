using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class BossEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;
    public DanmakuPrefab laserPrefab;

    private FireBuilder fireDataBullet;
    private FireBuilder fireDataLaser;

    private bool still = false;
    private bool enraged = false;

	public override void Start()
    {
        fireDataBullet = new FireBuilder(bulletPrefab, Field);
        fireDataBullet.From(transform);
        fireDataBullet.WithSpeed(12);
        fireDataBullet.WithModifier(new CircularBurstModifier(45, 7, 0, 0));
        fireDataBullet.WithController(new AccelerationController(3));

        fireDataLaser = new FireBuilder(laserPrefab, Field);
        fireDataLaser.From(transform);
        fireDataLaser.WithSpeed(0);
        fireDataLaser.WithRotation(transform);
        fireDataLaser.WithController(new AutoDeactivateController(0.25f));

        StartCoroutine(Attack());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(!LevelController.Singleton.Paused)
        {
            if(!still && Vector3.Distance(transform.position, Vector3.zero) > 0.1)
            {
                Vector3 direction = Vector3.zero - transform.position;
                GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 3;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if(Health <= 250)
        {
            enraged = true;
            fireDataBullet.WithSpeed(24);
        }
    }

    private IEnumerator Attack()
    {
        while(true)
        {
            Vector3 position;
            int direction;
            for(int i = 0; i < 3; i++)
            {
                SetRotation(180);
                position = Player.transform.position;
                direction = Random.Range(0, 2) * 2 - 1; // -1 or 1
                for(int j = 0; j < 16; j++)
                {
                    fireDataBullet.Towards(position);
                    fireDataBullet.WithRotation((12f * Mathf.Sqrt((16 - j) * 24) - 80) * direction);
                    fireDataBullet.Fire();
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(0.5f);
            }

            for(int i = 0; i < (enraged ? 5 : 3); i++)
            {
                still = true;
                FacePlayer = false;
                TargetRotation = transform.rotation;
                fireDataLaser.Fire();
                yield return new WaitForSeconds(0.25f);

                still = false;
                FacePlayer = true;
                yield return new WaitForSeconds((enraged ? 0.25f : 0.5f));
            }
        }
    }
}