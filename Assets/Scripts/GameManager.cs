using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	private float customGravity = 12.0f;
	private List<Ball_Generic> balls;
	private List<PocketDetector> pockets;
	private int goodBalls;
	private int shotCount;
	private int maxShotCount = 4;
	private bool isShotOngoing;
	private UiManager uiManager;
	private bool isGameOver;

	public void addGoodBall()
	{
		goodBalls++;
		checkVictory();
	}

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		//Debug.Log ("Game Manager Ready");
		balls = new List<Ball_Generic>();
		balls.Clear ();

		pockets = new List<PocketDetector>();
		pockets.Clear ();
		isShotOngoing = false;

		// Identify Ui Manager
		uiManager = GetComponent<UiManager>();
	}

	public bool checkShotOngoing(){
		// Report shot disallowed if game is over
		if (isGameOver == true) return false;

		// Report false if any ball is rolling
		return isShotOngoing;
	}

	void checkVictory()
	{
		Debug.Log ("good balls: " + goodBalls);
		Debug.Log ("pockets: " + pockets.Count);

		if(goodBalls >= pockets.Count)
		{
			doGameWon();
		}
	}

	void doGameWon()
	{
		// Ignore if game already over
		if(isGameOver == true) return;

		isGameOver = true;
		Debug.Log ("************* VICTORY!!!");

		// Display game win popup
		uiManager.displayGameWin();
	}

	void doGameOver(string reason)
	{
		// Ignore if game already over
		if(isGameOver == true) return;

		isGameOver = true;
		Debug.Log ("************* FAIL!!!");

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
			Debug.Log ("All balls stopped");
			isShotOngoing = false;
			inBetweenShots();
		}
	}

	// Use this for initialization
	void Start () {
		// Tweak gravity settings
		Physics.gravity = new Vector3(0, -customGravity, 0);

		goodBalls = 0;
		shotCount = 0;
		isGameOver = false;

		// Update HUD
		updateHud();
	}
	
	// Update is called once per frame
	void Update () {

		// Check for rolling balls
		/*if(isBallsRolling == true){
			isBallsRolling = false;
			for(int i = 0; i < balls.Count; i++)
			{
				if (balls[i].checkRolling() == true)isBallsRolling = true;
			}

			// Catch the moment when balls stop rolling

			if(isBallsRolling == false) 
			{
				Debug.Log ("All balls stopped");
				inBetweenShots();
			}
		}*/


	}

	void updateHud()
	{
		// update shot counter
		uiManager.updateShots(shotCount + "/" + maxShotCount);
	}
}
