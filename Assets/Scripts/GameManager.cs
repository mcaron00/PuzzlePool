﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public string[] levels = new string[2];
	public int lastLevelWon;

	private float customGravity = 12.0f;
	private List<Ball_Generic> balls;
	private List<PocketDetector> pockets;
	private int goodBalls;
	private int shotCount;
	private int maxShotCount = 8;
	private bool isShotOngoing;
	private UiManager uiManager;
	private bool isGameOver;
	private int currentLevel;


	public void addGoodBall(GameObject ballInPocket)
	{
		goodBalls++;
		checkVictory();

		// Destroy ball in the pocket
		balls.Remove (ballInPocket.GetComponent<Ball_Generic>());
		Object.Destroy (ballInPocket);

		// Report a stopped ball
		reportStopped();
	}

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		//Debug.Log ("Game Manager Ready");
		balls = new List<Ball_Generic>();

		pockets = new List<PocketDetector>();

		// Identify Ui Manager
		uiManager = GetComponent<UiManager>();

		// Build array of level names
		levels[0] = "Pul_TestLevel01_prefabs";
		levels[1] = "Pul_TestLevel02";

		// Set all levels as "not won"
		lastLevelWon = 0;
	}

	public bool checkShotOngoing(){
		// Report shot disallowed if game is over
		if (isGameOver == true) return false;

		// Report false if any ball is rolling
		return isShotOngoing;
	}

	void checkVictory()
	{
		//Debug.Log ("good balls: " + goodBalls);
		//Debug.Log ("pockets: " + pockets.Count);

		if(goodBalls >= pockets.Count)
		{
			doGameWon();
		}
	}

	void doGameWon()
	{
		// Record that the level was conquered
		lastLevelWon = currentLevel;

		// Ignore if game already over
		if(isGameOver == true) return;

		isGameOver = true;
		//Debug.Log ("************* VICTORY!!!");

		// Display game win popup
		uiManager.displayGameWin();

		// Disable "next" button in case the last level has been completed
		if(currentLevel >= levels.Length)
		{
			//Debug.Log ("last level reached");
			uiManager.disableNext();
		}
	}

	void doGameOver(string reason)
	{
		// Ignore if game already over
		if(isGameOver == true) return;

		isGameOver = true;
		//Debug.Log ("************* FAIL!!!");

		// Display game over popup
		uiManager.displayGameOver(reason);
	}

	void inBetweenShots()
	{

		// Stop controlling velocity
		foreach (Ball_Generic gameObj in balls)
		{
			gameObj.stopVelocityControl();
		}

		// Check if game over
		if(shotCount >= maxShotCount)doGameOver("shots");
	}

	public void launchLevel(int levelNumber)
	{
		//Debug.Log ("Launch Level #" + levelNumber);

		// Record level
		currentLevel = levelNumber;

		// Clear lists
		balls.Clear ();
		pockets.Clear ();

		// Reset phase trackers
		isShotOngoing = false;
		goodBalls = 0;
		shotCount = 0;
		isGameOver = false;

		// Update HUD
		updateHud();
		
		// Destroy previously loaded level
		GameObject sceneMasterNode = GameObject.Find ("SceneMaster");
		Object.Destroy (sceneMasterNode);
		
		// Set UI for game
		uiManager.startGame ();
		
		// Load first level
		Application.LoadLevelAdditive(levels[currentLevel - 1]);
	}

	public void listBall (Ball_Generic reportedBall)
	{
		balls.Add(reportedBall);

		//Debug.Log ("Reported balls: " + balls.Count);
	}

	public void listPocket (PocketDetector reportedPocket)
	{
		pockets.Add(reportedPocket);
		
		//Debug.Log ("Reported pocekts: " + pockets.Count);
	}

	public void mainMenuButtonPressed()
	{
		uiManager.showMainMenu ();

	}
	
	public void nextButtonPressed()
	{
		// Increment level index
		currentLevel++;

		// ... And load!
		launchLevel(currentLevel);
	}

	public void replayButtonPressed()
	{
		// Load current level again
		//loadCurrentLevel ();
		launchLevel(currentLevel);
	}

	public void reportBadBall()
	{
		doGameOver("ball");
	}

	public void reportGameUi(UiManager passedGameUi)
	{
		uiManager = passedGameUi;
	}

	public void reportShot(){
		// This function is called when a shot is taken to tell the manager balls are now rolling

		// Update shot count
		shotCount++;
		updateHud();

		isShotOngoing = true;
	}

	public void reportStopped()
	{
		// We don't care if game is over
		if(isGameOver == true) return;

		// On ball just stopped. Check if all balls are stopped
		int stoppedBalls = 0;
		for(int i = 0; i < balls.Count; i++)
		{
			if (balls[i].checkRolling() == false)stoppedBalls++;
		}

		// If they are all stopped, start "in between shots" phase
		if (stoppedBalls >= balls.Count)
		{
			//Debug.Log ("All balls stopped");
			isShotOngoing = false;
			inBetweenShots();
		}
	}

	// Use this for initialization
	void Start () {

		// Tweak gravity settings
		Physics.gravity = new Vector3(0, -customGravity, 0);


	}
	
	// Update is called once per frame
	/*void Update () {

	}*/

	void updateHud()
	{
		// update shot counter
		uiManager.updateShots(shotCount + "/" + maxShotCount);
	}
}
