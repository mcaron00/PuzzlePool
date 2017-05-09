using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuLevelButton : MonoBehaviour {

	public int levelNumber;

	private Button buttonComponent;

	public void launchLevel()
	{
		GameManager.instance.launchLevel (levelNumber);
	}

	// Use this for initialization
	void Start () {
		// Init button component
		buttonComponent = GetComponent<Button>();

		// replace placeholder text by level number

		Text textfield = buttonComponent.GetComponentInChildren<Text>();
		textfield.text = levelNumber.ToString();

		// Destroy self if level number above the actual number of levels
		if(GameManager.instance.levels.Length < levelNumber)
		{
			Destroy(gameObject);
		
		
		// Update button states for non-destroyed ones
		}else{
			// Update button state
			updateButton();
		}



	}

	public void updateButton()
	{
		// Disable buttons for levels not yet ready to be tried
		if(levelNumber > GameManager.instance.lastLevelWon + 1)
		{
			buttonComponent.interactable = false;
		}else{
			buttonComponent.interactable = true;
		}

		//Debug.Log ("Button # " + levelNumber + "... last level won is " + GameManager.instance.lastLevelWon + " ... to be " + buttonComponent.interactable);
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
}
