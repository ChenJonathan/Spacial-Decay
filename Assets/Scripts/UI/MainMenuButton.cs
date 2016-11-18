using UnityEngine;

/// <summary>
/// Adds button functionality to the main menu buttons.
/// </summary>
public class MainMenuButton : MonoBehaviour
{
    [SerializeField]
    private PlayerFake player;
    [SerializeField]
    private Vector2 target;
    [SerializeField]
    private string button;

    private Menu menu;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip onHoverEffect;
    [SerializeField]
    private AudioClip onClickEffect;

    public void Start()
    {
        menu = GetComponentInParent<Menu>();
    }

    public void OnMouseOver()
    {
        if(player.gameObject.activeSelf)
            player.SetForcedMoveTarget(true, target);

        if(Input.GetMouseButtonDown(0))
        {
            switch(menu.CurrentState)
            {
                case Menu.State.Main:
                    switch(button)
                    {
                        case "Play":
                            menu.SetState(Menu.State.LevelSelect);
                            break;
                        case "Options":
                            menu.SetState(Menu.State.Options);
                            break;
                        case "Credits":
                            menu.SetState(Menu.State.Credits);
                            break;
                        case "Exit":
                            Application.Quit();
                            break;
                    }
                    break;
                case Menu.State.Options:
                    menu.SetState(Menu.State.Main);
                    break;
                case Menu.State.Credits:
                    menu.SetState(Menu.State.Main);
                    break;
                case Menu.State.LevelSelect:
                    switch(button)
                    {
                        case "Back":
                            menu.SetState(Menu.State.Main);
                            break;
                    }
                    break;
            }
        }
    }

    public void OnMouseExit()
    {
        player.SetForcedMoveTarget(false);
    }
}