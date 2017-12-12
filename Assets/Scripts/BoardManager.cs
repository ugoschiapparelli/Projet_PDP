using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

public class BoardManager : MonoBehaviour
{

    private int columns;                                         //Number of columns in our game board.
    private int rows;                                            //Number of rows in our game board.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.

    public int nbObstacles = 3;
    public GameObject[] obstacleTiles;
    public GameObject[] treeTiles;
    public GameObject[] rockTiles;

	private int decalage = 4;
	private int[,] grid;
    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private Transform obstacleHolder;
    private Transform treeHolder;
    private Transform rockHolder;
    private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.


    public int GetColumns()
    {
        return columns;
    }

    public int GetRows()
    {
        return rows;
    }

	public int GetColumnsDecalage()
	{
		return columns + decalage;
	}

	public int GetRowsDecalage()
	{
		return rows + decalage;
	}

	public int GetDecalage(){
		return decalage;
	}

	public int[,] GetGrid(){
		return grid;
	}

	void InitGrid(){
		grid = new int[columns + decalage*2, rows + decalage*2];
		for (int i = 0; i < columns + decalage*2; i++) {
			for (int j = 0; j < rows + decalage*2; j++) {
				grid [i, j] = 0;
			}
		}
	}


    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 2; x < columns; x++)
        {
            for (int y = 2; y < rows; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;
		for (int x = 0-GetDecalage(); x <= GetColumnsDecalage(); x++)
        {
			for (int y = 0-GetDecalage(); y <= GetRowsDecalage(); y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate;

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == columns + 1 || y == -1 || y == rows + 1)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                else
                {
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                }

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    void ObstacleSetup()
    {
      
      obstacleHolder = new GameObject("Obstacles").transform;
      GameObject toInstantiate;
      Vector2 homePosition = GetComponent<HomeManager> ().GetHomePosition ();
      float homeSize = GetComponent<HomeManager> ().GetHomeSize ();
      
      int homeSizeX = GetComponent<HomeManager> ().GetSizeX ();
      int homeSizeY = GetComponent<HomeManager> ().GetSizeY ();
      for (int i = (int) homePosition.x - homeSizeX/2; i < (int) homePosition.x + homeSizeX/2; i++) {
	  for (int j = (int) homePosition.y - homeSizeY/2; j < (int) homePosition.y + homeSizeY/2; j++) {
	     if (j < homePosition.y && i > homePosition.x) {
	        grid [i, j] = 0;
	     } else {
	        grid [i, j] = 1;
	     }
	   }
     }

      bool notFloor = true;
      for (int i = 0; i < nbObstacles; i++)
        {
	  while (notFloor)
            {
	      Vector3 pos = RandomPosition();
	      if (grid[(int)pos.x, (int)pos.y] == 0)
                {
		  notFloor = false;
		  toInstantiate = obstacleTiles[Random.Range(0, obstacleTiles.Length)];
		  grid [(int) pos.x, (int) pos.y] = 1;
		        pos.x = (int)pos.x;
		        pos.y = (int)pos.y;
		  GameObject instance = Instantiate(toInstantiate, pos, Quaternion.identity) as GameObject;
		  instance.transform.SetParent(obstacleHolder);
                }
            }
            notFloor = true;
        }
    }

    void DecorSetup()
    {
        treeHolder = new GameObject("tree").transform;
        rockHolder = new GameObject("rock").transform;
        int x, y, z;
		x = 0;
		y = -2;
		z = rows / 4;

        //left/down
        while (x < columns / 4)
        {
            while (y <= z)
            {
				if ((y % 2 == 0 || y==-2) && (x % 2 == 0))
                {
                    GameObject toInstantiate = treeTiles[Random.Range(0, treeTiles.Length)];
                    GameObject instance;
					instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(treeHolder);
                }
                y++;
            }
            z--;
            y = -2;
            x++;
        }
        //right/down
        x = columns;
        y = -2;
        z = rows / 4;
        while (x > columns - columns / 4)
        {
            while (y <= z)
            {
				if ((y % 2 == 0 || y==-2) && (x % 2 == 0))
                {
                    GameObject toInstantiate = treeTiles[Random.Range(0, treeTiles.Length)];
                    GameObject instance;
					instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(treeHolder);
                }
                y++;
            }
            z--;
            y = -2;
            x--;
        }
        //left/top
        x = 0;
        y = rows+1;
        z = rows - rows / 3;
        while (x < columns / 3)
        {
            while (y >= z)
            {
                GameObject toInstantiate = rockTiles[Random.Range(0, rockTiles.Length)];
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(rockHolder);
                y--;
            }
            if (x % 2 == 0)
                z++;
            y = rows+1;
            x++;
        }
        //Right/top
        x = columns;
        y = rows+1;
        z = rows - rows / 3;
        while (x > columns - columns / 3)
        {
            while (y >= z)
            {
                GameObject toInstantiate = rockTiles[Random.Range(0, rockTiles.Length)];
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(rockHolder);
                y--;
            }
            if (x % 2 == 0)
                z++;
            y = rows+1;
            x--;
        }
    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene(int columns, int rows, int nbObstable, bool readData)
    {
		this.columns = columns;
		this.rows = rows;
		if (readData) {
			this.nbObstacles = nbObstable;
		}
      //Creates the outer walls and floor.
        BoardSetup();

        InitialiseList();
		InitGrid ();

        ObstacleSetup();

        //Reset our list of gridpositions.
        InitialiseList();

        DecorSetup();

        InitialiseList();
    }
}
