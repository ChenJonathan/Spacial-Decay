using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;
using DanmakU.Controllers;

public class SpiralBossScript : Enemy
{
    public DanmakuPrefab crossPrefab;
    public DanmakuPrefab circlePrefab;
    public DanmakuPrefab enragedPrefab;

    private FireBuilder fireDataCross1;
    private FireBuilder fireDataCross2;
    private FireBuilder fireDataCross3;
    private FireBuilder fireDataCross4;
    private FireBuilder fireDataCircle;
    private FireBuilder circleAttack;
    private bool still = false;
    private bool enraged = false;
    private bool weaponized = false;
    private bool finalForm = false;
    private int bulletCount = 0;
    private int phase = 1;

    public override void Start()
    {
        fireDataCross1 = new FireBuilder(crossPrefab, Field);
        fireDataCross1.From(transform);
        fireDataCross1.WithSpeed(12);
        fireDataCross1.WithModifier(new CircularBurstModifier(180, 2, 0, 0));

        fireDataCross2 = new FireBuilder(crossPrefab, Field);
        fireDataCross2.From(transform);
        fireDataCross2.WithSpeed(12);
        fireDataCross2.WithModifier(new CircularBurstModifier(180, 2, 0, 0));

        fireDataCross3 = new FireBuilder(crossPrefab, Field);
        fireDataCross3.From(transform);
        fireDataCross3.WithSpeed(12);
        fireDataCross3.WithModifier(new CircularBurstModifier(180, 2, 0, 0));

        fireDataCross4 = new FireBuilder(crossPrefab, Field);
        fireDataCross4.From(transform);
        fireDataCross4.WithSpeed(12);
        fireDataCross4.WithModifier(new CircularBurstModifier(180, 2, 0, 0));

        fireDataCircle = new FireBuilder(circlePrefab, Field);
        fireDataCircle.From(transform);
        fireDataCircle.WithSpeed(7 + 2 * Difficulty);
        fireDataCircle.WithModifier(new RandomizeAngleModifier(360));

        circleAttack = new FireBuilder(enragedPrefab, Field);
        circleAttack.From(transform);
        circleAttack.WithSpeed(5 + 2 * Difficulty);
        circleAttack.WithAngularSpeed(10);
        circleAttack.WithModifier(new CircularBurstModifier(360, new DynamicInt(13, 20) + 2 * Difficulty, 0, 0));
        circleAttack.WithController(new AutoDeactivateController(8f));

        SetRotation(90f);

        StartCoroutine(Attack());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!still && Vector3.Distance(transform.position, Vector3.zero) > 0.1)
        {
            Vector3 direction = Vector3.zero - transform.position;
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * 10;
        }
        else
        {
            AddRotation(0.5f + (weaponized ? 0.5f : 0f) + (finalForm ? 1f : 0f));
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
        int rotate = 0;
        int rotator = 2 + Difficulty;
        int rotateAccelerator = 1;
        while (true)
        {
            if (weaponized)
            {
                switch (phase)
                {
                    case 1:
                        if (enraged)
                        {
                            rotator = 4 + Difficulty;
                        }
                        if (finalForm)
                        {
                            fireDataCircle.Fire();
                        }
                        fireDataCircle.Fire();
                        if (bulletCount % 20 == 0 && enraged)
                        {
                            circleAttack.Fire();
                        }
                        rotate += rotator;
                        break;
                    case 2:
                        if (weaponized)
                        {
                            fireDataCross1.WithSpeed(6 + Difficulty);
                            fireDataCross2.WithSpeed(6 + Difficulty);
                            fireDataCross3.WithSpeed(6 + Difficulty);
                            fireDataCross4.WithSpeed(6 + Difficulty);
                            if (rotator >= 100)
                            {
                                rotateAccelerator = -1;
                            }
                            if (rotator <= -100)
                            {
                                rotateAccelerator = 1;
                            }
                            rotator += rotateAccelerator;
                            rotate += rotator;
                            if (bulletCount % ((enraged ? 15 : 20) - Difficulty) == 0)
                            {
                                circleAttack.Fire();
                            }
                            
                            if (finalForm)
                            {
                                fireDataCross3.WithRotation(rotate + 45);
                                fireDataCross4.WithRotation(rotate - 45);
                                fireDataCross3.Fire();
                                fireDataCross4.Fire();
                                fireDataCircle.Fire();
                            }
                        }
                        break;
                }
                fireDataCross1.WithRotation(rotate);
                fireDataCross2.WithRotation(rotate + 90);
                fireDataCross1.Fire();
                fireDataCross2.Fire();
                bulletCount++;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
