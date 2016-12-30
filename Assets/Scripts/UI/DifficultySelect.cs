using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows the player to choose the difficulty
/// </summary>
public class DifficultySelect : MonoBehaviour
{
    public Vector2 LocationEasy;
    public Vector2 LocationMedium;
    public Vector2 LocationHard;

    public Image ImageEasy;
    public Image ImageMedium;
    public Image ImageHard;

    private float targetX;
    private readonly float CURSOR_SPEED = 0.3f;
    
    [SerializeField]
    private AudioClip onClickEffect;

    public void Start()
    {
        Vector3 position = transform.position;
        switch(GameController.Singleton.Difficulty)
        {
            case 0:
                position.x = targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationEasy.x, LocationEasy.y, 13.2f)).x;
                break;
            case 1:
                position.x = targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationMedium.x, LocationMedium.y, 13.2f)).x;
                break;
            case 2:
                position.x = targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationHard.x, LocationHard.y, 13.2f)).x;
                break;
        }
        transform.position = position;
    }

    public void Update()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Lerp(position.x, targetX, CURSOR_SPEED);
        transform.position = position;

        Color temp = ImageEasy.color;
        temp.a = Mathf.MoveTowards(temp.a, GameController.Singleton.Difficulty == 0 ? 1 : 0, Time.deltaTime);
        ImageEasy.color = temp;
        temp = ImageMedium.color;
        temp.a = Mathf.MoveTowards(temp.a, GameController.Singleton.Difficulty == 1 ? 1 : 0, Time.deltaTime);
        ImageMedium.color = temp;
        temp = ImageHard.color;
        temp.a = Mathf.MoveTowards(temp.a, GameController.Singleton.Difficulty == 2 ? 1 : 0, Time.deltaTime);
        ImageHard.color = temp;

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            if(Vector3.Distance(mousePosition, LocationEasy) < 0.1f)
            {
                GameController.Singleton.Difficulty = (int)Difficulty.Easy;
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationEasy.x, LocationEasy.y, 13.2f)).x;
                AudioSource.PlayClipAtPoint(onClickEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
            }
            else if(Vector3.Distance(mousePosition, LocationMedium) < 0.1f)
            {
                GameController.Singleton.Difficulty = (int)Difficulty.Medium;
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationMedium.x, LocationMedium.y, 13.2f)).x;
                AudioSource.PlayClipAtPoint(onClickEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
            }
            else if(Vector3.Distance(mousePosition, LocationHard) < 0.1f)
            {
                GameController.Singleton.Difficulty = (int)Difficulty.Hard;
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationHard.x, LocationHard.y, 13.2f)).x;
                AudioSource.PlayClipAtPoint(onClickEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
            }
        }
    }
}