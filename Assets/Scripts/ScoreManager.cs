using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	public int score;
	private int chestLevel;
	private bool scoreNeedToUp;
	private UIManager ui;

	public int GetScore(){
		return score;
	}

	public bool GetScoreNeedToUp(){
		return scoreNeedToUp;
	}

	// Use this for initialization
	public void SetupScore () {
		scoreNeedToUp = true;
		if (PlayerPrefs.HasKey ("LastScore")) {
			score = PlayerPrefs.GetInt ("LastScore");
		} else {
			score = 0;
		}
		ui = GetComponent<UIManager> (); 
		ui.SetScoreText (score);
	}

	void Update(){
		chestLevel = GetComponent<ChestManager>().GetChestLevel();
		if(scoreNeedToUp == true && GetComponent<WaveManager>().GetWave() == false && GetComponent<BarManager>().GetMonsterInMap() == 0){
			score+=chestLevel;
			scoreNeedToUp = false;
			ui.SetScoreText (score);
		}
	}
}