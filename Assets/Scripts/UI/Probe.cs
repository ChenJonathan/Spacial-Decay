using System.Collections;
using UnityEngine;

/// <summary>
/// A space probe that travels to a level, unlocking it.
/// </summary>
public class Probe : MonoBehaviour
{
    private SpriteRenderer sprite;
    private float totalDistance; // Total distance between start location and destination

    /// <summary>
    /// Called when the probe is instantiated.
    /// </summary>
    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Sets the probe's destination. Call this immediately after instantiating the probe.
    /// </summary>
    /// <param name="level">The level object to travel towards</param>
    public void SetDestination(Level level)
    {
        totalDistance = (level.transform.position - transform.position).magnitude;
        StartCoroutine(Travel(level));
    }

    /// <summary>
    /// Coroutine to move the probe towards its destination.
    /// </summary>
    /// <param name="warning">The level object to travel towards</param>
    public IEnumerator Travel(Level level)
    {
        int direction = 1;
        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;
        
        while(true)
        {
            yield return new WaitForSeconds(0.005f);
            
            color.a += 0.1f * direction;
            sprite.color = color;

            if(color.a <= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, level.transform.position, totalDistance / 6);
                direction = 1;

                if(transform.position == level.transform.position)
                {
                    yield return new WaitForSeconds(0.5f);

                    level.gameObject.SetActive(true);
                    level.Appear();
                    Destroy(gameObject);
                    yield break;
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if(color.a >= 1)
            {
                direction = -1;
            }
        }
    }
}