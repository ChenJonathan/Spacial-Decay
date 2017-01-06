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
    private bool dragging;
    private Canvas menu;

    private readonly float CURSOR_SPEED = 0.3f;

    [SerializeField]
    private AudioClip onClickEffect;

    public void Start()
    {
        menu = Menu.Instance.GetComponent<Canvas>();

        Vector3 position = transform.position;
        switch(GameController.Singleton.Difficulty)
        {
            case 0:
                position.x = targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationEasy.x, LocationEasy.y, 17)).x;
                break;
            case 1:
                position.x = targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationMedium.x, LocationMedium.y, 17)).x;
                break;
            case 2:
                position.x = targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationHard.x, LocationHard.y, 17)).x;
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
        temp.a = Mathf.MoveTowards(temp.a, (!dragging && GameController.Singleton.Difficulty == 0) ? 1 : 0, Time.deltaTime);
        ImageEasy.color = temp;
        temp = ImageMedium.color;
        temp.a = Mathf.MoveTowards(temp.a, (!dragging && GameController.Singleton.Difficulty == 1) ? 1 : 0, Time.deltaTime);
        ImageMedium.color = temp;
        temp = ImageHard.color;
        temp.a = Mathf.MoveTowards(temp.a, (!dragging && GameController.Singleton.Difficulty == 2) ? 1 : 0, Time.deltaTime);
        ImageHard.color = temp;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        if(dragging)
        {
            if(Input.GetMouseButtonUp(0))
            {
                dragging = false;
                if(mousePosition.x < (LocationEasy.x + LocationMedium.x) / 2)
                    SetDifficulty(Difficulty.Easy);
                else if(mousePosition.x > (LocationMedium.x + LocationHard.x) / 2)
                    SetDifficulty(Difficulty.Hard);
                else
                    SetDifficulty(Difficulty.Medium);
            }
            else
            {
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(Mathf.Clamp(mousePosition.x, LocationEasy.x, LocationHard.x), LocationEasy.y, 17)).x;
            }
        }
        else if(Input.GetMouseButtonDown(0) && Mathf.Abs(mousePosition.y - LocationEasy.y) < 0.1f && mousePosition.x > LocationEasy.x - 0.05f && mousePosition.x < LocationHard.x + 0.05f)
        {
            dragging = true;
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        GameController.Singleton.Difficulty = (int)difficulty;
        switch(difficulty)
        {
            case Difficulty.Easy:
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationEasy.x, LocationEasy.y, 17)).x;
                break;
            case Difficulty.Medium:
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationMedium.x, LocationMedium.y, 17)).x;
                break;
            case Difficulty.Hard:
                targetX = Camera.main.ViewportToWorldPoint(new Vector3(LocationHard.x, LocationHard.y, 17)).x;
                break;
        }
        AudioSource.PlayClipAtPoint(onClickEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
    }
}