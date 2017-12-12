using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour {

    public GameObject homeTile;

	private GameObject home;
	private Transform homeHolder;

	private float homeSize;
	private Vector2 homePosition;
	private int sizeX;
	private int sizeY;

	public float GetHomeSize(){
		return homeSize;
	}

	public Vector2 GetHomePosition(){
		return homePosition;
	}

	public Vector2 GetDoor(){
		int pX = (int) home.transform.position.x + sizeX/2;
		int pY = (int) home.transform.position.y - sizeY/2;
		return new Vector2 ((pX + homePosition.x) / 2, (pY + homePosition.y) / 2);
	}

	public int GetSizeX(){
		return sizeX;
	}

	public int GetSizeY(){
		return sizeY;
	}

	// Use this for initialization
	public void SetupHome(int columns, int rows) {

        homeHolder = new GameObject("Home").transform;
        GameObject toInstantiate = homeTile;
		homeSize = (toInstantiate.transform.localScale.magnitude * 2);
		homePosition = new Vector2 (columns/2 , rows / 2);
		home = Instantiate(toInstantiate, homePosition, Quaternion.identity) as GameObject;
        home.transform.SetParent(homeHolder);
		Vector2 sr = home.GetComponent<SpriteRenderer> ().sprite.bounds.size;
		sizeX = (int) sr.x;
		sizeY = (int) sr.y;
	}
}
