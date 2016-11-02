using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    private float invulnDuration = 0;
    private static readonly float MAX_INVULN_DURATION = 0.5f;

	public void Update()
    {
        invulnDuration = Mathf.Max(invulnDuration - Time.deltaTime, 0);
	}

    public bool IsActive()
    {
        return invulnDuration > 0;
    }

    /// <summary>
    /// Called repeatedly when the shield continues to collide with an object. Marks the shield as active if the shield collided with the player.
    /// </summary>
    /// <param name="collider">The collider that the player collided with</param>
    private void OnTriggerStay2D(Collider2D collider)
    {
        Player player = collider.gameObject.GetComponent<Player>();
        if(player != null)
        {
            invulnDuration = MAX_INVULN_DURATION;
        }
    }
}
