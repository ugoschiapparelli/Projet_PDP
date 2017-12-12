using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {

	public Button[] buttonUpgrade;
	public Vector4[] posButtonUpgrade;
	public Image[] image;
	public Vector4[] posImage;

	private UIManager ui;
	private GuardManager gm;
	private GameObject myInstance;
	private bool touchBase;
	private bool upgrade;

	void Start(){
		upgrade = false;
	}

	// Use this for initialization
	public void SetupUpgrade (GameObject instance) {
		myInstance = instance;
		ui = GetComponent<UIManager> ();
		gm = GetComponent<GuardManager> ();
		GameObject canvas = ui.GetCoffreDuJour ();
		for (int i = 0; i < image.Length; i++) {
			ui.CreateInstance (image [i].gameObject, new Vector2 (posImage [i].x, posImage [i].y), new Vector2 (posImage [i].z, posImage [i].w), canvas);
		}
		AddUpgrade(buttonUpgrade, posButtonUpgrade, canvas);
	}
	
	public void ButtonLife(){
		gm.UpgradeLifeGuard ();
		myInstance.SetActive (false);
		GameState.unfreezeTime();
	}

	public void ButtonGuard(){
		upgrade = true;
		myInstance.SetActive (false);

		GameObject[] tabGuard = gm.GetTabGuard ();
		int[] guardActive = gm.GetActiveGuard();
		for (int i = 0; i < tabGuard.Length; i++) {
			if(guardActive[i] == 0){
				tabGuard [i].GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, .5f);
				tabGuard[i].SetActive(true);
			}
		}
	}

	public void AddUpgrade (Button[] tab, Vector4[] cornerTab, GameObject toInstantiate)
	{
		Button toInstantiateButton;
		for (int i = 0; i < tab.Length; i++) {
			toInstantiateButton = tab [i];
			if ((i == 0 && !gm.HaveNoGuard()) || (i == 1 && !gm.HaveAllGuards ())) {
				Button instance = ui.CreateInstance (toInstantiateButton.gameObject, new Vector2 (cornerTab [i].x, cornerTab [i].y), new Vector2 (cornerTab [i].z, cornerTab [i].w));
				ui.TextMenu (toInstantiateButton.name, new Vector2 (1f, 1f), new Vector2 (0f, 0f), instance.gameObject, new Color (0, 0, 0));
				if (i == 0) {
					instance.onClick.AddListener (ButtonLife);
				} else {
					instance.onClick.AddListener (ButtonGuard);
				}
				instance.transform.SetParent (toInstantiate.transform);
			}
		}
	}

	public bool GetUpgrade(){
		return upgrade;
	}

	public void ActivateGuard(GameObject go){
		GameObject[] tabGuard = gm.GetTabGuard ();
		int[] guardActive = gm.GetActiveGuard();
		gm.UpgradeGuardActive();
		for (int i = 0; i < tabGuard.Length; i++) {
			if (go.Equals (tabGuard [i])) {
				guardActive [i] = 1;
				Life life = tabGuard [i].GetComponent<Life> (); 
				tabGuard [i].GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				life.SetLifeBase ();
				life.SetAlive (true);
				life.InitTabEnnemyBlock ();
				break;
			}
		}
		for (int i = 0; i < tabGuard.Length; i++) {
			if (guardActive [i] == 0) {
				tabGuard [i].GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				tabGuard [i].SetActive (false);
				Life life = tabGuard [i].GetComponent<Life> (); 
				life.SetAlive (false);
				life.InitTabEnnemyBlock ();
			}
		}
		upgrade = false;
		GameState.unfreezeTime();
	}
}
