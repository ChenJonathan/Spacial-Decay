using System.Collections;
using UnityEngine;

/// <summary>
/// A space probe that travels to a level, unlocking it.
/// </summary>
public class Probe : MonoBehaviour
{
    private SpriteRenderer sprite;

    private Level destination; // Destination of the probe
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
        destination = level;
        totalDistance = (level.transform.position - transform.position).magnitude;
        StartCoroutine(Travel());
    }

    /// <summary>
    /// Coroutine to move the probe towards its destination.
    /// </summary>
    public IEnumerator Travel()
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
                transform.position = Vector3.MoveTowards(transform.position, destination.transform.position, totalDistance / 5);
                direction = 1;

                yield return new WaitForSeconds(0.1f);
                if(transform.position == destination.transform.position)
                {
                    if(destination.gameObject.activeSelf)
                    {
                        destination.LineAppear();
                    }
                    else
                    {
                        destination.gameObject.SetActive(true);
                        destination.Appear();
                    }
                    Destroy(gameObject);
                    yield break;
                }
            }
            else if(color.a >= 1)
            {
                direction = -1;
            }
        }
    }
    /// <summary>
    /// Ensures that levels are added if the level select is exited early.
    /// </summary>
    public void OnDestroy()
    {
        if(destination != null && !GameController.Singleton.Levels[destination.Scene].Unlocked)
        {
            GameController.LevelData levelData = GameController.Singleton.Levels[destination.Scene];
            levelData.Unlocked = true;
            GameController.Singleton.Levels[destination.Scene] = levelData;
        }
    }
}