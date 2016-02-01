using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {


	public void updateLevelButtons()
	{
		// Update main menu button states

		//GetComponentInChildren<MainMenuLevelButton>().updateButton();

		MainMenuLevelButton[] levelButtons = GetComponentsInChildren<MainMenuLevelButton>();
		
		foreach(MainMenuLevelButton thisButton in levelButtons)
		{
			thisButton.updateButton();
		}
	}

	// Use this for initialization
	/*void Start () {

	}*/
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
}
