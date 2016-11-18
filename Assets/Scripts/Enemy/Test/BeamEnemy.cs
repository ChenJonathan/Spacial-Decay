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

    private Danmaku currentWarning;
    private Danmaku currentBeam;

    /// <summary> The horizontal speed of the enemy. </summary>
    private float speed = -5;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        warningData = new FireBuilder(warningPrefab, Field);
        warningData.From(transform);
        warningData.WithSpeed(0);
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
        fireData.WithController(new EnemyDeathController(this));

        if (parameters.Length > 0) {
            speed *= parameters[0];
        }

        base.Start();
    }

    protected override IEnumerator Run()
    {
        for (int i = 0; i < 4; i++)
        {
            // Moving left
            FacePlayer = true;
            rigidbody2d.velocity = new Vector2(speed, 0);
            yield return new WaitForSeconds(2);

            // Stop and face player
            rigidbody2d.velocity = Vector2.zero;
            warningData.Towards(Player.transform.position);
            fireData.Towards(Player.transform.position);
            FacePlayer = false;
            RotateTowards(Player.transform.position);

            // Fire
            currentWarning = warningData.Fire();
            yield return new WaitForSeconds(1f - .25f * Difficulty);
            currentWarning.DeactivateImmediate();
            currentBeam = fireData.Fire();
            yield return new WaitForSeconds(2f);
            currentBeam.Deactivate();
        }
        Die();
    }

    /// <summary>
    /// Updates the position of the beam if the enemy moves.
    /// </summary>
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (currentWarning != null && currentWarning.IsActive)
            currentWarning.position = transform.position;
        if (currentBeam != null && currentBeam.IsActive)
            currentBeam.position = transform.position;
    }
}