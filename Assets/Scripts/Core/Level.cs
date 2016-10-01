using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public string Scene;
    public List<Level> Unlocks;

    private SpriteRenderer sprite;
    private LineRenderer line;
    private bool clickable;

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        clickable = true;
    }

    public void Appear()
    {
        clickable = false;
        StartCoroutine(Appear(1.5f));
    }

    private IEnumerator Appear(float duration)
    {
        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;
        line.SetColors(color, color);

        for(int i = 1; i <= 100; i++)
        {
            yield return new WaitForSeconds(duration / 100);

            // Increase alpha
            color.a = i / 100.0f;
            sprite.color = color;
            line.SetColors(color, color);
        }

        clickable = true;
    }

    public void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0) && clickable)
        {
            GameController.Singleton.LoadLevel(this);
        }
    }
}