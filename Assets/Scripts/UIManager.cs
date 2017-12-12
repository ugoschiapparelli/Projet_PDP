using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

	public Button[] buttonTab;
	public Vector4[] posButtonTab;
	public Button[] buttonMenu;
	public Button[] buttonChest;
	public Vector4[] posButtonChest;
	public Button[] buttonPotion;
	public Vector4[] posButtonPotion;
	public Vector4[] posButtonMenu;
	public Font myFont;
	public Image ImageMenu;

	private Button instancePotion;

	private Canvas myCanvas;
	private GameObject mainCanvas;
	private GameObject[] instanceMenu;

	public Slider[] sliderVolume;
	public Vector2[] posSliderVolume;
	private float[] volume;
	private Slider[] instanceVolume;

	public Vector4[] posBar;
	public Image[] bar;
	public Image[] instanceBar;

	private Text instanceScore;
	private GuardManager gm;
	private WaveManager wm;
	private string textNbGuard;
	private string textLifeGuard;
	private Text instanceNbGuard;
	private Text instanceLifeGuard;


	// Use this for initialization
	public void SetupUI ()
	{
		
		gm = GetComponent<GuardManager> ();
		wm = GetComponent<WaveManager> ();
		mainCanvas = new GameObject ("Canvas");
		CreateCanvas (mainCanvas,"UI");
		instanceMenu = new GameObject[buttonTab.Length + 2];
		instanceVolume = new Slider[sliderVolume.Length];
		volume = new float[2];
		if (PlayerPrefs.HasKey ("VolumeMusic")) {
			volume[0] = PlayerPrefs.GetFloat ("VolumeMusic");
			volume[1] = PlayerPrefs.GetFloat ("VolumeEffect");
		} else {
			volume[0] = 0.5f;
			volume[1] = 0.5f;
		}

		textNbGuard = "Nombres de gardes : " + gm.GetNbGuardsActive () + "/" + gm.GetNbGuards();
		textLifeGuard = "Vie des gardes : " + gm.GetLifeGuard();
		AddButton (buttonTab, posButtonTab, new Vector2 (0, 0), mainCanvas);
		instanceBar = new Image[bar.Length];
		AddImage(bar, posBar, mainCanvas);
		TextMenu ("Score", new Vector2 (0.7f, 1f), new Vector2 (0.3f, 0.9f), myCanvas.gameObject, new Color (1f, 1f, 0f, 1f));

		mainCanvas = new GameObject ("EventSystem");
		mainCanvas.AddComponent<EventSystem> ();
		mainCanvas.AddComponent<StandaloneInputModule> ();
		mainCanvas.transform.SetParent (myCanvas.transform);

		for (int i = 0; i < buttonTab.Length; ++i) {
			CreateMenu (buttonTab[i].name, new Color (0f, 0f, 0f, 0.5f), new Color (0.5f, 0.5f, 0.5f, 1f), buttonMenu, posButtonMenu, i);
		}
		CreateMenu ("Coffre du jour", new Color (0f, 0f, 0f, 0.5f), new Color (0.5f, 0.5f, 0.5f, 1f), buttonChest, posButtonChest, buttonTab.Length);
		CreateMenu ("Création des potions", new Color (0f, 0f, 0f, 0.5f), new Color (0.5f, 0.5f, 0.5f, 1f), buttonPotion, posButtonPotion, buttonTab.Length + 1);
	}

	public void Quit ()
	{
		instanceMenu [1].SetActive (true);
	}

	public void DayUpgrade(){
		instanceMenu[buttonTab.Length].SetActive(true);
		GameState.freezeTime ();
	}

	public void OpenChest(){
		StartCoroutine (LaunchChest ());
	}

	public IEnumerator LaunchChest(){
		StartCoroutine(wm.LaunchQuestionnaireChest());
		yield return new WaitUntil (() => wm.GetQuestionnaire ().hasAnsweredAll);
		instanceMenu [buttonTab.Length].GetComponentInChildren<Button> ().gameObject.SetActive (false);
		GetComponent<UpgradeManager>().SetupUpgrade (instanceMenu [buttonTab.Length]);	
	}

	public void InterfacePotion(){
		GameState.freezeTime ();
		instanceMenu [buttonTab.Length + 1].SetActive (true);
	}

	public void CreatePotion(){
		StartCoroutine(wm.LaunchQuestionnaire ());
		instanceMenu [buttonTab.Length + 1].SetActive (false);
		GameState.unfreezeTime();
	}

	public void Option ()
	{
		GameState.freezeTime ();
		instanceMenu [0].SetActive (true);
	}

	public void Upgrade ()
	{
		GameState.freezeTime ();
		instanceNbGuard.text = "Nombres de gardes : " + gm.GetNbGuardsActive () + "/" + gm.GetNbGuards();
		instanceLifeGuard.text = "Vie des gardes : " + gm.GetLifeGuard();
		instanceMenu [2].SetActive (true);
	}

	public Transform GetOptionObject(){
		Transform tmp = instanceMenu [0].transform;
		return tmp;
	}

	public GameObject GetCanvas(){
		GameObject tmp = mainCanvas;
		return tmp;
	}

	public void ConfigOption(GameObject parent, Slider sliderObject, string name, Vector2[] position, int i){
		Slider toInstantiate = sliderObject;
		toInstantiate.handleRect.gameObject.GetComponent<RectTransform> ().localScale = new Vector3(1.5f*(float)Screen.width/1000,1f);
		toInstantiate.value = volume [i];
		instanceVolume[i] = Instantiate(toInstantiate, new Vector3(0f,0f,0f), Quaternion.identity) as Slider;
		instanceVolume[i].transform.SetParent (parent.transform);
		instanceVolume[i].transform.localScale = new Vector3 (1f, 1f, 1f);
		instanceVolume[i].transform.localPosition = new Vector3 (0f, 0f, 0f);
		var rect = instanceVolume [i].gameObject.GetComponent<RectTransform> ();
		InitRectTransform (rect, position[0+i*4], position[1+i*4], parent);
		TextMenu (name, position[2+i*4], position[3+i*4], parent, new Color(0, 0, 0));
	}

	public void ConfigUpgrade (GameObject parent){
		TextMenu (textNbGuard, new Vector2 (1f, 0.8f), new Vector2 (0f, 0.7f), parent, new Color (0f, 0f, 0f, 1f));
		TextMenu (textLifeGuard, new Vector2 (1f, 0.6f), new Vector2 (0f, 0.5f), parent, new Color (0f, 0f, 0f, 1f));
	}

	public float GetVolume(int i){
		return volume[i];
	}

	public void SetVolume(){
		for (int i = 0; i < instanceVolume.Length; i++) {
			volume [i] = instanceVolume [i].value;
			GetComponent<SoundManager> ().GetInstance().ChangeVolume (volume[i],i);
		}
	}

	public void Confirmer(){
		if (instanceMenu [1].activeSelf)
			QuitGame ();
		else if (instanceMenu [0].activeSelf) {
			SetVolume ();
		
		}
		GameState.unfreezeTime ();
		QuitMenu ();
	}

	public void QuitMenu ()
	{
		if (instanceMenu [0].activeSelf) {
			for (int i = 0; i < instanceVolume.Length; i++) {
				instanceVolume[i].value = volume[i];
			}
		}
		GameObject tmp = GameObject.FindGameObjectWithTag ("Menu");
		GameState.unfreezeTime ();
		tmp.SetActive (false);
	}

	public void QuitGame ()
	{
		GetComponent<TimeManager> ().OnApplicationQuit ();
		GameState.quitCliqueTonQCM();
		MetricLogger.instance.Log("CliqueTonQCM_QUIT", ProfileManager.CurrentProfileName(), true);
	}

	public void Potion(){
		int nbPotion = wm.GetNumberPotion ();
		//Debug.Log("nb " + nbPotion + " wave " + wm.GetWave());
		if (nbPotion > 0 && wm.GetWave()) { 
			wm.DownNumberPotion ();
			nbPotion--;
			gm.WakeUpAllGuard ();
			instancePotion.gameObject.GetComponentInChildren<Text> ().text = nbPotion.ToString ();
		}
	}

	public void updatePotion(){
		int nbPotion = wm.GetNumberPotion ();
		instancePotion.gameObject.GetComponentInChildren<Text> ().text = nbPotion.ToString ();
	}

	public void CreatePanel (Image toInstantiate, Vector2 topRight, Vector2 bottomLeft)
	{
		var rectTransform = toInstantiate.GetComponent <RectTransform> ();
		rectTransform.SetParent (myCanvas.transform);
		InitRectTransform (rectTransform, topRight, bottomLeft, myCanvas.gameObject);
		toInstantiate.transform.localScale = new Vector3 (1f, 1f, 1f);
	}

	public void InitRectTransform(RectTransform rect, Vector2 topRight, Vector2 bottomLeft, GameObject parent){
		rect.SetParent (parent.transform);
		rect.anchorMax = topRight;
		rect.anchorMin = bottomLeft;
		rect.offsetMax = Vector2.zero;
		rect.offsetMin = Vector2.zero;
		rect.localScale = new Vector3 (1f, 1f, 1f);
	}

	public void AddButton (Button[] tab, Vector4[] cornerTab, Vector2 plan, GameObject toInstantiate)
	{
		Button toInstantiateButton;
		for (int i = 0; i < tab.Length; i++) {
			toInstantiateButton = tab [i];
			Button instance = CreateInstance (toInstantiateButton.gameObject, plan + new Vector2 (cornerTab [i].x, cornerTab [i].y), plan + new Vector2 (cornerTab [i].z, cornerTab [i].w));
			if(toInstantiateButton.name!="Potion" && toInstantiateButton.name!="ChestButton" && toInstantiateButton.name!="CreatePotion") //sale
				TextMenu (toInstantiateButton.name, new Vector2 (1f, 1f), new Vector2 (0f, 0f), instance.gameObject, new Color (0, 0, 0));
			if (!toInstantiate.name.Equals("Canvas")) {
				if(!toInstantiate.GetComponentInChildren<Button>()){
					if (toInstantiateButton.name == "CreatePotion") {
						instance.onClick.AddListener (CreatePotion);
					} else {
						instance.onClick.AddListener (OpenChest);
					}
				} else {
					if (i == 0) {
						instance.onClick.AddListener (QuitMenu);
					} else {
						instance.onClick.AddListener (Confirmer);
					}
				}
			} else{
				if (i == 0) {
					instance.onClick.AddListener (Option);
				} else if (i == 1) {
					instance.onClick.AddListener (Quit);
				} else if (i == 2) {
					instance.onClick.AddListener (Upgrade);
				} else {
					TextMenu (wm.GetNumberPotion ().ToString(), new Vector2 (0.5f, 0.5f), new Vector2 (-0.8f, 0.2f), instance.gameObject, new Color(255, 255, 255));
					instancePotion = instance;
					instance.onClick.AddListener (Potion);
				}
			}
			instance.transform.SetParent (toInstantiate.transform);
		}
	}

	private void SaveInstanceText(string text, Text myText){
		if (text.Equals ("Score"))
			instanceScore = myText;
		else if(text.Equals(textNbGuard)){
			instanceNbGuard = myText;
		} else if(text.Equals(textLifeGuard)){
			instanceLifeGuard = myText;
		}
	}


	public void TextMenu(string text, Vector2 topRight, Vector2 bottomLeft, GameObject parent, Color c){
		GameObject toInstantiate = new GameObject ("text" + text);
		Text myText = toInstantiate.AddComponent<Text> ();
		SaveInstanceText (text, myText);
		myText.text = text;
		myText.font = myFont;
		myText.color = c;
		myText.alignment = TextAnchor.MiddleCenter;
		myText.fontSize = 20*Screen.width/1000;
		//Debug.Log (myText.fontSize+" : font size");
		myText.resizeTextForBestFit = true;
		myText.resizeTextMaxSize = myText.fontSize;
		var rect = myText.GetComponent<RectTransform> ();
		InitRectTransform (rect, topRight, bottomLeft, parent);
	}

	public void CreateCanvas(GameObject myObject, string layer){
		myCanvas = myObject.AddComponent<Canvas> ();
		myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		myCanvas.worldCamera = GetComponent<CameraManager> ().GetCamera ();
		myCanvas.sortingLayerName = layer;
		myObject.AddComponent<GraphicRaycaster> ();
	}

	public void CreateMenu (string nameMenu, Color panelColor, Color imageColor, Button[] tab, Vector4[] cornerTab, int i)
	{
		if (nameMenu != "Potion") {
			GameObject toInstantiate = new GameObject (nameMenu);
			instanceMenu [i] = toInstantiate;
			toInstantiate.tag = "Menu";
			CreateCanvas (toInstantiate, "UIMenu");

			//Panel => bloc les clique sur les bouttons en arriere plan
			GameObject toInstantiatePanel = new GameObject ("Panel");
			Image myPanel = toInstantiatePanel.AddComponent<Image> ();
			myPanel.color = panelColor;
			CreatePanel (myPanel, new Vector2 (1f, 1f), new Vector2 (0f, 0f));
			//Clone l'image objet du menu passee en parametre
			Image myImage = Instantiate (ImageMenu, Vector3.zero, Quaternion.identity) as Image;
			CreatePanel (myImage, new Vector2 (0.8f, 0.8f), new Vector2 (0.2f, 0.2f));

			if (nameMenu == "Option") {
				for (int j = 0; j < instanceVolume.Length; j++) {
					ConfigOption (myImage.gameObject, sliderVolume [j], sliderVolume [j].name, posSliderVolume, j);
				}
			}

			if (nameMenu == "Gardes") {
				ConfigUpgrade (myImage.gameObject);
			}

			//ajout des bouttons
			if (nameMenu == "Coffre du jour" || nameMenu == "Création des potions") {
				AddButton (tab, cornerTab, new Vector2 (0f, 0f), myImage.gameObject);
			} else {
				AddButton (tab, cornerTab, myImage.rectTransform.anchorMin, toInstantiate);
			}

			TextMenu (nameMenu, new Vector2 (1f, 1f), new Vector2 (0f, 0.8f), myImage.gameObject, new Color (0, 0, 0));
			instanceMenu [i].SetActive (false);
		}
	}

	public GameObject CreateInstance (GameObject toInstantiate, Vector2 topRight, Vector2 bottomLeft, GameObject canvas)
	{
		GameObject instance = Instantiate (toInstantiate, Vector3.zero, Quaternion.identity) as GameObject;
		var rectTransform = instance.GetComponent <RectTransform> ();
		InitRectTransform (rectTransform, topRight, bottomLeft, canvas);
		instance.transform.localScale = new Vector3 (1f, 1f, 1f);
		return instance;
	}

	public Button CreateInstance(GameObject toInstantiate, Vector2 topRight, Vector2 bottomLeft)
	{
		Button instance = Instantiate (toInstantiate.GetComponent<Button>(), Vector3.zero, Quaternion.identity) as Button;
		var rectTransform = instance.GetComponent <RectTransform> ();
		InitRectTransform (rectTransform, topRight, bottomLeft, myCanvas.gameObject);
		instance.transform.localScale = new Vector3 (1f, 1f, 1f);
		return instance;
	}

	public void AddImage (Image[] bar, Vector4[] posBar, GameObject toInstantiate){
		Image toInstantiateImage;
		for (int i = 0; i < bar.Length; i++) {
			toInstantiateImage = bar[i];
			instanceBar[i] = CreateInstance (toInstantiateImage.gameObject, new Vector2 (posBar [i].x, posBar [i].y), new Vector2 (posBar [i].z, posBar [i].w), myCanvas.gameObject).GetComponent<Image>();
			instanceBar[i].transform.SetParent (toInstantiate.transform);
		}
	}

	public Image[] GetInstanceBar(){
		return instanceBar;
	}

	public GameObject GetCoffreDuJour(){
		return instanceMenu[buttonTab.Length]; 
	}

	public void SetScoreText(int i){
		instanceScore.text = "Score : " + i.ToString ();
	}
}
