using DanmakU;
using UnityEngine;

public class Menu : Singleton<Menu>
{
    public PlayerFake Player;
    public GameObject Levels;

    private GameObject mainMenu;
    private GameObject optionsMenu;
    private GameObject creditsMenu;
    private GameObject levelSelectMenu;

    public enum State { Main, Options, Credits, LevelSelect };

    private static State currentState = State.Main;
    public State CurrentState
    {
        get { return currentState; }
    }

    [HideInInspector]
    public bool StateChanged = false; // Hacky fix for update order

    public override void Awake()
    {
        base.Awake();

        mainMenu = transform.FindChild("Main").gameObject;
        optionsMenu = transform.FindChild("Options").gameObject;
        creditsMenu = transform.FindChild("Credits").gameObject;
        levelSelectMenu = transform.FindChild("Level Select").gameObject;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && CurrentState != State.Main && Level.Clickable)
        {
            Camera.main.GetComponent<Scroll>().Zoom = false;
            SetState(State.Main);
        }
    }

    public void SetState(State state)
    {
        if(state != State.LevelSelect)
            Player.gameObject.SetActive(true);

        mainMenu.SetActive(state == State.Main);
        optionsMenu.SetActive(state == State.Options);
        creditsMenu.SetActive(state == State.Credits);
        levelSelectMenu.SetActive(state == State.LevelSelect);
        Levels.SetActive(state == State.LevelSelect);
        currentState = state;
    }
}