using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

	public float waitBetweenMonster;
	public float waitBeforeWaves;
	public GameObject Monster;
	public int NombreQuestion;

	private float nbMonster;
	private float actualNbMonster;
	private BoardManager bm;
	private int numberPotion;
	private int columns;
	private int rows;
	private bool wave;
	private int nbWaves;
	private Questionnaire questionnaire;

	// Use this for initialization
	public void SetupWaves (int newWaitBetweenMonster, int newWaitBeforeWaves, int newNumberPotion, bool readData) {
		if (readData) {
			waitBetweenMonster = newWaitBetweenMonster;
			waitBeforeWaves = newWaitBeforeWaves;
			numberPotion = newNumberPotion;
		} else {
			numberPotion = NombreQuestion;
		}
		bm = GetComponent<BoardManager> ();
		nbMonster = GetComponent<TimeManager> ().GetNumberMonster();
		SetNbMonster ((int)(nbMonster));
		columns = bm.GetColumnsDecalage();
		rows = bm.GetRowsDecalage();
		wave = false;
		questionnaire = GameObject.Find("Navigator").GetComponent<Questionnaire>();
	}
		
	private IEnumerator SpawnWaves(){
		if (!wave) {
			Debug.Log ("nombre monstre " + nbMonster);
			yield return new WaitForSeconds (waitBeforeWaves);
			GetComponent<UIManager> ().InterfacePotion ();
			Vector3 spawnValues;
			wave = true;
			Debug.Log("Enterwave" + wave);
			for (int i = 0; i < nbMonster; i++){
				int random = Random.Range (0, 4);
				if (random == 0) {
					spawnValues = new Vector3 (0, Random.Range (0, rows), 1);
				} else if (random == 1) {
					spawnValues = new Vector3 (columns, Random.Range (0, rows), 1);
				} else if (random == 2) {
					spawnValues = new Vector3 (Random.Range (0, columns), 0, 1);
				} else {
					spawnValues = new Vector3 (Random.Range (0, columns), rows, 1);
				}
				Quaternion spawnRotation = Quaternion.identity;

				Instantiate (Monster, spawnValues, spawnRotation);
				yield return new WaitForSeconds (waitBetweenMonster);
			}
			nbWaves--;
		}
	}

	void Update(){
	  actualNbMonster = GetComponent<BarManager>().GetMonsterInMap();
	  if(wave && actualNbMonster <= 0){
	    wave = false;
	  }
	}
	  
	public IEnumerator LaunchQuestionnaire(){
		questionnaire.startQuestionnaire (numberPotion);
		yield return new WaitUntil(() => questionnaire.hasAnsweredAll);
		numberPotion = questionnaire.howManyRightAnswers;
		GetComponent<UIManager> ().updatePotion ();
		GameState.unfreezeTime ();
	}

	public IEnumerator LaunchQuestionnaireChest(){
		do {
			questionnaire.startQuestionnaire (1);
			yield return new WaitUntil (() => questionnaire.hasAnsweredAll);
		} while(questionnaire.howManyRightAnswers != 1);
		GameState.freezeTime ();
	}

	public void LaunchWaves(){
		if (GetComponent<TimeManager> ().HaveUpgrade ()) {
			GetComponent<UIManager> ().DayUpgrade ();
		}
		StartCoroutine (SpawnWaves ());
	}	

	public bool GetWave(){
		return wave;
	}

	public int GetNbWaves(){
		return nbWaves;
	}

	public int GetNumberPotion(){
		return numberPotion;
	}

	public void SetNumberPotion(int newNumberPotion){
		numberPotion = newNumberPotion;
	}

	public void DownNumberPotion(){
		numberPotion--;
	}

	public float GetTotalMonster(){
		return nbMonster;
	}

	public float GetNbMonster(){
		return actualNbMonster;
	}

	public int GetNumberQuestion(){
		return NombreQuestion;
	}

	private void SetNbMonster(int i){
		actualNbMonster = i;
	}

	public Questionnaire GetQuestionnaire(){
		return questionnaire;
	}
}
