using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	public float coeffDifficulty; 
	public int firstWave;
	private System.DateTime ancientTime;
	private bool firstGame;
	private int nbGuard;
	private int[] guardActive;
	private GuardManager gm;
	private SoundManager sm;

	public void SetupTime (int newFirstWave, int newcoeffDifficulty, bool readData) {
		if (readData) {
			firstWave = newFirstWave;
			coeffDifficulty = newcoeffDifficulty;
		}
		if (PlayerPrefs.HasKey ("AncientTime")) {
			ancientTime = System.DateTime.Parse(PlayerPrefs.GetString ("AncientTime"));
			firstGame = false;
		} else {
			ancientTime = System.DateTime.Now;
			firstGame = true;
		}
		gm = GetComponent<GuardManager> ();
		sm = GetComponent<SoundManager> ();
	}

	public void OnApplicationQuit(){
		PlayerPrefs.SetString ("AncientTime", System.DateTime.Now.ToString());
		for (int i = 0; i < nbGuard; i++) {
			string g = "guard" + i.ToString(); 
			PlayerPrefs.SetInt (g, guardActive [i]);
		}
		PlayerPrefs.SetInt ("LifeGuard", Mathf.Abs(gm.GetLifeGuard () - gm.GetBaseLife()));
		PlayerPrefs.SetFloat ("VolumeMusic", sm.GetVolume (0));
		PlayerPrefs.SetFloat ("VolumeEffect", sm.GetVolume (1));
		PlayerPrefs.SetInt ("LastScore", GetComponent<ScoreManager>().GetScore());
	}

	public int GetNumberMonster(){
		if (firstGame) {
			return firstWave;
		} else {
			System.TimeSpan time = System.DateTime.Now - ancientTime;
			float hours = (float)time.TotalHours;
			Debug.Log (" hours "  + Mathf.Sqrt (hours) + " nbGuard " + Mathf.Sqrt (gm.nbGuards)  + " lifeGuard " + gm.GetLifeGuard () + " coeff " + coeffDifficulty);
			int nbMonster = (int) (Mathf.Log(hours) * Mathf.Log(gm.GetNbGuardsActive()) * Mathf.Log(gm.GetLifeGuard()) * coeffDifficulty*100f); //Nombre de vague par heure
			return nbMonster;
		}
	}

	public bool HaveUpgrade(){
		if (firstGame) {
			return true;
		} else {
			return ancientTime.Date != System.DateTime.Now.Date;
		}
	}

	public int[] SetGuardActive(){
		nbGuard = gm.GetNbGuards();
		guardActive = new int[nbGuard];
		for(int i = 0; i < nbGuard; i++){
			string g = "guard" + i.ToString(); 
			if (PlayerPrefs.HasKey (g)) {
				guardActive [i] = PlayerPrefs.GetInt (g);
			} else {
				guardActive [i] = 0;
			}
		}
		return guardActive;
	}

}
