using UnityEngine;
using System.Collections;

public class Ball_Generic : MonoBehaviour {
	
	public string ballColor;
	private float sleepThreshold = 0.01f;
	private float clampHeight = 0.5f;
	private bool isVelocityControlOn;
	private bool wasSleeping;

	protected Rigidbody rb;

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
