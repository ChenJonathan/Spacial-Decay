using UnityEngine;
using System.Collections;

public class SmoothJitter : MonoBehaviour
{
    private Vector3 position;
    private Vector3 rotation;
    private float seed;

    public Vector3 positionWobble;
    public Vector3 rotationWobble;
    public float speed;

    void Awake()
    {
        position = transform.position;
        rotation = transform.rotation.eulerAngles;
        seed = position.z;
    }

    void Update()
    {
        float t = Time.time * speed;
        Vector3 p = new Vector3(Mathf.Sin(seed + 09 * t), Mathf.Cos(seed + 11 * t), Mathf.Sin(seed + 13 * t));
        Vector3 r = new Vector3(Mathf.Cos(seed + 11 * t), Mathf.Sin(seed + 13 * t), Mathf.Cos(seed + 15 * t));

        transform.position = position + Vector3.Scale(positionWobble, p);
        transform.rotation = Quaternion.Euler(rotation + Vector3.Scale(rotationWobble, r));
    }
}