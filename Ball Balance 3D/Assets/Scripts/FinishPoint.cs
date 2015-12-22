using UnityEngine;
using System.Collections;

public class FinishPoint : MonoBehaviour 
{
	bool done = false;
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.CompareTag("Egg"))
		{
			col.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.GetComponent<Ball>().enabled = false;
			done = true;
			Levels.llu = (byte)Mathf.Max(Levels.llu, MapGenerator.level+1);
			PlayerPrefs.SetInt("lastLevel", Levels.llu);
			Debug.Log("Well done Egg!");
		}
	}

	void OnGUI()
	{
		if (!done)
			return;

		if(GUI.Button(new Rect((Levels.SCR_WIDTH-Levels.SCR_WIDTH*0.2f)*0.5f, 10, Levels.SCR_WIDTH*0.2f, 100), "Next Level")) {
			MapGenerator.level++;
			Application.LoadLevel("Play");
		}
	}
}
