using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public string[] levels;
	public int[] shotsPerLevel;
	public int lastLevelWon;

	private float customGravity = 12.0f;
	private List<Ball_Generic> balls;
	//private List<PocketDetector> pocketDetectors;
	private int goodBalls;
	private int shotCount;
	private int maxShotCount;
	private bool isShotOngoing;
	private bool isShotAwaiting;
	private UiManager uiManager;
	private bool isGameOver;
	private int currentLevel;
	private int ballsToSink;
	private bool wasMouseDown;
	private Vector3 initialMousePosition;
	private Ball_White whiteBall;


	public void addGoodBall(GameObject ballInPocket)
	{
		
		goodBalls++;
		checkVictory();

		// Destroy ball in the pocket
		balls.Remove (ballInPocket.GetComponent<Ball_Generic>());
		Object.Destroy (ballInPocket);

	}

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		// Init levels array and fill it
		levels = new string[4];
		levels[0] = "Pzp_Level1_3";
		levels[1] = "Pzp_Level2_2";
		levels[2] = "Pzp_Level3_1";
		levels[3] = "Pzp_Level4_1";

		// Determine shots per level
		shotsPerLevel = new int[4];
		shotsPerLevel[0] = 7;
		shotsPerLevel[1] = 8;
		shotsPerLevel[2] = 9;
		shotsPerLevel[3] = 5;

		//Debug.Log ("Game Manager Ready");
		balls = new List<Ball_Generic>();

		// Don't accept mouse input for shots yet
		isShotAwaiting = false;

		// Identify Ui Manager
		uiManager = GameObject.Find("UiManager").GetComponent<UiManager>();
		uiManager.init ();

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
		// Check that only one ball is left (assuming it's the white one)
		if(goodBalls >= ballsToSink - 1)
		{
			doGameWon();
		}
	}

	void doGameWon()
	{
		isShotAwaiting = false;

		// Hide the ongoing shot indicator
		uiManager.setOngoing(false);

		// Ignore if game already over
		if(isGameOver == true) return;

		// Record that the level was conquered
		lastLevelWon = currentLevel;

		isGameOver = true;

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
		isShotAwaiting = false;

		// Hide the ongoing shot indicator
		uiManager.setOngoing(false);

		// Ignore if game already over
		if(isGameOver == true) return;

		isGameOver = true;
		//Debug.Log ("************* FAIL!!!");

		// Display game over popup
		uiManager.displayGameOver(reason);
	}

	void FixedUpdate () 
	{
		//Debug.Log ("Fixed Update");
		//Debug.Log("isShotOngoing: " + isShotOngoing);
		//Debug.Log("isShotAwaiting: " + isShotAwaiting);
		// *************************************************************
		// if shotCount is ongoing, check if balls have stopped
		// *************************************************************

		// Skip if game over
		if(isGameOver)return;

		if(isShotOngoing)
		{
			// Check for all sleeping balls
			bool allStopped = true;

			foreach (Ball_Generic item in balls) {
				//Debug.Log ("Ball " + item.ballColor + " sleep: " + item.checkSleeping());

				if (item.checkSleeping() == false)
					allStopped = false;
			}

			if (allStopped == true) {
				//Debug.Log ("***** ALL STOPPED *****");

				isShotOngoing = false;
				inBetweenShots ();
			}

			return;
		}

		// *************************************************************
		// if not yet ready to accept inputs, stop now
		// *************************************************************

		if(!isShotAwaiting)
		{
			return;
		}

		// *************************************************************
		// Accept player inputs
		// *************************************************************
		bool isMouseDown = Input.GetMouseButton(0);
		Vector3 deltaMousePos;
		//bool isMouseUp = Input.GetMouseButtonUp(0);

		if (isMouseDown && wasMouseDown != true) {
			// Record initial contact point
			initialMousePosition = Input.mousePosition;

		} else if (isMouseDown) {
			// Transmit delta of mouse position to white ball
			whiteBall.receiveMouseInput(initialMousePosition - Input.mousePosition);

		} else if (wasMouseDown && !isMouseDown) {
			whiteBall.receiveMouseUp ();
		}

		// Record state of input
		wasMouseDown = isMouseDown;
	}

	void inBetweenShots()
	{
		//Debug.Log ("in between shots");
		// Turn the ongoing indicator off
		uiManager.setOngoing(false);

		// Check if game over
		if(shotCount >= maxShotCount)doGameOver("shots");

		// Accept shot input
		isShotAwaiting = true;
	}

	public void launchLevel(int levelNumber)
	{
		//Debug.Log("LaunchLevel");
		// Record level
		currentLevel = levelNumber;

		// Clear lists
		balls.Clear ();
		//pockets.Clear ();

		// Reset phase trackers
		isShotOngoing = false;
		goodBalls = 0;
		shotCount = 0;
		isGameOver = false;
		ballsToSink = 0;

		// Update max shot count for this level
		maxShotCount = shotsPerLevel[levelNumber - 1];

		// Update HUD
		updateHud();

		// Destroy previously loaded level
		GameObject sceneMasterNode = GameObject.Find ("SceneMaster");
		Object.Destroy (sceneMasterNode);
		
		// Set UI for game
		uiManager.startGame ();
		
		// Load first level
		Application.LoadLevelAdditive(levels[currentLevel - 1]);

		// Accept input
		isShotAwaiting = true;
	}

	public void listBall (Ball_Generic reportedBall)
	{
		balls.Add(reportedBall);
		ballsToSink++;

		// Use this number to differentiate balls
		// This is used for audio management (making sure 2 balls never simultaneously trigger the same audio)
		reportedBall.audioPriority = ballsToSink; 

		//Debug.Log ("Reported balls: " + balls.Count);
	}
	
	public void nextLevel()
	{
		// Increment level index
		currentLevel++;

		// ... And load!
		launchLevel(currentLevel);
	}

	public void recordWhiteBall(Ball_White reportedBall)
	{
		whiteBall = reportedBall;
	}

	public void replay()
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

		// Turn the ongoing indicator on
		uiManager.setOngoing(true);

		isShotOngoing = true;
		isShotAwaiting = false;
	}

	// Use this for initialization
	void Start () {

		// Tweak gravity settings
		Physics.gravity = new Vector3(0, -customGravity, 0);

		// Now show the main menu
		uiManager.showMainMenu();
	}

	public void setInput (bool state)
	{
		isShotAwaiting = state;
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
