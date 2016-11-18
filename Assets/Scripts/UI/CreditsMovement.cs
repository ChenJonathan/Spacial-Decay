using UnityEngine;
using System.Collections;

public class CreditsMovement : MonoBehaviour {

    [SerializeField]
    private GameObject text;

    private Transform start, end;

	// Use this for initialization
	void Start () {
        start = new Vector3(36,-430,0);
        end = new Vector3(36, 430, 0);
       
	}

    public void Move()
    {
        text.transform.position = Vector3.Lerp(start, end, 0.2f);
    }

    public void Stop()
    {
        text.transform.position = new Vector3(text.transform.position.x, text.transform.position.y, text.transform.position.z);
    }
}
