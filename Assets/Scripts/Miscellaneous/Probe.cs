using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour
{
    private SpriteRenderer sprite;

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetDestination(Level level)
    {
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
            yield return new WaitForSeconds(0.01f);
            
            color.a += 0.05f * direction;
            sprite.color = color;

            if(color.a <= 0)
            {
                yield return new WaitForSeconds(0.1f);

                transform.position = Vector3.MoveTowards(transform.position, level.transform.position, 0.5f);
                direction = 1;
            }
            else if(color.a >= 1)
            {
                if(transform.position == level.transform.position)
                {
                    level.gameObject.SetActive(true);
                    Destroy(gameObject);
                    yield break;
                }
                direction = -1;
            }
        }
    }
}