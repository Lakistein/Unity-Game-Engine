using UnityEngine;
using System.Collections;

public class PlayAgainButton : MonoBehaviour {

	public void PlayAgain()
	{
		Application.LoadLevel ("Game");// If user clicks play again button, we load game again
	}
}
