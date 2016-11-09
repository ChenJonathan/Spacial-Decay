using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Controllers;

public class BeamEnemy : Enemy
{
    public DanmakuPrefab warningPrefab;
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private FireBuilder warningData;
    private Rigidbody2D rigidbody2d;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        warningData = new FireBuilder(warningPrefab, Field);
        warningData.From(transform);
        warningData.WithSpeed(0);
        warningData.WithController(new AutoDeactivateController(1.0f));
        warningData.WithController(new EnemyDeathController(this));
        ColorChangeController colorController = new ColorChangeController();
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        GradientColorKey[] gck = new GradientColorKey[2];
        gak[0].alpha = 0.0f;
        gak[0].time = 0f;
        gak[1].alpha = 1f;
        gak[1].time = 1f;
        gck[0].color = Color.red;
        gck[0].time = 0f;
        gck[1].color = Color.yellow;
        gck[1].time = 1f;
        colorController.ColorGradient = new Gradient();
        colorController.ColorGradient.SetKeys(gck, gak);
        colorController.StartTime = 0;
        colorController.EndTime = 1;
        warningData.WithController(colorController);

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(0);
        fireData.WithController(new AutoDeactivateController(2.0f));
        fireData.WithController(new EnemyDeathController(this));

        base.Start();
    }

    protected override IEnumerator Run()
    {
        for (int i = 0; i < 4; i++)
        {
            // Moving left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(-5, 0);
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            warningData.Towards(Player.transform.position);
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            warningData.Fire();
            yield return new WaitForSeconds(1f);
            fireData.Fire();
            yield return new WaitForSeconds(2.0f);
        }
        Die();
    }
}