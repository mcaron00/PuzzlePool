using UnityEngine;
using System.Collections;

public class audioFXManager : MonoBehaviour {

	public AudioClip ballKnock;
	public AudioClip cueKnock;
	public AudioClip pocketSound;
	public AudioClip wallKnock;

	private float minBallKnockVelocity = 0.5f;
	private float maxBallKnockVelocity = 25.5f;
	private float minBallKnockVolume = 0.15f;
	private float maxBallKnockVolume = 1.0f;

	private float maxCueVelocity = 25.5f;
	private float maxCueVolume = 1.0f;

	private float minWallKnockVelocity = 0.5f;
	private float maxWallKnockVelocity = 25.5f;
	private float minWallKnockVolume = 0.15f;
	private float maxWallKnockVolume = 1.0f;

	private AudioSource source;

	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	public void playBallKnock(float velocity)
	{
		//Debug.Log ("Play ball knock with velocity " + velocity);

		float hitRatio;
		if (velocity < minBallKnockVelocity) {
			return;
		} else if (velocity > maxBallKnockVelocity) {
			hitRatio = 1.0f;
		} else {
			hitRatio = velocity / maxBallKnockVelocity;
		}

		float hitVolume = Mathf.Max(hitRatio * maxBallKnockVolume, minBallKnockVolume);

		source.PlayOneShot (ballKnock, hitVolume);
	}

	public void playCue(float velocity)
	{
		float hitRatio;
		if (velocity > maxCueVelocity) {
			hitRatio = 1.0f;
		}else {
			hitRatio = velocity / maxCueVelocity;
		}

		source.PlayOneShot (cueKnock, hitRatio);
	}

	public void playPocket()
	{
		source.PlayOneShot (pocketSound, 1.0f);
	}

	public void playWallKnock(float velocity)
	{
		//Debug.Log ("************** Play wall knock with velocity " + velocity);

		float hitRatio;
		if (velocity < minWallKnockVelocity) {
			return;
		} else if (velocity > maxWallKnockVelocity) {
			hitRatio = 1.0f;
		} else {
			hitRatio = velocity / maxWallKnockVelocity;
		}

		float hitVolume = Mathf.Max(hitRatio * maxWallKnockVolume, minWallKnockVolume);

		source.PlayOneShot (wallKnock, hitVolume);
	}

	// Use this for initialization
	/*void Start () {
	
	}*/
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
}
