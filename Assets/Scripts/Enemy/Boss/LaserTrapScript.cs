using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class LaserTrapScript : Enemy
{
    public DanmakuPrefab laserPrefab;
    public DanmakuPrefab petalPrefab;
    public DanmakuPrefab circlePrefab;

    private FireBuilder fireDataLaser;
    private FireBuilder fireDataPetal;
    private FireBuilder fireDataCircle;
    private bool enraged = false;
    private bool weaponized = false;
    private bool finalForm = false;
    private float atkSpd;

    public override void Start()
    {
        fireDataLaser = new FireBuilder(laserPrefab, Field);
        fireDataLaser.From(transform);
        fireDataLaser.WithSpeed(0);
        fireDataLaser.WithModifier(new CircularBurstModifier(90, 2, 0, 0));

        fireDataPetal = new FireBuilder(petalPrefab, Field);
        fireDataPetal.From(transform);
        fireDataPetal.WithSpeed(6 + Difficulty);
        fireDataPetal.WithModifier(new CircularBurstModifier(92, new DynamicInt(15, 25), 0, 0));
        fireDataPetal.WithController(new AccelerationController(3));

        fireDataCircle = new FireBuilder(circlePrefab, Field);
        fireDataCircle.From(transform);
        fireDataCircle.WithSpeed(8 + 2 * Difficulty);
        fireDataCircle.WithModifier(new CircularBurstModifier(360, new DynamicInt(10, 15), 0, 0));
        fireDataCircle.WithController(new AccelerationController(5f));

        switch (Difficulty)
        {
            case 0: atkSpd = 3.5f;
                break;

            case 1: atkSpd = 3f;
                break;

            case 2: atkSpd = 2.5f;
                break;

            default: atkSpd = 3f;
                break;
        }

        StartCoroutine(Attack());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();



        if (!weaponized && Vector3.Distance(transform.position, Vector3.zero) > 0.1)
        {
            Vector3 direction = Vector3.zero - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 10;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            weaponized = true;
        }

    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if (Health <= 400)
        {
            enraged = true;
        }

        if (Health <= 200)
        {
            finalForm = true;
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (weaponized)
            {
                FacePlayer = false;
                Transform target = transform;
                yield return new WaitForSeconds(.1f);
                fireDataLaser.WithController(new AutoDeactivateController((finalForm ? (atkSpd / 2f) : atkSpd)));
                fireDataLaser.WithRotation(target);
                fireDataPetal.WithRotation(transform);
                fireDataLaser.Fire();
                for (int i = 0; i < 10; i++)
                {
                    
                    fireDataPetal.Fire();
                    yield return new WaitForSeconds(finalForm ? atkSpd / 20f: atkSpd / 10f);
                    if (i % (finalForm ? 2: 4) == 0)
                    {
                        EnragedAttack();
                    }
                }
                FacePlayer = true;
                yield return new WaitForSeconds(finalForm ? 0.5f : 1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void EnragedAttack()
    {
        if (enraged)
        {
            fireDataCircle.Fire();
        }
    }
}