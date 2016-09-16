using UnityEngine;

public class Warning : MonoBehaviour
{
    private float duration;
    public float Duration
    {
        get { return duration; }
        set { duration = value; }
    }
    private float time = 0;

    public void Update()
    {
        time += Time.deltaTime;
        if(time > duration)
            Destroy(gameObject);
    }
}