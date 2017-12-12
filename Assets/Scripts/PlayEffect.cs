using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffect : MonoBehaviour {

	private GameObject gameController;

	void OnMouseDown(){
		Debug.Log ("effet");
		gameController.GetComponent<SoundManager> ().PlayEffect ();
	}
}
