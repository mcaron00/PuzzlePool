using UnityEngine;
using System.Collections;

public class Ball_White : Ball_Generic {

	public float speed;
	private GameObject arrowHelper;
	private GameObject mouseTracker;

	private bool isAiming;
	private float helperY = 1.1f;
	private float helperScale = 0.025f;
	private float vanishTres = 0.5f;
	private float minScale = 0.01f;
	private float maxScale = 0.5f;
	private float forceMultiplier = 150f;
	private bool isHelperStarted;
	private bool isSetToVanish;
	private Renderer helperRend;

	// Use this for initialization
	void Start () {
		//Debug.Log ("Ball_White start");
		base.Start();

		// on awake, find the arrowHelper and the mouseTracker
		arrowHelper = GameObject.Find ("ArrowPlaneRoot");
		mouseTracker = GameObject.Find ("mouseTrackerSphere");


		// Get renderer for helper
		helperRend = arrowHelper.transform.GetChild(0).GetComponent<Renderer>();

		isAiming = false;

		// Scale helper plane to prepare it
		arrowHelper.transform.localScale = new Vector3(helperScale,helperScale,helperScale);

	}
	
	// Update is called once per frame
	void Update () {
		// Adjust aiming guides if currently aiming
		if (isAiming == true)
		{

			// Process mouse position and
			// calculate distance between mouse and ball
			Vector3 oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 mousePosAdjusted = new Vector3(oldMousePos.x, helperY, oldMousePos.z);
			float distance = Vector3.Distance(arrowHelper.transform.position,mousePosAdjusted);
			Vector3 newScale = arrowHelper.transform.localScale;


			// Move arrow helper on ball and rotate it away from mouse helper
			arrowHelper.transform.LookAt(mousePosAdjusted);
			mouseTracker.transform.position = mousePosAdjusted;

			// Hide helper and process if mouse helper inside the vanish distance
			if (distance <= vanishTres && isHelperStarted == true)
			{
				newScale.z = minScale;
				helperRend.enabled = false;
				isSetToVanish = true;
			}

			// Show helper, scale it, and process if helper outside of vanish distance
			else
			{
				helperRend.enabled = true;
				newScale.z = Mathf.Min((distance-vanishTres) * 0.3f, maxScale);
				arrowHelper.transform.localScale = newScale;
				isHelperStarted = true;
				isSetToVanish = false;
			}

		}

		//base.Update ();

	}

	// Detect click
	void OnMouseDown ()
	{
		//Debug.Log ("mouse down");
		// Only accept mouse input if ball is not rolling
		if(GameManager.instance.checkShotOngoing () == false){
			//Debug.Log ("Shot allowed");

			// Bring arrow plane on top of ball
			arrowHelper.transform.position = new Vector3(transform.position.x,helperY,transform.position.z);

			// Set to default scale
			arrowHelper.transform.localScale = new Vector3(helperScale,helperScale,minScale);
			
			// Set isAiming to true so update knows to rotate helper
			isAiming = true;

		}

		// Track whether or not the "beggining" part of the helper is done
		isHelperStarted = false;
		isSetToVanish = false;

	}

	void OnMouseUp ()
	{
		// Only give an impulse if helper not set to vanish
		if (isSetToVanish == false)
		{
			//Debug.Log ("Shot sent");

			// Prepare impulse
			float force = arrowHelper.transform.localScale.z * forceMultiplier;
			Vector3 dir = arrowHelper.transform.position - mouseTracker.transform.position;
			dir = dir.normalized;
			
			// Shoot!
			rb.AddForce(dir * force,ForceMode.Impulse);

			// Tell game manager that balls are now rolling
			GameManager.instance.reportShot ();
		}

		// Stop aiming
		isAiming = false;

		// move arrow plane away
		arrowHelper.transform.position = new Vector3(50.0f,50.0f,50.0f);
		mouseTracker.transform.position = new Vector3(50.0f,50.0f,50.0f);

	}
}
