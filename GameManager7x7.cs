using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager7x7 : MonoBehaviour 
{
	public static GameManager7x7 gm;
	public GameObject XPiece;
	public GameObject OPiece;
	public GameObject BPiece;
	private int currPlayer;
	public GUIText prompt;
	public Square7x7[] aSquares;
	public Square7x7[,] aGrid;
	private List<Square7x7> aWinOportunities;
	private List<Square7x7> aBlockOportunities;
	private bool gameOver = false;
	private int moves = 0;
	private IEnumerator coroutine;
	private float rng = 0;

	private short numberOrBlocks = 7;

	public bool p1Won = false;
	public bool p2Won = false;

	public GUIText scoreBoard;
	public static int P1Score = 0;
	public static int P2Score = 0;


	// Use this for initialization
	void Start () 
	{
		p1Won = false;
		p2Won = false;

		if (gm == null) 
		{
			gm = gameObject.GetComponent<GameManager7x7>();
		}
		aSquares = FindObjectsOfType(typeof(Square7x7)) as Square7x7[];

		aGrid = new Square7x7[7, 7];

		Square7x7 theSquare;

		for (int i = 0; i < aSquares.Length; i++) 
		{
			theSquare = aSquares [i];
			aGrid [theSquare.r, theSquare.c] = theSquare;
		}

		currPlayer = 3;

		for (short b = 3; b < numberOrBlocks+3; b++) 
		{
			BlockRandomSquares (b);
			currPlayer++;
			moves++;
		}

		currPlayer = 1;
		ShowPlayerPrompt();
		ShowScore ();
	}

	void BlockRandomSquares(short fauxPlayer)
	{
		currPlayer = fauxPlayer;
		Square7x7 theSquare;
		List<Square7x7> aEmptySquare = new List<Square7x7>();
		for (int i = 0; i < aSquares.Length; i++) 
		{
			theSquare = aSquares[i];
			if (theSquare.player == 0) 
			{
				aEmptySquare.Add(theSquare);
			}
		}
		theSquare = aEmptySquare[Random.Range (0, aEmptySquare.Count)];
		PlacePiece (BPiece, theSquare);
	}

	public void ClickSquare(Square7x7 other)
	{
		if (gameOver) 
		{
			return;
		}
		if (currPlayer == 1) {
			PlacePiece (XPiece, other);
			if(moves <= 48 && currPlayer != 1 && gameOver == false)
			{
				StartCoroutine (ComputerWaitUntilTakingTurn ());
			}
		} 
		/*else 
		{
			PlacePiece (OPiece, other);
		}*/
	}

	IEnumerator ComputerWaitUntilTakingTurn()
	{
		yield return new WaitForSeconds (3);
		ComputerTakeATurn();
	}

	void ComputerTakeATurn()
	{
		Square7x7 theSquare;
		theSquare = NullifyTheSquare();

		rng = Random.value;
		if (rng >= 0.5f) 
		{
			theSquare = WinOrBlock();
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = CreateOrPreventTrap();
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetCentre();
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyOuterCorner();
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyMiddleCorner();	
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyInnerCorner();	
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyOutsideSidesCenter();	
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyMiddleSidesCenter();	
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyInnerSides();	
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyMiddleSidesSide();	
		}

		rng = Random.value;
		if (theSquare == null && rng > 0.5f) 
		{
			theSquare = GetEmptyOutsideSidesSide();	
		}

		if (theSquare == null) 
		{
			theSquare = GetRandomEmptySquare();	
		}


		PlacePiece (OPiece, theSquare);
		//return theSquare;
	}

	Square7x7 CreateOrPreventTrap()
	{
		int p1ICorners = 0;
		int p2ICorners = 0;

		int p1MCorners = 0;
		int p2MCorners = 0;

		int p1OCorners = 0;
		int p2OCorners = 0;

		Square7x7[] OutsideCorners = new Square7x7[] {
			aGrid[0, 0], aGrid[0, 6],
			aGrid[6, 0], aGrid[6, 6]
		};

		Square7x7[] MiddleCorners = new Square7x7[] {
			aGrid[1, 1], aGrid[1, 5],
			aGrid[5, 1], aGrid[5, 5]
		};

		Square7x7[] InsideCorners = new Square7x7[] {
			aGrid[2, 2], aGrid[2, 4],
			aGrid[4, 2], aGrid[4, 4]
		};

		foreach (Square7x7 O in OutsideCorners) 
		{
			if (O.player == 1) 
			{
				p1OCorners++;
			}
			if (O.player == 2) 
			{
				p2OCorners++;
			}
		}

		foreach (Square7x7 M in MiddleCorners) 
		{
			if (M.player == 1) 
			{
				p1MCorners++;
			}
			if (M.player == 2) 
			{
				p2MCorners++;
			}
		}

		foreach (Square7x7 I in InsideCorners) 
		{
			if (I.player == 1) 
			{
				p1ICorners++;
			}
			if (I.player == 2) 
			{
				p2ICorners++;
			}
		}

		if (p1OCorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyOutsideSidesCenter");
			return GetEmptyOutsideSidesCenter();
		}
		if (p2OCorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyOuterCorner");
			return GetEmptyOuterCorner ();
		}

		if (p1MCorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyMiddleSidesCenter");
			return GetEmptyMiddleSidesCenter();
		}
		if (p2MCorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyMiddleCorner");
			return GetEmptyMiddleCorner ();
		}

		if (p1ICorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyInnerSides");
			return GetEmptyInnerSides();
		}
		if (p2ICorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyInnerCorner");
			return GetEmptyInnerCorner ();
		}
		return null;
	}

	Square7x7 WinOrBlock()
	{
		aWinOportunities = new List<Square7x7> ();
		aBlockOportunities = new List<Square7x7> ();
		float index = 0;
		Square7x7 theSquare = null;

		for (index = 0; index < 7; index++) // for rows
		{
			CheckForTwo(new Vector2[]{new Vector2(index, 0), new Vector2(index, 1), new Vector2(index, 2), new Vector2(index, 3), new Vector2(index, 4), new Vector2(index, 5), new Vector2(index, 6)});
		}

		for (index = 0; index < 7; index++) //for all columns
		{
			CheckForTwo(new Vector2[]{new Vector2(0, index), new Vector2(1, index), new Vector2(2, index), new Vector2(3, index), new Vector2(4, index), new Vector2(5, index), new Vector2(6, index)});
		}

		CheckForTwo(new Vector2[]{new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 3), new Vector2(4, 4), new Vector2(5, 5), new Vector2(6, 6)}); // for the main diagonal UL - DR
		CheckForTwo(new Vector2[]{new Vector2(0, 6), new Vector2(1, 5), new Vector2(2, 4), new Vector2(3, 3), new Vector2(4, 2), new Vector2(5, 1), new Vector2(6, 0)}); // for the main diagonal DL - UR

		CheckForTwo(new Vector2[]{new Vector2(2, 0), new Vector2(1, 1), new Vector2(0, 2)}); // for the 20 11 02 diagonal
		CheckForTwo(new Vector2[]{new Vector2(4, 0), new Vector2(5, 1), new Vector2(6, 2)}); // for the 40 51 62 diagonal
		CheckForTwo(new Vector2[]{new Vector2(4, 6), new Vector2(5, 5), new Vector2(6, 4)}); // for the 46 55 64 diagonal
		CheckForTwo(new Vector2[]{new Vector2(2, 6), new Vector2(1, 5), new Vector2(0, 4)}); // for the 26 15 04 diagonal

		CheckForTwo(new Vector2[]{new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3), new Vector2(3, 4), new Vector2(4, 5), new Vector2(5, 6)}); // for the 01 - 56 diagonal
		CheckForTwo(new Vector2[]{new Vector2(1, 0), new Vector2(2, 1), new Vector2(3, 2), new Vector2(4, 3), new Vector2(5, 4), new Vector2(6, 5)}); // for the 10 - 65 diagonal
		CheckForTwo(new Vector2[]{new Vector2(6, 1), new Vector2(5, 2), new Vector2(4, 3), new Vector2(3, 4), new Vector2(2, 5), new Vector2(1, 6)}); // for the 61 - 16 diagonal
		CheckForTwo(new Vector2[]{new Vector2(5, 0), new Vector2(4, 1), new Vector2(3, 2), new Vector2(2, 3), new Vector2(1, 4), new Vector2(0, 5)}); // for the 50 - 05 diagonal

		CheckForTwo(new Vector2[]{new Vector2(2, 0), new Vector2(3, 1), new Vector2(4, 2), new Vector2(5, 3), new Vector2(6, 4)}); // for the 20 - 64 diagonal
		CheckForTwo(new Vector2[]{new Vector2(0, 2), new Vector2(1, 3), new Vector2(2, 4), new Vector2(3, 5), new Vector2(4, 6)}); // for the 02 - 46 diagonal
		CheckForTwo(new Vector2[]{new Vector2(4, 0), new Vector2(3, 1), new Vector2(2, 2), new Vector2(1, 3), new Vector2(0, 4)}); // for the 40 - 04 diagonal
		CheckForTwo(new Vector2[]{new Vector2(6, 2), new Vector2(5, 3), new Vector2(4, 4), new Vector2(3, 5), new Vector2(2, 6)}); // for the 62 - 26 diagonal

		CheckForTwo(new Vector2[]{new Vector2(0, 3), new Vector2(1, 4), new Vector2(2, 5), new Vector2(3, 6)}); // for the 03 - 36 diagonal
		CheckForTwo(new Vector2[]{new Vector2(3, 0), new Vector2(4, 1), new Vector2(5, 2), new Vector2(6, 3)}); // for the 30 - 63 diagonal
		CheckForTwo(new Vector2[]{new Vector2(3, 0), new Vector2(2, 1), new Vector2(1, 2), new Vector2(0, 3)}); // for the 30 - 03 diagonal
		CheckForTwo(new Vector2[]{new Vector2(6, 3), new Vector2(5, 4), new Vector2(4, 5), new Vector2(3, 6)}); // for the 63 - 36 diagonal

		if (aWinOportunities.Count > 0)
		{
			return aWinOportunities[Random.Range(0, aWinOportunities.Count)];
		}

		if (aBlockOportunities.Count > 0)
		{
			return aBlockOportunities[Random.Range(0, aBlockOportunities.Count)];
		}
		return theSquare;
	}

	Square7x7 CheckForTwo(Vector2 [] coords)
	{
		Square7x7 theSquare = null;
		Vector2 coord1 = new Vector2 ();
		Vector2 coord2 = new Vector2 ();
		Vector2 coord3 = new Vector2 ();

		for (int i = 2; i < coords.Length; i++) 
		{
			coord1 = coords [i-2];
			coord2 = coords [i-1];
			coord3 = coords [i];
			if (GetPlayer ((int)coord1.x, (int)coord1.y) == 2 && GetPlayer ((int)coord2.x, (int)coord2.y) == 2 && GetPlayer((int)coord3.x, (int)coord3.y) == 0) 
			{
				Debug.Log("Adding: " + coord3.x + ", " + coord3.y + "for winning");
				theSquare = aGrid [(int)coord3.x, (int)coord3.y];
				aWinOportunities.Add(theSquare);
			}
			if (GetPlayer ((int)coord2.x, (int)coord2.y) == 2 && GetPlayer ((int)coord3.x, (int)coord3.y) == 2 && GetPlayer((int)coord1.x, (int)coord1.y) == 0)
			{
				Debug.Log("Adding: " + coord1.x + ", " + coord1.y + "for winning");
				theSquare = aGrid [(int)coord1.x, (int)coord1.y];
				aWinOportunities.Add(theSquare);
			}
			if (GetPlayer ((int)coord1.x, (int)coord1.y) == 2 && GetPlayer ((int)coord3.x, (int)coord3.y) == 2 && GetPlayer((int)coord2.x, (int)coord2.y) == 0) 
			{
				Debug.Log("Adding: " + coord2.x + ", " + coord2.y + "for winning");
				theSquare = aGrid [(int)coord2.x, (int)coord2.y];
				aWinOportunities.Add(theSquare);
			}
		}

		for (int i = 2; i < coords.Length; i++) 
		{
			coord1 = coords [i-2];
			coord2 = coords [i-1];
			coord3 = coords [i];
			if (GetPlayer ((int)coord1.x, (int)coord1.y) == 1 && GetPlayer ((int)coord2.x, (int)coord2.y) == 1 && GetPlayer((int)coord3.x, (int)coord3.y) == 0) 
			{
				Debug.Log("Adding: " + coord3.x + ", " + coord3.y + "for blocking");
				theSquare = aGrid [(int)coord3.x, (int)coord3.y];
				aBlockOportunities.Add(theSquare);
			}
			if (GetPlayer ((int)coord2.x, (int)coord2.y) == 1 && GetPlayer ((int)coord3.x, (int)coord3.y) == 1 && GetPlayer((int)coord1.x, (int)coord1.y) == 0)
			{
				Debug.Log("Adding: " + coord1.x + ", " + coord1.y + "for blocking");
				theSquare = aGrid [(int)coord1.x, (int)coord1.y];
				aBlockOportunities.Add(theSquare);
			}
			if (GetPlayer ((int)coord1.x, (int)coord1.y) == 1 && GetPlayer ((int)coord3.x, (int)coord3.y) == 1 && GetPlayer((int)coord2.x, (int)coord2.y) == 0) 
			{
				Debug.Log("Adding: " + coord2.x + ", " + coord2.y + "for blocking");
				theSquare = aGrid [(int)coord2.x, (int)coord2.y];
				aBlockOportunities.Add(theSquare);
			}
		}
		
		return null;
	}

	Square7x7 GetEmptyOutsideSidesSide()
	{
		List<Square7x7> aEmptyOutsideSidesSide = new List<Square7x7>();	

		if (GetPlayer (0, 1) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [0, 1]);
		}
		if (GetPlayer (0, 2) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [0, 2]);
		}
		if (GetPlayer (0, 4) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [0, 4]);
		}
		if (GetPlayer (0, 5) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [0, 5]);
		}

		if (GetPlayer (1, 0) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [1, 0]);
		}
		if (GetPlayer (1, 6) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [1, 6]);
		}

		if (GetPlayer (2, 0) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [2, 0]);
		}
		if (GetPlayer (2, 6) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [2, 6]);
		}

		if (GetPlayer (4, 0) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [4, 0]);
		}
		if (GetPlayer (4, 6) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [4, 6]);
		}

		if (GetPlayer (5, 0) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [5, 0]);
		}
		if (GetPlayer (5, 6) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [5, 6]);
		}

		if (GetPlayer (6, 1) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [6, 1]);
		}
		if (GetPlayer (6, 2) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [6, 2]);
		}
		if (GetPlayer (6, 4) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [6, 4]);
		}
		if (GetPlayer (6, 5) == 0) 
		{
			aEmptyOutsideSidesSide.Add (aGrid [6, 5]);
		}

		if (aEmptyOutsideSidesSide.Count > 0) 
		{
			Debug.Log ("Returning an empty middle side side");
			return aEmptyOutsideSidesSide [Random.Range (0, aEmptyOutsideSidesSide.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyMiddleSidesSide()
	{
		List<Square7x7> aEmptyMiddleSidesSide = new List<Square7x7>();	

		if (GetPlayer (1, 2) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [1, 2]);
		}
		if (GetPlayer (1, 4) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [1, 4]);
		}

		if (GetPlayer (2, 1) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [2, 1]);
		}
		if (GetPlayer (2, 5) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [2, 5]);
		}

		if (GetPlayer (4, 1) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [4, 1]);
		}
		if (GetPlayer (4, 5) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [4, 5]);
		}

		if (GetPlayer (5, 2) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [5, 2]);
		}
		if (GetPlayer (5, 4) == 0) 
		{
			aEmptyMiddleSidesSide.Add (aGrid [5, 4]);
		}

		if (aEmptyMiddleSidesSide.Count > 0) 
		{
			Debug.Log ("Returning an empty middle side side");
			return aEmptyMiddleSidesSide [Random.Range (0, aEmptyMiddleSidesSide.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyOutsideSidesCenter()
	{
		List<Square7x7> aEmptyOutsideSidesCenter = new List<Square7x7>();	

		if (GetPlayer (0, 3) == 0) 
		{
			aEmptyOutsideSidesCenter.Add (aGrid [0, 3]);
		}
		if (GetPlayer (3, 0) == 0) 
		{
			aEmptyOutsideSidesCenter.Add (aGrid [3, 0]);
		}
		if (GetPlayer (3, 6) == 0) 
		{
			aEmptyOutsideSidesCenter.Add (aGrid [3, 6]);
		}
		if (GetPlayer (6, 3) == 0) 
		{
			aEmptyOutsideSidesCenter.Add (aGrid [6, 3]);
		}

		if (aEmptyOutsideSidesCenter.Count > 0) 
		{
			Debug.Log ("Returning an empty outside side center");
			return aEmptyOutsideSidesCenter [Random.Range (0, aEmptyOutsideSidesCenter.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyMiddleSidesCenter()
	{
		List<Square7x7> aEmptyMiddleSidesCenter = new List<Square7x7>();	

		if (GetPlayer (1, 3) == 0) 
		{
			aEmptyMiddleSidesCenter.Add (aGrid [1, 3]);
		}
		if (GetPlayer (3, 1) == 0) 
		{
			aEmptyMiddleSidesCenter.Add (aGrid [3, 1]);
		}
		if (GetPlayer (3, 5) == 0) 
		{
			aEmptyMiddleSidesCenter.Add (aGrid [3, 5]);
		}
		if (GetPlayer (5, 3) == 0) 
		{
			aEmptyMiddleSidesCenter.Add (aGrid [5, 3]);
		}

		if (aEmptyMiddleSidesCenter.Count > 0) 
		{
			Debug.Log ("Returning an empty middle side center");
			return aEmptyMiddleSidesCenter [Random.Range (0, aEmptyMiddleSidesCenter.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyInnerSides()
	{
		List<Square7x7> aEmptyInnerSides = new List<Square7x7>();	

		if (GetPlayer (2, 3) == 0) 
		{
			aEmptyInnerSides.Add (aGrid [2, 3]);
		}
		if (GetPlayer (3, 2) == 0) 
		{
			aEmptyInnerSides.Add (aGrid [3, 2]);
		}
		if (GetPlayer (3, 4) == 0) 
		{
			aEmptyInnerSides.Add (aGrid [3, 4]);
		}
		if (GetPlayer (4, 3) == 0) 
		{
			aEmptyInnerSides.Add (aGrid [4, 3]);
		}

		if (aEmptyInnerSides.Count > 0) 
		{
			Debug.Log ("Returning an empty inside side");
			return aEmptyInnerSides [Random.Range (0, aEmptyInnerSides.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyInnerCorner()
	{
		List<Square7x7> aEmptyInnerCorners = new List<Square7x7>();	

		if (GetPlayer (2, 2) == 0) 
		{
			aEmptyInnerCorners.Add (aGrid [2, 2]);
		}
		if (GetPlayer (2, 4) == 0) 
		{
			aEmptyInnerCorners.Add (aGrid [2, 4]);
		}
		if (GetPlayer (4, 2) == 0) 
		{
			aEmptyInnerCorners.Add (aGrid [4, 2]);
		}
		if (GetPlayer (4, 4) == 0) 
		{
			aEmptyInnerCorners.Add (aGrid [4, 4]);
		}

		if (aEmptyInnerCorners.Count > 0) 
		{
			Debug.Log ("Returning an empty inner corner");
			return aEmptyInnerCorners [Random.Range (0, aEmptyInnerCorners.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyMiddleCorner()
	{
		List<Square7x7> aEmptyMiddleCorners = new List<Square7x7>();	

		if (GetPlayer (1, 1) == 0) 
		{
			aEmptyMiddleCorners.Add (aGrid [1, 1]);
		}
		if (GetPlayer (1, 5) == 0) 
		{
			aEmptyMiddleCorners.Add (aGrid [1, 5]);
		}
		if (GetPlayer (5, 1) == 0) 
		{
			aEmptyMiddleCorners.Add (aGrid [5, 1]);
		}
		if (GetPlayer (5, 5) == 0) 
		{
			aEmptyMiddleCorners.Add (aGrid [5, 5]);
		}

		if (aEmptyMiddleCorners.Count > 0) 
		{
			Debug.Log ("Returning an empty middle corner");
			return aEmptyMiddleCorners [Random.Range (0, aEmptyMiddleCorners.Count)];
		}
		return null;
	}

	Square7x7 GetEmptyOuterCorner()
	{
		List<Square7x7> aEmptyOuterCorners = new List<Square7x7>();	

		if (GetPlayer (0, 0) == 0) 
		{
			aEmptyOuterCorners.Add (aGrid [0, 0]);
		}
		if (GetPlayer (0, 6) == 0) 
		{
			aEmptyOuterCorners.Add (aGrid [0, 6]);
		}
		if (GetPlayer (6, 0) == 0) 
		{
			aEmptyOuterCorners.Add (aGrid [6, 0]);
		}
		if (GetPlayer (6, 6) == 0) 
		{
			aEmptyOuterCorners.Add (aGrid [6, 6]);
		}

		if (aEmptyOuterCorners.Count > 0) 
		{
			Debug.Log ("Returning an empty corner");
			return aEmptyOuterCorners [Random.Range (0, aEmptyOuterCorners.Count)];
		}

		return null;
	}

	Square7x7 GetCentre()
	{
		if (GetPlayer (3, 3) == 0) 
		{
			Debug.Log ("Returning Center");
			return aGrid [3, 3];
		}
		return null;
	}

	Square7x7 GetRandomEmptySquare()
	{
		Square7x7 theSquare;
		List<Square7x7> aEmptySquare = new List<Square7x7>();

		for (int i = 0; i < aSquares.Length; i++) 
		{
			theSquare = aSquares[i];
			if (theSquare.player == 0) 
			{
				aEmptySquare.Add(theSquare);
			}
		}
		theSquare = aEmptySquare[Random.Range (0, aEmptySquare.Count)];
		Debug.Log ("Returning Random Square");
		return theSquare;
	}

	Square7x7 NullifyTheSquare()
	{
		return null;
	}

	void PlacePiece(GameObject piece, Square7x7 other)
	{
		moves++;

		Instantiate (piece, other.gameObject.transform.position, Quaternion.identity);
		other.player = currPlayer;

		if (CheckForWin(other))
		{
			gameOver = true;
			StartCoroutine(ShowWinnerPrompt());
			return;
		}
		else if (moves >= 49) 
		{
			gameOver = true;
			StartCoroutine (ShowTiePrompt ());
			return;
		}
		currPlayer++;
		if (currPlayer > 2) 
		{
			currPlayer = 1;
		}
		ShowPlayerPrompt ();
	}

	bool CheckForWin(Square7x7 other)
	{
		int index = 0;
		int index2 = 0;

		for (index = 2; index < 7; index++) // checked OK -> checking rows 
		{
			if ((GetPlayer (other.r, index - 2) == currPlayer) && (GetPlayer (other.r, index - 1) == currPlayer) && (GetPlayer (other.r, index) == currPlayer)) 
			{
				return true;
			}
		}

		for (index = 2; index < 7; index++) // checked OK -> checking columns
		{
			if ((GetPlayer (index - 2, other.c) == currPlayer) && (GetPlayer (index - 1, other.c) == currPlayer) && (GetPlayer (index, other.c) == currPlayer)) 
			{
				return true;
			}
		}

		for (index = 2; index < 7; index++) // checked OK -> main diagonal UL-DR
		{
			if ((GetPlayer (index - 2, index - 2) == currPlayer) && (GetPlayer (index - 1, index - 1) == currPlayer) && (GetPlayer (index, index) == currPlayer)) 
			{
				return true;
			}
		}

		index2 = 7;
		for (index = 2; index < 7; index++) // checked OK -> main diagonal UR-DL
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		// checked OK -> 20 11 02 diagonal
		if ((GetPlayer (2, 0) == currPlayer) && (GetPlayer (1, 1) == currPlayer) && (GetPlayer (0, 2) == currPlayer)) 
		{
			return true;
		}

		// checked OK -> 46 55 64 diagonal
		if ((GetPlayer (4, 6) == currPlayer) && (GetPlayer (5, 5) == currPlayer) && (GetPlayer (6, 4) == currPlayer)) 
		{
			return true;
		}

		// checked OK -> 40 51 62 diagonal
		if ((GetPlayer (4, 0) == currPlayer) && (GetPlayer (5, 1) == currPlayer) && (GetPlayer (6, 2) == currPlayer)) 
		{
			return true;
		}

		// checked OK -> 04 15 26 diagonal
		if ((GetPlayer (0, 4) == currPlayer) && (GetPlayer (1, 5) == currPlayer) && (GetPlayer (2, 6) == currPlayer)) 
		{
			return true;
		}

		// checked OK -> 01 - 56 diagonal
		for (index = 2; index < 6; index++) 
		{
			if ((GetPlayer (index - 2, index - 1) == currPlayer) && (GetPlayer (index - 1, index) == currPlayer) && (GetPlayer (index, index + 1) == currPlayer)) 
			{
				return true;
			}
		}

		// checked OK -> 10 - 65 diagonal
		for (index = 2; index < 6; index++) 
		{
			if ((GetPlayer (index - 1, index - 2) == currPlayer) && (GetPlayer (index, index - 1) == currPlayer) && (GetPlayer (index + 1, index) == currPlayer)) 
			{
				return true;
			}
		}

		// checked OK -> 50 - 05 diagonal
		index2 = 6;
		for (index = 2; index < 6; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		// checked OK -> 51 - 16 diagonal
		index2 = 7;
		for (index = 3; index < 7; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		// checked OK -> 02 - 46 diagonal
		for (index = 4; index < 7; index++) 
		{
			if ((GetPlayer (index - 4, index - 2) == currPlayer) && (GetPlayer (index - 3, index - 1) == currPlayer) && (GetPlayer (index - 2, index) == currPlayer)) 
			{
				return true;
			}
		}

		// checked OK -> 20 - 64 diagonal
		for (index = 4; index < 7; index++) 
		{
			if ((GetPlayer (index - 2, index - 4) == currPlayer) && (GetPlayer (index - 1, index - 3) == currPlayer) && (GetPlayer (index, index - 2) == currPlayer)) 
			{
				return true;
			}		
		}

		//checked OK -> 04 - 40 diagonal
		index2 = 5;
		for (index = 2; index < 5; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		//checked OK -> 26 - 62 diagonal
		index2 = 7;
		for (index = 4; index < 7; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		//checked OK -> 03 - 36 diagonal
		for (index = 2; index < 4; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index + 1) == currPlayer) && (GetPlayer (index - 1, index + 2) == currPlayer) && (GetPlayer (index, index + 3) == currPlayer)) 
			{
				return true;
			}
		}

		//checked OK -> 30 - 36 diagonal
		for (index = 2; index < 4; index++) 
		{
			index2--;
			if ((GetPlayer (index + 1, index - 2) == currPlayer) && (GetPlayer (index + 2, index - 1) == currPlayer) && (GetPlayer (index + 3, index) == currPlayer)) 
			{
				return true;
			}
		}

		//checked OK -> 30 - 03 diagonal
		index2 = 4;
		for (index = 2; index < 4; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		//checked OK -> 63 - 36 diagonal
		index2 = 7;
		for (index = 5; index < 7; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}

		return false;
	}

	int GetPlayer(int r, int c)
	{
		return aGrid [r, c].player;
	}

	void ShowScore()
	{
		scoreBoard.text = "Current Score is: " + P1Score + " : " + P2Score;
	}

	IEnumerator ShowWinnerPrompt()
	{
		if (currPlayer == 1)
		{
			P1Score++;
			prompt.text = "X gets 3 in a row. Player 1 wins!";
		}
		else
		{
			P2Score++;
			prompt.text = "O gets 3 in a row. Player 2 wins!";
		}

		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(2);
	}

	void ShowPlayerPrompt()
	{
		if (currPlayer == 1)
		{
			prompt.text = "Player 1, place an X";
		}
		else
		{
			prompt.text = "Player 2, place an O";
		}
	}

	IEnumerator ShowTiePrompt()
	{
		prompt.text = "Tie! Neither player wins.";

		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(2);
	}


	// Update is called once per frame
	void Update () 
	{
		
	}
}
