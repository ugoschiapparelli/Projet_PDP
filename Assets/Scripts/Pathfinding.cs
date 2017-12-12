using UnityEngine;
using System.Collections;

public class Pathfinding : MonoBehaviour {
	
	private GameObject gameController;
	private ChestManager chestManager;
	private TouchMonster touchMonster;
	private float speed;
	private Vector2 house;
	private Vector2 target;
	private Node root;
	private int [,] grid;
	private Node[] openList;
	private Node[] closeList;
	private int indiceOpenList;
	private int indiceCloseList;
	private int taille;
	private int columns;
	private int rows;
	private Vector2[] path;
	private int indicePath;
	private bool col;
	private GameObject[] tabGuard;
	private bool focusGuard;
	private bool focusHome;
	private bool focusOut;
	private GameObject nearGuard;
	private Vector2 targetOut;
	private bool isDead;

	private Transform boardHolder;

	private class Node{
		int countG;
		int countF;
		int countH;
		Node parent;
		Vector2 coor;

		public Node(Vector2 v, Node parent){
			this.coor = v;
			this.parent = parent;
			countF = 0;
			countG = 0;
			countH = 0;
		}

		public Node(Vector2 v){
			this.coor = v;
			countF = 0;
			countG = 0;
			countH = 0;
		}

		public Node(){
			this.coor = new Vector2(-1, -1);
			countF = 0;
			countG = 0;
			countH = 0;
		}

		public Node(Vector2 v, Node parent, int g, int h){
			this.coor = v;
			this.parent = parent;
			this.countF = g + h;
			this.countG = g;
			this.countH = h;
		}

		public int CountG{
			get{ return countG; }
			set{ countG = value; }
		}

		public int CountF{
			get{ return countF; }
			set{ countF = value; }
		}

		public int CountH{
			get{ return countH; }
			set{ countH = value; }
		}

		public Node Parent{
			get{ return parent; }
			set{ parent = value; }
		}

		public Vector2 Coor{
			get{ return coor; }
			set{ coor = value; }
		}
	}
		

	void InitPath(){
		indicePath = 0;
		path = new Vector2[taille];
		for(int i = 0; i < taille; i++){
			path [i] = new Vector2 (0, 0);
		}
	}
		

	// Use this for initialization
	void Start () {
		speed = 5;
		gameController = GameObject.Find ("_GameManager");
		BoardManager bm = gameController.GetComponent<BoardManager> ();
		house = gameController.GetComponent<HomeManager> ().GetDoor ();
		columns = bm.GetColumnsDecalage();
		rows = bm.GetRowsDecalage();
		tabGuard = gameController.GetComponent<GuardManager> ().GetTabGuard ();
		chestManager = gameController.GetComponent<ChestManager> (); 
		touchMonster = GetComponent<TouchMonster> ();
		nearGuard = NearGuard ();
		if (nearGuard.GetComponent<Life> ().GetAlive() == false) {
			focusGuard = false;
			focusHome = true;
			target = house;
		} else {
			focusGuard = true;
			focusHome = false;
			target = nearGuard.transform.position;
		}
		isDead = false;
		focusOut = false;
		target = new Vector2 ((int)target.x, (int)target.y);
		taille = columns * rows;
		grid = bm.GetGrid();
		col = false;
		Vector2 start = new Vector2 (transform.position.x, transform.position.y);
		targetOut = start;
		UpdatePath (start, target);
		GetPath (start);
	}

	GameObject NearGuard(){
		int nearDist = Distance(new Vector2(0, 0), new Vector2(columns, rows)); //Plus grande distance possible
		int indice = 0;
		int dist;
		for (int i = 0; i < tabGuard.Length; i++) {
			if (tabGuard [i].gameObject) {
				dist = Distance (tabGuard [i].transform.position, transform.position); 
				if (dist < nearDist) {
					nearDist = dist;
					indice = i;
				}
			}
		}
		return tabGuard [indice];
	}

	int Distance(Vector2 p1, Vector2 p2){
		//carré de la distance euclidienne 
		return (int) ((p1.x-p2.x)*(p1.x-p2.x) + (p1.y-p2.y)*(p1.y-p2.y));
	}

	int IsInList(Node[] list, int x, int y, int indice){
		for (int i = 0; i < indice; i++) {
			if (list [i].Coor.x == x && list[i].Coor.y == y)
				return i;
		}
		return -1;
	}

	void AddCaseAdjacent(Node[] list, Node n, Node end, int indice){
		for (int x = (int) n.Coor.x - 1; x <= (int) n.Coor.x + 1; x++) {
			for (int y = (int) n.Coor.y - 1; y <= (int) n.Coor.y + 1; y++) {
				if (y > -1 && y < rows + 1 && x > -1 && x < columns + 1){  
					if (x != n.Coor.x || y != n.Coor.y) {
						if (this.grid [x, y] != 1) {
							AddOpenList (n, end, x, y);
						}
					}
				}
			}
		}
	}

