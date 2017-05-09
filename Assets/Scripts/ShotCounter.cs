using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ShotCounter : MonoBehaviour {

	private Text textField;

	// Use this for initialization
	void Start () {
		// Indentify text field
		textField = GetComponent<Text>();

	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

	public void updateShotCounter(string passedString)
	{
		textField.text = passedString;
	}
}
