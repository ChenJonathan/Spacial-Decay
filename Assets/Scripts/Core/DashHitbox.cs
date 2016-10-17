using UnityEngine;

/// <summary>
/// Hitbox for the player for player-enemy collisions.
/// </summary>
public class DashHitbox : MonoBehaviour
{
    private Player player;

    /// <summary>
    /// Called upon instantiation.
    /// </summary>
    public void Awake()
    {
        player = transform.GetComponentInParent<Player>();
    }

    /// <summary>
    /// Called when the hitbox first collides with an object. Handles collision with enemies.
    /// </summary>
    /// <param name="collider">The collider that the player collided with</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            if(player.IsDashing)
            {
                enemy.Damage(50);
            }
            else if(!player.IsInvincible)
            {
                player.Hit();
            }
        }
    }

    /// <summary>
    /// Called repeatedly when the hitbox continues to collide with an object. Handles collision with enemies.
    /// </summary>
    /// <param name="collider">The collider that the player collided with</param>
    private void OnTriggerStay2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if(enemy != null && !player.IsDashing && !player.IsInvincible)
        {
            player.Hit();
        }
    }
}
