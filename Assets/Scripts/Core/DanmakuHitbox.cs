using DanmakU;
using UnityEngine;

/// <summary>
/// Hitbox for the player for player-Danmaku collisions.
/// </summary>
public class DanmakuHitbox : DanmakuCollider
{
    private Player player;

    /// <summary>
    /// Called upon instantiation.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        TagFilter = "Bullet|Piercing";
        player = transform.GetComponentInParent<Player>();
    }

    /// <summary>
    /// Called when the player collides with a bullet.
    /// </summary>
    /// <param name="danmaku">The bullet that the player collided with</param>
    /// <param name="info">Information about the collision</param>
    protected override void DanmakuCollision(Danmaku danmaku, RaycastHit2D info)
    {
        if(danmaku.Tag != "Piercing")
            danmaku.Deactivate();
        if(!player.IsDashing && !player.IsInvincible)
            player.Hit();
    }
}
