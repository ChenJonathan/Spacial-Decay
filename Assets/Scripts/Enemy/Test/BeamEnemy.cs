using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Controllers;

public class BeamEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(0);
        fireData.WithController(new AutoDeactivateController(2.0f));

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
            fireData.Towards(Player.transform.position);
            FacePlayer = false;

            // Fire
            yield return new WaitForSeconds(0.5f);
            fireData.Fire();
            yield return new WaitForSeconds(2.0f);
        }
        Die();
    }
}