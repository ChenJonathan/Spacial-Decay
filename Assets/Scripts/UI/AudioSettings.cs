using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour {
    [SerializeField]
    private Slider vol;
    [SerializeField]
    private Slider music;
    [SerializeField]
    private GameController gc;

    void Start()
    {
        vol.value = gc.sfxVol;
        music.value = gc.musicVol;
    }

    public void changeVol ()
    {
        gc.sfxVol = vol.value;

    }

    public void changeMusic ()
    {
        gc.musicVol = music.value;
    }

	
}
