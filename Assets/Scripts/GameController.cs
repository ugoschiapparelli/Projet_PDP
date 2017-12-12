using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


	private BoardManager boardScript;
	private int columns;
	public int rows;
	private CameraManager mainCamera;
	private UIManager UI;
	private HomeManager home;
	private ScoreManager score;
	private ChestManager chest;
	private GuardManager guard;
	private SoundManager sound;
	private WaveManager wave;
	private TimeManager time;
	private BarManager bar;

	private Vector3 spawnValues;
	private float cardinal;
	private int[] data;
	private bool readData;

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Landscape;
		boardScript = GetComponent<BoardManager>();
		mainCamera = GetComponent<CameraManager>();
		home = GetComponent<HomeManager>();
		chest = GetComponent<ChestManager> ();
		time = GetComponent<TimeManager> ();
		wave = GetComponent<WaveManager> ();			
		UI = GetComponent<UIManager>();
		guard = GetComponent<GuardManager> ();
		sound = GetComponent<SoundManager> ();
		bar = GetComponent<BarManager> ();
		score = GetComponent<ScoreManager> ();
		InitGame();
	}

	void InitGame(){
		data = new int[GetComponent<ReadManager>().GetNbData()];
	    readData = GetComponent<ReadManager> ().ReadFile (); 
		if (readData) {
			data = GetComponent<ReadManager> ().GetData ();
			rows = data [0];
		}
	    columns = (int)(Screen.width*rows/Screen.height);
	      if (columns % 2 != 0)
	         columns++;
	  	home.SetupHome(columns, rows);
		boardScript.SetupScene(columns, rows, data [1], readData);
		mainCamera.SetupCamera();
		time.SetupTime (data [8], data[9], readData);
		guard.SetupGuard(data [2], data [3], readData);
		wave.SetupWaves (data[6], data [7], data[10], readData);
		chest.SetupChest (data [4], data [5], readData);
		UI.SetupUI();
		sound.SetupSound ();
		bar.SetupBar ();
		wave.LaunchWaves ();
		score.SetupScore ();
	}

	public UIManager GetUIManager(){
		UIManager tmp = UI;
		return tmp;
	}

	public void End (){
		Destroy (boardScript);
		Destroy (mainCamera);
		Destroy (home);
		Destroy (UI);
		Destroy (score);
		Destroy (chest);
		Destroy (guard);
		Destroy (bar);
	}
		
}
