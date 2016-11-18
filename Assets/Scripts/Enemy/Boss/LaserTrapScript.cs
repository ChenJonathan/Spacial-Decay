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
    public DanmakuPrefab laserWarning;
    public DanmakuPrefab meteor;

    private FireBuilder fireDataLaser;
    private FireBuilder fireDataPetal;
    private FireBuilder fireDataCircle;
    private FireBuilder warningLaser;
    private FireBuilder fireLaser2;
    private FireBuilder rapidFire;
    private FireBuilder scatterCircle;
    private bool enraged = false;
    private bool weaponized = false;
    private bool finalForm = false;
    private float atkSpd;
    private int phase = 1;
    private int bulletCount = 0;
    private bool charging = true;
    private bool chargingLaser = true;
    private float rotate = 0;
    private float rotator = 2.5f;

    public override void Start()
    {
        fireDataLaser = new FireBuilder(laserPrefab, Field);
        fireDataLaser.From(transform);
        fireDataLaser.WithSpeed(0);
        fireDataLaser.WithModifier(new CircularBurstModifier(90, 2, 0, 0));

        warningLaser = new FireBuilder(laserWarning, Field);
        warningLaser.From(transform);
        warningLaser.WithSpeed(0);
        warningLaser.WithDamage(0);
        warningLaser.WithController(new AutoDeactivateController(.01f));


        fireLaser2 = new FireBuilder(laserPrefab, Field);
        fireLaser2.From(transform);
        fireLaser2.WithSpeed(0);
        fireLaser2.WithController(new AutoDeactivateController(.01f));

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

        rapidFire = new FireBuilder(meteor, Field);
        rapidFire.From(transform);
        rapidFire.WithModifier(new RandomizeAngleModifier(360));
        rapidFire.WithController(new AutoDeactivateController(3f));

        scatterCircle = new FireBuilder(circlePrefab, Field);
        scatterCircle.From(transform);
        scatterCircle.WithSpeed(8 + 2 * Difficulty);
        scatterCircle.WithModifier(new RandomizeAngleModifier(360));
        scatterCircle.WithController(new SpeedLimitController(1f, 12f));
        scatterCircle.WithController(new AccelerationController(-12f));

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

        if (Health <= 150)
        {
            finalForm = true;
            enraged = true;
        }
        else if (Health <= 300)
        {
            enraged = true;
            finalForm = false;
        }
        else if (Health <= 450)
        {
            phase = 2;
            enraged = false;
            finalForm = false;
        }
        else if (Health <= 600)
        {
            enraged = true;
            finalForm = true;
        }
        else if (Health <= 750)
        {
            enraged = true;
            finalForm = false;
        }
        else if (Health <= 900)
        {
            enraged = false;
            finalForm = false;
            phase = 1;
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (weaponized)
            {
                switch (phase)
                {
                    case 1:
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
                            yield return new WaitForSeconds(finalForm ? atkSpd / 20f : atkSpd / 10f);
                            if (i % (finalForm ? 2 : 4) == 0)
                            {
                                if (enraged)
                                {
                                    fireDataCircle.Fire();
                                }
                            }
                        }
                        FacePlayer = true;
                        yield return new WaitForSeconds(finalForm ? 0.5f : 1f);
                        break;
                    case 2:
                        SetRotation(0);
                        if (charging)
                        {
                            yield return new WaitForSeconds(2);
                            charging = false;
                        }
                        warningLaser.WithRotation(rotate);
                        fireLaser2.WithRotation(rotate);
                        fireLaser2.Fire();
                        if (bulletCount % (finalForm ? 75 : 100) == 0)
                        {
                            for (int i = 0; i < 75 + 25 * Difficulty; i++)
                            {
                                rapidFire.WithSpeed(new DynamicInt(7, 10));
                                rapidFire.Fire();
                            }
                        }
                        if (bulletCount % ((finalForm ? 5 : 7)) == 0 && enraged)
                        {
                            scatterCircle.WithRotation(rotate);
                            scatterCircle.Fire();
                        }
                        rotate += (rotator + 0.5f * Difficulty);
                        //WILL IMPLEMENT THIS LATER TO CHARGE THE LASER
                        /* if ((bulletCount / 100) % 2 == 0)
                        {
                            chargingLaser = true;
                        } else
                        {
                            chargingLaser = false;
                        } */
                        bulletCount++;
                        yield return new WaitForSeconds(.02f);
                        break;
                    default:
                        break;
                }
            } else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

}