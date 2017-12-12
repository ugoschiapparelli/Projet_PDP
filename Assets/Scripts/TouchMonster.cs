using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMonster : MonoBehaviour {

	public Material particleMaterial;

	private GameObject gameController;
	private SoundManager soundManager;
	private ParticleSystem system;
	private GameObject go;
	private bool afterEffect;
	private Pathfinding path;

	void Start(){
		gameController = GameObject.Find ("_GameManager");
		soundManager = gameController.GetComponent<SoundManager> ();
		afterEffect = false;
		go = new GameObject("Particle System");
		go.transform.localScale = new Vector3 (1, 1, 1);
		system = go.AddComponent<ParticleSystem>();
		ParticleSystemRenderer psr = system.GetComponent<ParticleSystemRenderer> ();
		psr.material = particleMaterial;
		psr.sortingLayerID = this.GetComponent<Renderer> ().sortingLayerID;
		path = GetComponent<Pathfinding> ();

		system.Stop ();

		var mainModule = system.main;
		mainModule.startColor = Color.magenta;
		mainModule.startSize = 0.5f;
		mainModule.duration = 1.0f;
		mainModule.simulationSpeed = 2;
		mainModule.startLifetime = 1;
		mainModule.startSpeed = 0.5f;
		mainModule.loop = false;
	}

	void Update(){
		if (afterEffect && !system.isPlaying) {
			Destroy (go);
			Destroy (system.gameObject);
			Destroy (this.gameObject);
		}
	}

	void OnMouseDown(){
		if (!path.GetIsDead()) {
			DestroyMonster ();
		}
	}

	public void DestroyMonster(){
		soundManager.PlayEffect ();
		go.transform.position = this.transform.position;
		path.SetColFalse ();
		system.Play ();
		this.gameObject.GetComponent<Renderer> ().enabled = false;
		afterEffect = true;
		path.SetIsDead ();
		gameController.GetComponent<BarManager> ().DownNbMonster (); 
	}
}
