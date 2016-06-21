using UnityEngine;
using System.Collections;

public class Ball_Generic : MonoBehaviour {
	
	public string ballColor;
	public int audioPriority;
	private float sleepThreshold = 0.01f;
	private float clampHeight = 0.5f;
	private bool isVelocityControlOn;
	private bool wasSleeping;
	protected audioFXManager audioMan;

	protected Rigidbody rb;

	void Awake()
	{
		audioMan = GameManager.instance.GetComponent<audioFXManager> ();
	}

	public bool checkRolling()
	{
		return !rb.IsSleeping();
	}

	// Use this for initialization
	protected virtual void Start () {
		// Set velocity control to false
		isVelocityControlOn = false;

		// Get rigid body
		rb = GetComponent<Rigidbody>();

		// Init sleeping tracker
		wasSleeping = true;

		// Report self to game manager
		GameManager.instance.listBall(this);

		//Debug.Log ("Ball ready");
	}

	void OnCollisionEnter(Collision col)
	{
		// If hitting another ball, tell the audio manager to play the ball knock audio
		Ball_Generic otherBall = col.gameObject.GetComponent<Ball_Generic>();
		string colMaterial = col.gameObject.GetComponent<Collider> ().material.name;

		Debug.Log("Material of collider " + colMaterial);
		Debug.Log ("euqals with Wall_Physics (Instance)" + string.Equals ("Wall_Physics (Instance)", colMaterial));

		if (otherBall != null) {

			// When 2 balls collide, only one needs to trigger a sound
			if (otherBall.audioPriority < audioPriority) {
				audioMan.playBallKnock (col.relativeVelocity.magnitude);
			}

		//} else if(colMaterial == "Wall_Physics" || colMaterial == "Wall_Pocket_Physics"){
		} else if(string.Equals("Wall_Physics (Instance)",colMaterial) || string.Equals("Wall_Pocket_Physics (Instance)",colMaterial)){
			audioMan.playWallKnock (col.relativeVelocity.magnitude);
		}

	}

	public void stopVelocityControl()
	{
		isVelocityControlOn = false;
		rb.WakeUp ();
	}
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {

		// Check ball when a shot is ongoing
		if(GameManager.instance.checkShotOngoing() == true)
		{
			// If ball is moving, make a few adjustments
			if(!rb.IsSleeping())
			{
				//Update sleeping tracker
				wasSleeping = false;
				
				// Clamp height
				rb.transform.position = new Vector3(rb.transform.position.x,Mathf.Min (rb.transform.position.y,clampHeight),rb.transform.position.z);
				
				// Calculate and log sleep threshold
				float vel = rb.velocity.sqrMagnitude * 0.5f;
				
				// If currently moving, turn velocity control on
				if (vel > sleepThreshold) isVelocityControlOn = true;
				
				// Slam the breaks on if rolling but really slow
				if(isVelocityControlOn == true)
				{
					// Put the breaks on if moving below sleep threshold
					if(vel <= sleepThreshold)rb.Sleep ();
					
				}
				
			}

			// If shot is ongoing and ball just stopped, report it to game manager
			else
			{
				if(wasSleeping == false) GameManager.instance.reportStopped ();
				
				// Change sleeping tracjer
				wasSleeping = true;
			}
		}


	}
}
