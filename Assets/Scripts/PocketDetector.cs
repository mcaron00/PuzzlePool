using UnityEngine;
using System.Collections;

public class PocketDetector : MonoBehaviour {

	public string pocketColor;

	// Report to GameManager on collision
	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log ("Pocket entered");
		string otherColor = collider.gameObject.GetComponent<Ball_Generic>().ballColor;
		//Debug.Log ("Ball is " + otherColor);
		//Debug.Log ("Pocket is " + pocketColor);

		if(otherColor == pocketColor) GameManager.instance.addGoodBall(collider.gameObject);
		else GameManager.instance.reportBadBall();
	}

	// Use this for initialization
	void Start () {
		// Report self to game manager
		GameManager.instance.listPocket(this);
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
}
