using UnityEngine;

/// <summary>
/// Adds button functionality to the menu buttons.
/// </summary>
public class MenuButton : MonoBehaviour
{
    [SerializeField]
    private PlayerFake player;
    [SerializeField]
    private Vector2 target;
    [SerializeField]
    private string button;

    private Menu menu;
    
    [SerializeField]
    private AudioClip onHoverEffect;
    [SerializeField]
    private AudioClip onClickEffect;

    public void Awake()
    {
        menu = GetComponentInParent<Menu>();
    }

    private void OnMouseEnter()
    {
        AudioSource.PlayClipAtPoint(onHoverEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
    }

    public void OnMouseOver()
    {
        if(player.gameObject.activeSelf)
            player.SetForcedMoveTarget(true, target);

        if(Input.GetMouseButtonDown(0))
        {
            menu.StateChanged = true;

            AudioSource.PlayClipAtPoint(onClickEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);

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
                        case "Endless":
                            GameController.Singleton.LoadLevel("Endless");
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