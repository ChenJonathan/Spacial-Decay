using UnityEngine;

public class CreditsText : MonoBehaviour
{
    public Vector2 StartLocation;
    public Vector2 TargetLocation;
    public float Delay;

    private float countdown;

    public void FixedUpdate()
    {
        if(countdown == 0)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, TargetLocation, 0.1f);
        }
        else
        {
            countdown -= Time.fixedDeltaTime;
            if(countdown < 0)
                countdown = 0;
        }
    }

    public void OnEnable()
    {
        transform.localPosition = StartLocation;
        countdown = Delay;
    }
}