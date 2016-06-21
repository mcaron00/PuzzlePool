using UnityEngine;
using System.Collections;

public class Ball_White : Ball_Generic {

	public float speed;
	private GameObject arrowHelper;
	private GameObject mouseTracker;

	//private bool isAiming;
	private float helperY = 1.1f;
	private float helperScale = 0.025f;
	private float vanishTres = 7.5f;
	private float minScale = 0.01f;
	private float maxScale = 0.5f;
	private float forceMultiplier = 150f;
	private bool isHelperStarted;
	private bool isSetToVanish;
	private Renderer helperRend;
	private Vector3 topDownDirection;

	// receive mouse input from game manager
	public void receiveMouseInput(Vector3 mouseDelta)
	{
		//Debug.Log ("Mouse input received with vector " + mouseDelta);

		// Move and scale shot helper *******

		// Bring arrow plane on top of ball
		arrowHelper.transform.position = new Vector3(transform.position.x,helperY,transform.position.z);

		// Set to default scale
		arrowHelper.transform.localScale = new Vector3(helperScale,helperScale,minScale);

		// Helper faces to reflect the received delta
		topDownDirection = new Vector3(mouseDelta.x,0,mouseDelta.y);
		arrowHelper.transform.LookAt(-topDownDirection);

		// Prepare some calculations
		Vector3 newScale = arrowHelper.transform.localScale;

		//Debug.Log ("Vector magnitude " + mouseDelta.magnitude);

		// Hide helper and process if mouse helper inside the vanish distance
		if (mouseDelta.magnitude <= vanishTres) {
			newScale.z = minScale;
			helperRend.enabled = false;
			isSetToVanish = true;
		}

		//Otherwise, show helper, rotate and scale it
		else {
			// Helper scales to reflect the delta distance
			newScale.z = Mathf.Min((mouseDelta.magnitude-vanishTres) * 0.0075f, maxScale);
			arrowHelper.transform.localScale = newScale;

			helperRend.enabled = true;
			isSetToVanish = false;
		}
		//Debug.Log ("Mouse input finished with vector " + mouseDelta);
	}

	public void receiveMouseUp()
	{
		Debug.Log ("Mouse up time");

		// Only give an impulse if helper not set to vanish, AND if the player is actually shooting
		if (isSetToVanish == false)
		{
			Debug.Log ("Shot sent");

			// Prepare impulse
			float force = arrowHelper.transform.localScale.z * forceMultiplier;

			// Shoot!
			rb.AddForce(topDownDirection.normalized * force,ForceMode.Impulse);

			// Tell game manager that balls are now rolling
			GameManager.instance.reportShot ();
		}
			
		// hide helper
		helperRend.enabled = false;
	}

	// Use this for initialization
	void Start () {
		//Debug.Log ("Ball_White start");
		base.Start();

		// on awake, find the arrowHelper and the mouseTracker
		arrowHelper = GameObject.Find ("ArrowPlaneRoot");
		mouseTracker = GameObject.Find ("mouseTrackerSphere");


		// Get renderer for helper
		helperRend = arrowHelper.transform.GetChild(0).GetComponent<Renderer>();

		//isAiming = false;

		// Scale helper plane to prepare it
		arrowHelper.transform.localScale = new Vector3(helperScale,helperScale,helperScale);

		// Report self to game manager
		GameManager.instance.recordWhiteBall(this);
	}
	
	// Update is called once per frame
	/*void Update () {
		
	}*/


}
