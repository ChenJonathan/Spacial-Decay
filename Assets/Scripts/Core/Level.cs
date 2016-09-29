using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public string Scene;
    public Probe ProbePrefab;
    public List<Level> Unlocks;

    private SpriteRenderer sprite;

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void OnEnable()
    {
        StartCoroutine(Appear(1));
    }

    public IEnumerator Appear(float duration)
    {
        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;

        for(int i = 1; i <= 100; i++)
        {
            yield return new WaitForSeconds(duration / 100);

            // Increase alpha
            color.a = i / 100.0f;
            sprite.color = color;
        }

        // Send probe to new levels
        if(GameController.Singleton.CurrentLevel == this)
        {
            foreach(Level level in Unlocks)
            {
                if(level.gameObject.activeInHierarchy == false)
                {
                    Probe clone = ((Probe)Instantiate(ProbePrefab, transform.position, Quaternion.identity));
                    clone.SetDestination(level);
                    clone.transform.parent = transform;
                }
            }
            GameController.Singleton.CurrentLevel = null;
        }
    }
}