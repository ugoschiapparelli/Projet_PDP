using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Life : MonoBehaviour {

	private int life;
	private int lifeSaved;
	private GameObject[] tabEnemyBlock;
	private int indiceTab;
	private bool alive;
	private GameObject gameController;
	private GuardManager gm;
	private WaveManager wm;
	private bool wave;

	// Use this for initialization
	void Start () {
		if (this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder  == 1) {
			alive = false;
			life = 0;
		} else {
			alive = true;
		}
		gameController = GameObject.Find ("_GameManager");
		gm = gameController.GetComponent<GuardManager> ();
		wm = gameController.GetComponent<WaveManager> ();
		life = gm.GetLifeGuard ();
		lifeSaved = life;
		tabEnemyBlock = new GameObject[lifeSaved + 1];
		wave = wm.GetWave ();
		indiceTab = 0;
	}

	void Update(){
		wave = wm.GetWave ();
	}
		
	void OnCollisionEnter2D(Collision2D collision){
		//Debug.Log ("position " + collision.gameObject.transform.position + " life " + life + " FocusOut " + collision.gameObject.GetComponent<Pathfinding> ().GetFocusOut ());
		Pathfinding path = collision.gameObject.GetComponent<Pathfinding>();
		if (collision.gameObject.tag == "enemy" && life > 0 && !path.GetFocusOut()) {
			life--;
			path.SetColTrue ();
			collision.gameObject.GetComponent<TouchMonster> ().DestroyMonster ();
			tabEnemyBlock [indiceTab] = collision.gameObject;
			indiceTab++;

			if (life == 0 && alive) {
				gm.ActiveSleep(this.gameObject);
				for (int i = 0; i < indiceTab; i++) {
					if(tabEnemyBlock[i]) //Si l'objet existe encore
						tabEnemyBlock[i].GetComponent<Pathfinding> ().SetColFalse ();
				}
				alive = false;
			}
		}
	}

	public int GetLife(){
		return life;
	}

	public bool GetAlive(){
		return alive;
	}

	void OnMouseDown(){
		UpgradeManager um = gameController.GetComponent<UpgradeManager> (); 
		if (um.GetUpgrade ()) {
			um.ActivateGuard (this.gameObject);
		}
	}

	public void SetLifeBase(){
		life = lifeSaved;
	}

	public void SetLife(int i){
		life = i;
	}

	public void UpgradeLife(){
		life++;
		lifeSaved++;
	}

	public void SetAlive(bool b){
		alive = b;
	}

	public void InitTabEnnemyBlock(){
		tabEnemyBlock = new GameObject[lifeSaved + 1];
		indiceTab = 0;
	}
}
