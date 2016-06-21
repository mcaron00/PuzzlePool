using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {

	private ShotCounter shotCounter;
	private MainMenu mainMenu;
	private GameOverPopup gameOverPopup;
	private GameWinPopup gameWinPopup;
	private OngoingIndicator ongoingIndicator;

	public void disableNext()
	{
		GameObject nextButton = GameObject.Find("GameWinNextButton");
		Button buttonComponent = nextButton.GetComponent<Button>();
		buttonComponent.interactable = false;

		// Tell player the last level has been won
		Text textdfield = GameObject.Find("GameWinMessage").GetComponent<Text>();
		textdfield.text = "All levels completed";
	}

	public void displayGameOver(string reason)
	{
		// Display reason for game over
		Text textdfield = GameObject.Find("GameOverReason").GetComponent<Text>();

		switch (reason)
		{
		case "shots":
			textdfield.text = "Too many shots taken";
			break;
		case "ball":
			textdfield.text = "Right ball, wrong pocket";
			break;
		}

		// Move game over popup to middle of screen
		gameOverPopup.GetComponent<RectTransform>().localPosition = new Vector2(0,0);
	}

	public void displayGameWin()
	{
		// Move game win popup to middle of screen
		gameWinPopup.GetComponent<RectTransform>().localPosition = new Vector2(0,0);

		// Erase game win message altogether
		Text textdfield = GameObject.Find("GameWinMessage").GetComponent<Text>();
		textdfield.text = "";
	}

	public void hideMainMenu()
	{
		mainMenu.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
	}

	// Use this for initialization
	/*void Start () {
		// Find game over popup and game win popup


		//resetUi();
	}*/

	public void setOngoing(bool state)
	{
		ongoingIndicator.gameObject.SetActive(state);
	}

	public void showMainMenu()
	{
		// Move game over pop up and game win popup out of the way
		gameOverPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
		gameWinPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
		mainMenu.GetComponent<RectTransform>().localPosition = new Vector2(0,0);

		mainMenu.updateLevelButtons();


	}

	void Start()
	{
		ongoingIndicator = GameObject.Find("OngoingIndicator").GetComponent<OngoingIndicator>();

		// Hide the ongoing shot indicator
		setOngoing(false);
	}


	public void startGame()
	{
		//Debug.Log ("Resert UI");

		// Init ui elements if not done yet
		if(gameOverPopup == null  || gameWinPopup == null || mainMenu == null)
		{
			gameOverPopup = GameObject.Find("GameOverPopup").GetComponent<GameOverPopup>();
			gameWinPopup = GameObject.Find("GameWinPopup").GetComponent<GameWinPopup>();
			mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
			//mainMenu = GameObject.Find("MainMenu");

		}

		// Move game over pop up and game win popup out of the way
		gameOverPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
		gameWinPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
		mainMenu.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);



	}

	// Update is called once per frame
	/*void Update () {
	
	}*/

	public void updateShots(string passedString)
	{
		// Initialize shot counter if not done yet
		if(shotCounter == null)
		{
			shotCounter = GameObject.Find("ShotCounter").GetComponent<ShotCounter>();
		}
		shotCounter.updateShotCounter (passedString);
	}
}
