using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardManager : MonoBehaviour {

	public GameObject[] guardTile;
	public GameObject[] guardSleepTile;
	public GameObject[] guardBase;
	private Transform guardHolder;
	private Transform guardSleepHolder;
	private Transform baseHolder;
	private Vector2 guardPosition;

	private GameObject[] tabBase;
	private GameObject[] tabGuard;
	private GameObject[] tabGuardSleep;
	private int[] guardActive;
	private int lifeGuard;
	private WaveManager wm;

	public int baseLife;
	public int nbGuards;
	private int nbGuardsActive;
	private bool haveWave;

	public Vector2 GetGuardPosition(){
		return guardPosition;
	}

	public GameObject[] GetTabGuard(){
		return tabGuard;
	}

	public int GetNbGuardsActive(){
		return nbGuardsActive;
	}

	void Update(){
		//Debug.Log ("wave " + GetComponent<WaveManager> ().GetWave ());
		if (!wm.GetWave () && haveWave) {
			WakeUpAllGuard ();
			haveWave = false;
		} else if (!haveWave && wm.GetWave ()) {
			haveWave = true;
		}
	}
		
	// Use this for initialization
	public void SetupGuard(int newNbGuard, int newLifeBase, bool readData) {
		if (readData) {
			nbGuards = newNbGuard; 
			baseLife = newLifeBase;
		}
		nbGuardsActive = nbGuards;
		if (PlayerPrefs.HasKey ("LifeGuard")) {
			lifeGuard = PlayerPrefs.GetInt ("LifeGuard") + baseLife;
		} else {
			lifeGuard = baseLife;
		}
		wm = GetComponent<WaveManager> ();
		haveWave = false;
		Debug.Log ("lifeGuard " + lifeGuard);
		tabGuard = new GameObject[nbGuards];
		tabGuardSleep = new GameObject[nbGuards];
		tabBase = new GameObject[nbGuards];

		guardHolder = new GameObject("Guard").transform;
		guardSleepHolder = new GameObject ("GuardSleep").transform;
		baseHolder = new GameObject ("GuardBase").transform;

		guardActive = new int[nbGuards];
		guardActive = GetComponent<TimeManager> ().SetGuardActive ();

		for (int i = 0; i < nbGuards; i++) {
			guardPosition = PlaceGuard (i + 1, nbGuards);
			int indice = getIndiceTab (i, nbGuards);
			GameObject toInstantiate = guardTile[indice]; 
			GameObject toInstantiateSleep = guardSleepTile[getIndiceTab(i, nbGuards)];
			GameObject toInstantiateBase;
			guardPosition.y -= guardTile[indice].transform.localScale.y;
			if (indice % 2 != 0) {
				toInstantiateBase = guardBase [0];
			} else {
				toInstantiateBase = guardBase [1];
			}
			tabGuard [i] = Instantiate (toInstantiate, guardPosition, Quaternion.identity) as GameObject;
			tabBase [i] = Instantiate (toInstantiateBase, guardPosition, Quaternion.identity) as GameObject;
			tabGuardSleep[i] = Instantiate (toInstantiateSleep, guardPosition, Quaternion.identity) as GameObject;
			tabGuardSleep [i].SetActive (false);
			if (guardActive [i] == 0) {
				nbGuardsActive--;
				tabGuard [i].SetActive (false);
			}
			tabGuard[i].transform.SetParent(guardHolder);
			tabGuardSleep [i].transform.SetParent (guardSleepHolder);
			tabBase [i].transform.SetParent (baseHolder);
			tabGuard[i].GetComponent<SpriteRenderer> ().sortingLayerName = "garde";
			tabGuardSleep[i].GetComponent<SpriteRenderer> ().sortingLayerName = "garde";
		}
	}


	Vector2 PlaceGuard(int numGuard, int nbGuards){
		float deg = 360f / nbGuards * (numGuard - 1);
		float rad = Mathf.Deg2Rad*(deg);
		BoardManager bm = GetComponent<BoardManager> ();
		int scale = (int)(bm.GetColumns()/3.33);
		return new Vector2 (Mathf.Sin(rad)*scale+bm.GetColumns() / 2,
			Mathf.Cos(rad)*(scale*bm.GetRows()/bm.GetColumns())+bm.GetRows() / 2);
	}

	int getIndiceTab(int indice, int nbGuard){
		float j = (float) indice / nbGuard;
		if (j < 0.25) {
			return 0;
		} else if (j < 0.5) {
			return 1;
		} else if (j < 0.75) {
			return 2;
		} else {
			return 3;
		}
	}

	public void UpgradeLifeGuard(){
		lifeGuard++;
		for (int i = 0; i < nbGuards; i++) {
			if (tabGuard [i].activeSelf) {
				tabGuard [i].GetComponent<Life> ().UpgradeLife ();
			}
		}
	}

	public void WakeUpAllGuard(){
		for (int i = 0; i < nbGuards; i++) {
			if (tabGuardSleep [i].activeSelf) {
				tabGuard [i].SetActive (true);
				tabGuardSleep [i].SetActive (false);
				Life life = tabGuard [i].GetComponent<Life> (); 
				life.SetLifeBase ();
				life.SetAlive (true);
				life.InitTabEnnemyBlock ();
			}
		}
	}

	public void ActiveSleep(GameObject go){
		for (int i = 0; i < nbGuards; i++) {
			if (go == tabGuard [i]) {
				tabGuard[i].SetActive(false);
				tabGuardSleep[i].SetActive(true);
				Life life = tabGuardSleep [i].GetComponent<Life> (); 
				life.SetLife (0);
				life.SetAlive (false);
			}
		}
	}

	public void DesactiveSleep(GameObject go){
		for (int i = 0; i < nbGuards; i++) {
			if (go == tabGuardSleep [i]) {
				tabGuard[i].SetActive(true);
				tabGuardSleep[i].SetActive(false);
				Life life = tabGuard [i].GetComponent<Life> (); 
				life.SetLifeBase ();
				life.SetAlive (true);
				life.InitTabEnnemyBlock ();
			}
		}
	}

	public GameObject[] GetTabBase(){
		return tabBase;
	}

	public int[] GetActiveGuard(){
		return guardActive;
	}

	public int GetNbGuards(){
		return nbGuards;
	}

	public int GetLifeGuard(){
		return lifeGuard;
	}

	public bool HaveAllGuards(){
		return nbGuardsActive == nbGuards;
	}

	public bool HaveNoGuard(){
		return nbGuardsActive == 0;
	}

	public void UpgradeGuardActive(){
		nbGuardsActive++;
	}

	public int GetBaseLife(){
		return baseLife;
	}
}
