using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
//using DanmakU;

namespace Assets.Scripts.UI {
	public class MainMenu : MonoBehaviour {
		[SerializeField]
		private GameObject mainParent;
		[SerializeField]
		private GameObject LevelParent;
        [SerializeField]
        private GameObject lvlback;
		[SerializeField]
		private GameObject settingsParent;
		[SerializeField]
		private GameObject creditsParent;

		private bool inMain;
		private bool inSettings;

		// Use this for initialization
		void Start () {
			inMain = true;
			inSettings = true;
		}
		
		// Update is called once per frame
		void Update () {
			if (inMain) {
				if (Input.GetKey (KeyCode.Escape))
					Application.Quit ();
			} else {
				if (Input.GetKey (KeyCode.Escape))
					GoToMain ();
			}
        }

		public void GoToMain()
		{
            inMain = true;
            LevelParent.transform.position = new Vector3(LevelParent.transform.position.x, LevelParent.transform.position.y, 0);
            lvlback.SetActive(false);
			mainParent.SetActive(true);

			settingsParent.SetActive(false);
			creditsParent.SetActive(false);
		}

		public void GoToCredits()
		{
            inMain = false;
            LevelParent.transform.position = new Vector3(LevelParent.transform.position.x, LevelParent.transform.position.y, 0);
            mainParent.SetActive(false);
			settingsParent.SetActive(false);
			creditsParent.SetActive(true);
		}

		public void GoToLevelSelect()
		{
			inMain = false;
            LevelParent.transform.position = new Vector3(LevelParent.transform.position.x, LevelParent.transform.position.y, 25);
            lvlback.SetActive(true);
			mainParent.SetActive(false);
			settingsParent.SetActive(false);
			creditsParent.SetActive(false);
		}

		public void GoToSettings()
		{
			inMain = false;
            LevelParent.transform.position = new Vector3(LevelParent.transform.position.x, LevelParent.transform.position.y, 0);
            mainParent.SetActive(false);
			settingsParent.SetActive(true);
			creditsParent.SetActive(false);

		}

		public void Quit ()
		{
            Application.Quit();
        }

        public void setCurrentSelected(GameObject obj)
        {
            EventSystem.current.SetSelectedGameObject(obj);
        }
	}
}