	void AddOpenList(Node p, Node end, int x, int y){
		if(IsInList(closeList, x, y, indiceCloseList) == -1){
			Vector2 v = new Vector2 (x, y);
			Node n = new Node (v, p, p.CountG + Distance (v, p.Coor), Distance (v, end.Coor));
			int tmp = IsInList (openList, x, y, indiceOpenList); 
			if(tmp != -1){
				if (n.CountF < openList[tmp].CountF ) {
					openList [tmp] = n;
					indiceOpenList++;
				}
			} else {
				openList [indiceOpenList] = n;
				indiceOpenList++;
			}
		}
	}

	int BestNode(Node[] list, int indice){
		float countFTmp = list [0].CountF;
		Node nTmp = list [0];
		int ret = 0;
		for (int i = 1; i < indice; i++) {
			if (countFTmp > list [i].CountF && list[i].Coor.x != -1) {
				countFTmp = list [i].CountF;
				nTmp = list [i];
				ret = i;
			}
		}
		return ret;
	}

	void AddCloseList(int i){
		closeList [indiceCloseList] = openList [i];
		indiceCloseList++;

		for(int j = i; j < indiceOpenList; j++){
			openList[j]= openList[j+1];
		}
		indiceOpenList--;
	}

	void UpdatePath(Vector2 start, Vector2 end){
		
		indiceOpenList = 0;
		openList = new Node[taille];
		for (int i = 0; i < taille; i++) {
			openList [i] = new Node ();
		}

		indiceCloseList = 0;
		closeList = new Node[taille];
		for (int i = 0; i < taille; i++) {
			closeList [i] = new Node ();
		}

		Node current = new Node(new Vector2 (start.x, start.y));
		Node s = new Node (start);
		Node e = new Node (end);

		openList [indiceOpenList] = s;
		indiceOpenList++;
		AddCloseList (0);
		AddCaseAdjacent (openList, s, e, indiceOpenList);

		while (indiceOpenList != 0 && // Tant que la liste ouverte n'est pas vide
		      !current.Coor.Equals (e.Coor)) { //Et que la destination n'a pas été atteinte
			int c = BestNode (openList, indiceOpenList);
			current = openList [c];
			AddCloseList (c);
			AddCaseAdjacent (openList, current, e, indiceOpenList);
		}

		if (!current.Coor.Equals (e.Coor)) { // Si je trouve pas de chemin
			Destroy (this.gameObject);
		}
	}

	void InverseTab(Vector2[] tab, int size){
		Vector2[] tmp = new Vector2 [size];
		int j = size - 1;
		for (int i = 0; i < size; i++) {
			tmp [i] = new Vector2 (0, 0);
			tmp [i] = tab [j];
			j--;
		}
		for (int i = 0; i < size; i++)
			tab [i] = tmp [i];
	}

	void GetPath(Vector2 start){
		InitPath ();
		int indice = 0;
		path [indice] = closeList [indiceCloseList - 1].Coor;
		indice++;

		Node current = new Node ();
		current = closeList [indiceCloseList - 1];

		while(!current.Coor.Equals(start) && current.Parent !=null){
			current = current.Parent;
			Vector2 tmp = new Vector2 (current.Coor.x, current.Coor.y); 
			path [indice] = tmp;
			indice++;
		}

		path [indice] = closeList [0].Coor;
		indice++;
						
		InverseTab (path, indice);
	}

	void Update(){
		float step = speed * Time.deltaTime;
		if (!isDead) {
			if (transform.position.x != target.x ||
			   transform.position.y != target.y) {

				if (col == false) {
					transform.position = Vector2.MoveTowards (transform.position, 
						path [indicePath], step); 
					if (transform.position.x == path [indicePath].x &&
					   transform.position.y == path [indicePath].y) {
						indicePath++;
					}
				}
			}
			if (focusGuard && nearGuard.GetComponent<Life> ().GetAlive () == false) {
				ChangeTarget (house);
				focusGuard = false;
				focusHome = true;
			}
			if (focusHome && transform.position.x == house.x && transform.position.y == house.y) {
				ChangeTarget (targetOut);
				chestManager.CandySteal ();
				focusHome = false;
				focusOut = true;
			}
			if (focusOut && transform.position.x == target.x && transform.position.y == target.y) {
				GetComponent<TouchMonster> ().DestroyMonster ();
			}
		}
	}

	public void SetColFalse(){
		col = false;
	}

	public void SetColTrue(){
		col = true;
	}

	void ChangeTarget(Vector2 newTarget){
		target = new Vector2 ((int)newTarget.x, (int)newTarget.y);
		col = false;
		Vector2 start = new Vector2 ((int)transform.position.x, (int)transform.position.y);
		UpdatePath (start, target);
		GetPath (start);

	}

	public void SetIsDead(){
		isDead = true;
	}

	public bool GetIsDead(){
		return isDead;
	}

	public bool GetFocusOut(){
		return focusOut;
	}
}
		
  
		