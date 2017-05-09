using UnityEngine;
using System.Collections;

public class pocketEndCollision : MonoBehaviour {

	// Use this for initialization
	/*void Start () {
	
	}*/
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

	void OnCollisionEnter(Collision col)
	{
		// The ball that just collided will now to on the trapped balls layer
		// Unless it's the white ball, which has a chance of coming out
		if(col.gameObject.GetComponent<Ball_White>() == null){
			col.gameObject.layer = 8;
			//Debug.Log ("Non-White Ball");
		}

	}
}
