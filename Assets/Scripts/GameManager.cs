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
		//Debug.Log ("Good ball");

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
		//pocketDetectors = new List<PocketDetector>();

		// Don't accept mouse input for shots yet
		isShotAwaiting = false;

		// Identify Ui Manager
		uiManager = GetComponent<UiManager>();

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
		//Debug.Log ("Balls to sink: " + ballsToSink);

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
		// Skip if no need to process shot input
		if(!isShotAwaiting)
		{
			return;
		}

		//bool isMouseDown = Input.GetMouseButtonDown(0);
		bool isMouseDown = Input.GetMouseButton(0);
		Vector3 deltaMousePos;
		//bool isMouseUp = Input.GetMouseButtonUp(0);

		if (isMouseDown && wasMouseDown != true) {
			//Debug.Log ("1st Frame of Mouse Down");
			// Record initial contact point
			initialMousePosition = Input.mousePosition;
			//Debug.Log ("Mouse Position: " + initialMousePosition);
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

		// Stop controlling velocity
		foreach (Ball_Generic gameObj in balls)
		{
			gameObj.stopVelocityControl();
		}

		// Turn the ongoing indicator off
		uiManager.setOngoing(false);

		// Check if game over
		if(shotCount >= maxShotCount)doGameOver("shots");

		// Accept shot input
		isShotAwaiting = true;
	}

	public void launchLevel(int levelNumber)
	{
		//Debug.Log ("Launch Level #" + levelNumber);

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

		//Debug.Log ("Number of pockets: " + FindObjectsOfType (typeof(PocketDetector)).Length);
		
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

	public void recordWhiteBall(Ball_White reportedBall)
	{
		whiteBall = reportedBall;
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

		// Turn the ongoing indicator on
		uiManager.setOngoing(true);

		isShotOngoing = true;
		isShotAwaiting = false;
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
