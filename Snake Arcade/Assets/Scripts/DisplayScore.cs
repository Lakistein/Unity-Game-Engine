using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour 
{
	// When this script is run, we will display our score in text object
	void Start () 
	{
		Text score = GetComponent<Text> ();	// Since this script is attached to Text object we can simply call GetComponent<Text> to get reference to that object

		score.text = "Your Score: " + GameManager.Score.ToString (); // Here we update text with our score
	}
}
