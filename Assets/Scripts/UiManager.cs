using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	
	private ShotCounter shotCounter;
	private GameOverPopup gameOverPopup;
	private GameWinPopup gameWinPopup;

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
	}

	public void resetUi()
	{
		Debug.Log ("Resert UI");

		// Init ui elements if not done yet
		if(gameOverPopup == null  || gameWinPopup == null)
		{
			gameOverPopup = GameObject.Find("GameOverPopup").GetComponent<GameOverPopup>();
			gameWinPopup = GameObject.Find("GameWinPopup").GetComponent<GameWinPopup>();
		}

		// Move game over pop up and game win popup out of the way
		gameOverPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
		gameWinPopup.GetComponent<RectTransform>().localPosition = new Vector2(1000,0);
	}

	// Use this for initialization
	void Start () {
		// Find game over popup and game win popup


		//resetUi();
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
