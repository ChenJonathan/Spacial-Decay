using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
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

    public void Awake()
    {
        mainMenu = transform.FindChild("Main").gameObject;
        optionsMenu = transform.FindChild("Options").gameObject;
        creditsMenu = transform.FindChild("Credits").gameObject;
        levelSelectMenu = transform.FindChild("Level Select").gameObject;

        SetState(State.Main);
    }

    public void SetState(State state)
    {
        if(Player.gameObject.activeSelf)
            Player.SetForcedMoveTarget(false);

        mainMenu.SetActive(state == State.Main);
        optionsMenu.SetActive(state == State.Options);
        creditsMenu.SetActive(state == State.Credits);
        levelSelectMenu.SetActive(state == State.LevelSelect);
        Levels.SetActive(state == State.LevelSelect);
        currentState = state;
    }
}