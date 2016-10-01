using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour
{
    private SpriteRenderer sprite;
    private float distance;

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetDestination(Level level)
    {
        distance = (level.transform.position - transform.position).magnitude;
        StartCoroutine(Travel(level));
    }

    public IEnumerator Travel(Level level)
    {
        int direction = 1;
        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;
        
        while(true)
        {
            yield return new WaitForSeconds(0.005f);
            
            color.a += 0.05f * direction;
            sprite.color = color;

            if(color.a <= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, level.transform.position, distance / 7);
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