using UnityEngine;
using System.Collections;

public class Ball_Generic : MonoBehaviour {
	
	public string ballColor;
	public int audioPriority;
	private float clampHeight = 0.51f; // Slightly above 0.5 otherwise it fucks up the physics and some balls never sleep
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

	public bool checkSleeping()
	{
		return rb.IsSleeping();
	}

	// Use this for initialization
	protected virtual void Start () {
		
		// Get rigid body
		rb = GetComponent<Rigidbody>();

		// Report self to game manager
		GameManager.instance.listBall(this);

	}

	void OnCollisionEnter(Collision col)
	{
		// If hitting another ball, tell the audio manager to play the ball knock audio
		Ball_Generic otherBall = col.gameObject.GetComponent<Ball_Generic>();
		string colMaterial = col.gameObject.GetComponent<Collider> ().material.name;

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

	// Update is called once per frame
	protected virtual void FixedUpdate () {

		// Check ball when a shot is ongoing
		if(GameManager.instance.checkShotOngoing() == true)
		{
			// If ball is moving, clamp height
			if(!rb.IsSleeping())
			{
				rb.transform.position = new Vector3(rb.transform.position.x,Mathf.Min (rb.transform.position.y,clampHeight),rb.transform.position.z);
				
			}

		}


	}
}
