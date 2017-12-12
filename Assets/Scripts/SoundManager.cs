using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioClip[] sound;
	private AudioSource[] audioSource;
	private SoundManager instance;

	private Transform audioHolder;

	// Use this for initialization
	public void SetupSound () {
		if (instance == null) {	
			instance = this;
		}

		audioSource = new AudioSource[sound.Length];
		audioHolder = new GameObject ("Audio").transform;
		audioHolder.gameObject.AddComponent<AudioListener> ();

		for (int i = 0; i < sound.Length; i++) {
			AddClip (sound [i], i);
		}
	
		audioSource[0].loop = true;
		audioSource[0].Play ();
	}

	public void AddClip(AudioClip clip, int i){
		GameObject toInstantiate = new GameObject ("clip");
		audioSource[i] = toInstantiate.AddComponent<AudioSource> ();
		audioSource[i].clip = clip;
		audioSource [i].volume = GetComponent<UIManager> ().GetVolume (i);
		audioSource [i].transform.SetParent (audioHolder);
	}

	public SoundManager GetInstance(){
		return instance;
	}

	public void ChangeVolume(float value, int i){
		audioSource[i].volume = value;
	}

	public void PlayEffect(){
		audioSource [1].Play();
	}

	public float GetVolume(int i){
		return audioSource [i].volume;
	}

}
