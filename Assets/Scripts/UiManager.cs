using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {

	private ShotCounter shotCounter;
	private MainMenu mainMenu;
	private GameOverPopup gameOverPopup;
	private GameWinPopup gameWinPopup;
	private OngoingIndicator ongoingIndicator;
	private PauseMenu pauseMenu;
	private Button pauseButton;
	private GameManager gameMan;

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
		pauseButton.gameObject.SetActive(false);

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

		pauseButton.gameObject.SetActive(false);

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

	public void init()
	{
		// Init ui elements if not done yet
		if(gameOverPopup == null  || gameWinPopup == null || mainMenu == null)
		{
			gameMan = GameObject.Find("GameManager").GetComponent<GameManager>();
			gameOverPopup = GameObject.Find("GameOverPopup").GetComponent<GameOverPopup>();
			gameWinPopup = GameObject.Find("GameWinPopup").GetComponent<GameWinPopup>();
			mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
			pauseMenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
			pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
			//mainMenu = GameObject.Find("MainMenu");

		}
	}

	public void mainMenuButtonPressed()
	{
		// in case the pause menu is on screen, make it go away
		pauseMenu.GetComponent<RectTransform>().localPosition = new Vector2(2000,0);

		showMainMenu ();

	}

	public void pauseButtonPressed()
	{
		//Debug.Log ("Pause menu called");
		gameMan.setInput (false);
		pauseMenu.GetComponent<RectTransform>().localPosition = new Vector2(0,0);

		pauseButton.gameObject.SetActive(false);
	}

	public void resumeButtonPressed()
	{
		pauseMenu.GetComponent<RectTransform>().localPosition = new Vector2(2000,0);
		pauseButton.gameObject.SetActive(true);
		gameMan.setInput (true);
	}

	public void restartButtonPressed()
	{
		gameMan.replay();
		pauseMenu.GetComponent<RectTransform>().localPosition = new Vector2(2000,0);
	}

	public void setOngoing(bool state)
	{
		ongoingIndicator.gameObject.SetActive(state);
		//pauseButton.SetActive(!state);
		pauseButton.gameObject.SetActive(!state);
	}

	public void showMainMenu()
	{

		Debug.Log ("Show main menu called");

		pauseButton.gameObject.SetActive(false);

		mainMenu.GetComponent<RectTransform>().localPosition = new Vector2(0,0);

		mainMenu.updateLevelButtons();

		// Move game over pop up and game win popup out of the way
		gameOverPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
		gameWinPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);



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
