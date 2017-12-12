using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour {

	private float[] level;				//infos de la bar

	private float lerpSpeed = 2;
	private int actualMonster;

	private Image[] contents;
	private Image[] bar;
	private Text[] valueText;

	private float[] maxValue;
	private float[] value;
	private float nbMonster;

	private Color fullColor;
	private Color lowColor;

	private bool[] lerpColors;

	public float GetMaxValue(int i){
		return maxValue [i];
	}

	public void SetMaxValue(int i, float v){
		maxValue [i] = v;
	}

	public float GetValue(int i){
		return value [i];
	}

	public void SetValue(int i, float v){
		value [i] = v;
	}

	public void SetupBar(){
		fullColor = new Color(0.976f,0.223f,0.223f,1f);
		lowColor = new Color(0.325f,1f,0.119f,1f);
		contents = GetComponent<UIManager> ().GetInstanceBar ();
		nbMonster = GetComponent<WaveManager> ().GetTotalMonster ();
		value = new float[contents.Length];
		maxValue = new float[contents.Length];
		valueText = new Text[contents.Length];
		bar = new Image[contents.Length];
		level = new float[contents.Length];
		lerpColors = new bool [contents.Length];
		for (int i = 0; i < contents.Length; i++) { 
			bar[i] = contents[i].transform.GetChild (0).transform.GetChild (0).gameObject.GetComponent<Image> ();
			valueText[i] = contents[i].transform.GetChild (2).gameObject.GetComponent<Text> ();
			if (i == 0) {
				lerpColors [i] = false;
				InitializeBar (i, GetComponent<ChestManager> ().GetStartingLevel(), GetComponent<ChestManager> ().currentLevel);
			} else if (i == 1) {
				lerpColors [i] = true;
				InitializeBar (i, GetComponent<WaveManager> ().GetTotalMonster (), nbMonster);
			} else if (lerpColors [i] == true) {
				bar [i].color = fullColor;
			}
		}
	}

	void Update (){
		for (int i = 0; i < contents.Length; i++) {
			if (i == 0) {
				UpdateBar (GetComponent<ChestManager> ().currentLevel, i, "Bonbons");
			} else if (i == 1) {
				UpdateBar (nbMonster, i, "Monstres");
			}
			level[i] = Map (value[i], 0, maxValue[i], 0, 1);
			HandleBar (i);
		}
	}

	private void UpdateBar(float val, int i, string s){
		value[i] = val;
		valueText[i].text = s + ": " + Mathf.Clamp (value [i], 0, maxValue [i]);
	}
		
	private void HandleBar(int i){
		level[i] = Map (value[i], 0, maxValue[i], 0, 1);
		bar[i].fillAmount = Mathf.Lerp(bar[i].fillAmount, level[i], Time.deltaTime*lerpSpeed);	//lie le niveau de la barre l'image à la valeur fillAmount
		if(lerpColors[i] == true)
			bar[i].color = Color.Lerp(lowColor, fullColor, level[i]);
	}

	private float Map(float value, float inMin, float inMax, float outMin, float outMax){
		return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		//ce calcul permet de traduire la valeur actuel value en fonction du inmax et inmin de la barre et de l'echelle de l'image outmax et outmin (ici 0 à 1)
		// dans le cas oû le min serait 50 et max 500 comme ça cela fait la modification pour l'affichage qui n'est pas toujours de 0 à 1
		// ex : (78 - 0) * (1 - 0) / (230 - 0) + 0
		// 78 * 1 / 230 = 0.339
	}

	private void InitializeBar(int i, float maxV, float v){
		maxValue [i] = maxV;
		value [i] = v;
		level[i] = Map (value[i], 0, maxValue[i], 0, 1);
	}

	public void DownNbMonster(){
		nbMonster--;
	}

	public float GetMonsterInMap(){
		return nbMonster;
	}
}
