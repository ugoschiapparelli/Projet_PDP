using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]

public class ChestManager : MonoBehaviour {

	private GameObject[] instanceChest;
	public GameObject[] chestTiles;
	private int startingLevel;
	public int currentLevel;
	public int candySteal;
	private Transform chestHolder;
	public float updateInterval = 0.5F;
	private double lastInterval;


	GameObject toInstantiate;

	bool isEmpty;
	bool candyTook;

	public int GetChestLevel(){
		return currentLevel;
	}

	// Use this for initialization
	public void SetupChest (int NewCandySteal, int NewStartingLevel, bool readData) {
		if (readData) {
			startingLevel = NewStartingLevel;
			candySteal = NewCandySteal;
		}
		startingLevel = (int) GetComponent<WaveManager> ().GetNbMonster ();
		lastInterval = Time.realtimeSinceStartup;
		currentLevel = startingLevel;
		isEmpty = false;
		instanceChest = new GameObject[chestTiles.Length];
		AddSprites ();

	}


	// Update is called once per frame
	void Update () {
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval) {
			SpriteUpdate ();
			lastInterval = timeNow;
		}
		candyTook = false;
	}

	public void CandySteal(){
		candyTook = true;
		if (currentLevel > 0 && !isEmpty) {
			currentLevel -= candySteal;
			if (currentLevel < 0) {
				currentLevel = 0;
			}
		} else if (currentLevel <= 0 && !isEmpty) {
			Empty ();
		}
	}

	void Empty(){
		isEmpty = true;
	}

	void AddSprites(){
		int columns = GetComponent<BoardManager> ().GetColumns();
		int rows = GetComponent<BoardManager> ().GetRows();
		float homeSize = GetComponent<HomeManager> ().GetHomeSize ();
		chestHolder = new GameObject("Chest").transform;
		for (int i = 0; i < chestTiles.Length; i++)
		{
			toInstantiate = chestTiles[i];
			GameObject instance = Instantiate(toInstantiate, new Vector3(columns / 2, rows / 2 + homeSize), Quaternion.identity) as GameObject;
			instance.transform.SetParent(chestHolder);
			instanceChest [i] = instance;
			instanceChest [i].SetActive (false);
		}
	}

	void SpriteUpdate(){
		if ((float) currentLevel > (float) 3/4 * startingLevel) {
			instanceChest [0].SetActive (false);
			instanceChest [1].SetActive (false);
			instanceChest [2].SetActive (false);
			instanceChest [3].SetActive (true);
		}
		else if ((float) currentLevel >= (float) 1/2 * startingLevel) {
			instanceChest [0].SetActive (false);
			instanceChest [1].SetActive (false);
			instanceChest [2].SetActive (true);
			instanceChest [3].SetActive (false);
		}
		else if ((float) currentLevel >= (float) 1/4 * startingLevel) {
			instanceChest [0].SetActive (false);
			instanceChest [1].SetActive (true);
			instanceChest [2].SetActive (false);
			instanceChest [3].SetActive (false);
		}
		else {
			instanceChest [0].SetActive (true);
			instanceChest [1].SetActive (false);
			instanceChest [2].SetActive (false);
			instanceChest [3].SetActive (false);
		}
	}

	public int GetStartingLevel(){
		return startingLevel;
	}
}