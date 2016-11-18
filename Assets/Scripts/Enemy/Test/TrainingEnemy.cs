using UnityEngine;
using System.Collections;
using DanmakU;

public class TrainingEnemy : Enemy {

    public DanmakuPrefab bulletPrefab;

    public bool enableFire = false;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;

    public override void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(6);

        base.Start();
    }

    protected override IEnumerator Run()
    {
        // Moving left
        FacePlayer = true;
        rigidbody2d.velocity = new Vector2(-6, 0);
        yield return new WaitForSeconds(2);

        // Stop and face player
        rigidbody2d.velocity = Vector2.zero;
        FacePlayer = false;

        // Fire
        if (enableFire)
        {
            while (true)
            {
                fireData.Towards(Player.transform.position);
                fireData.Fire();
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
