using UnityEngine;

public class DashHitbox : MonoBehaviour
{
    private Player player;

    public void Awake()
    {
        player = transform.GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D collider)
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

    void OnTriggerStay2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if(enemy != null && !player.IsDashing && !player.IsInvincible)
        {
            player.Hit();
        }
    }
}
