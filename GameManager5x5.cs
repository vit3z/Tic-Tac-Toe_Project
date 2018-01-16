using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager5x5 : MonoBehaviour 
{
    public static GameManager5x5 gm;

    public GameObject XPiece;
    public GameObject OPiece;
	public GameObject BPiece;

    private int currPlayer;

    public GUIText prompt;
	public GUIText scoreBoard;

    public Square5x5[] aSquares;
    public Square5x5[,] aGrid;

    private List<Square5x5> aWinOportunities;
    private List<Square5x5> aBlockOportunities;

    private bool gameOver = false;

    private int moves = 0;

    private IEnumerator coroutine;

    private float rng = 0;

	private short numberOrBlocks = 5;

	public bool p1Won = false;
	public bool p2Won = false;

	public static int P1Score = 0;
	public static int P2Score = 0;

	public bool PowerUpIsClicked = false;

	// Use this for initialization
	void Start () 
    {
		p1Won = false;
		p2Won = false;

        if (gm == null)
        {
            gm = gameObject.GetComponent<GameManager5x5>();
        }
        aSquares = FindObjectsOfType(typeof(Square5x5)) as Square5x5[];

        aGrid = new Square5x5[5, 5];

        Square5x5 theSquare;

        for (int i = 0; i < aSquares.Length; i++)
        {
            theSquare = aSquares[i];
            aGrid[theSquare.r, theSquare.c] = theSquare;
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
		Square5x5 theSquare;
		List<Square5x5> aEmptySquare = new List<Square5x5>();
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

    public void ClickSquare(Square5x5 other)
    {
        if (gameOver)
        {  
            return;
        }
        if (currPlayer == 1)
        {
            PlacePiece(XPiece, other);
			if (moves <= 24 && currPlayer != 1 && gameOver == false) 
			{
				StartCoroutine (ComputerWaitUntilTakingTurn());
			}
        }
    }

	IEnumerator ComputerWaitUntilTakingTurn()
	{
		yield return new WaitForSeconds (3);
		ComputerTakeATurn();
	}

	void ComputerTakeATurn()
	{
		Square5x5 theSquare;
		theSquare = NullifyTheSquare();

		rng = Random.value;
		if (rng >= 0) 
		{
			theSquare = WinOrBlock();	
		}

		rng = Random.value;
		if (theSquare == null && rng >= 0)
		{
			theSquare = CreateOrPreventTrap();
		}

		rng = Random.value;
		if (theSquare == null && rng >= 0)
		{
			theSquare = GetCentre();
		}

		rng = Random.value;
		if (theSquare == null && rng >= 0) 
		{
			theSquare = GetEmptyMiddleCircleSides();
		}

		rng = Random.value;
		if (theSquare == null && rng >= 0) 
		{
			theSquare = GetEmptyMiddleCircleCorners();
		}

		rng = Random.value;
		if (theSquare == null && rng >= 0) 
		{
			theSquare = GetEmptyCorner();
		}

		rng = Random.value;
		if(theSquare == null && rng >= 0) 
		{
			theSquare = GetEmptySideMiddle();	
		}

		rng = Random.value;
		if(theSquare == null && rng >= 0) 
		{
			theSquare = GetEmptySideSides();	
		}

		if (theSquare == null) 
		{
			theSquare = GetRandomEmptySquare();
		}

		PlacePiece (OPiece, theSquare);
	}

	Square5x5 CreateOrPreventTrap()
	{
		int p1ICorners = 0;
		int p2ICorners = 0;

		int p1OCorners = 0;
		int p2OCorners = 0;

		Square5x5[] OutsideCorners = new Square5x5[]{
			aGrid[0, 0], aGrid[0, 4],
			aGrid[4, 0], aGrid[4, 4]};
		Square5x5[] InsideCorners = new Square5x5[] {
			aGrid [1, 1], aGrid [1, 3], 
			aGrid [3, 1], aGrid [3, 3]};

		foreach (Square5x5 O in OutsideCorners) 
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

		foreach (Square5x5 I in InsideCorners) 
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
			Debug.Log ("Firing up GetEmptySideMiddle");
			return GetEmptySideMiddle();
		}
		if (p2OCorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyCorner");
			return GetEmptyCorner();
		}

		if (p1ICorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyMiddleCircleSides");
			return GetEmptyMiddleCircleSides();
		}
		if (p2ICorners >= 2) 
		{
			Debug.Log ("Firing up GetEmptyMiddleCircleCorners");
			return GetEmptyMiddleCircleCorners();
		}

		return null;
	}

	Square5x5 WinOrBlock() //12 instanci slanja - realizirat ka i provjeru za pobjedu, pomocu for petlja
	{
		aWinOportunities = new List<Square5x5>();
		aBlockOportunities = new List<Square5x5>();
		float index = 0;
		Square5x5 theSquare = null;

		for (index = 0; index < 5; index++) // for all rows 
		{
			CheckForTwo(new Vector2[]{new Vector2(index, 0), new Vector2(index, 1), new Vector2(index, 2), new Vector2(index, 3), new Vector2(index, 4)});
		}

		for (index = 0; index < 5; index++) //for all columns
		{
			CheckForTwo(new Vector2[]{new Vector2(0, index), new Vector2(1, index), new Vector2(2, index), new Vector2(3, index), new Vector2(4, index)});
		}

		CheckForTwo(new Vector2[]{new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 3), new Vector2(4, 4)}); //for the main diagonal
		CheckForTwo(new Vector2[]{new Vector2(0, 4), new Vector2(1, 3), new Vector2(2, 2), new Vector2(3, 1), new Vector2(4, 0)}); //for the main diagonal but inverted

		CheckForTwo(new Vector2[]{new Vector2(2, 0), new Vector2(3, 1), new Vector2(4, 2)}); //for the 20 31 42 diagonal
		CheckForTwo(new Vector2[]{new Vector2(0, 2), new Vector2(1, 3), new Vector2(2, 4)}); //for the 02 13 24 diagonal

		CheckForTwo(new Vector2[]{new Vector2(0, 2), new Vector2(1, 1), new Vector2(2, 0)}); //for the 02 11 20 diagonal
		CheckForTwo(new Vector2[]{new Vector2(2, 4), new Vector2(3, 3), new Vector2(4, 2)}); //for the 24 33 42 diagonal

		CheckForTwo(new Vector2[]{new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3), new Vector2(3, 4)}); // for the 01 12 23 34 diagonal
		CheckForTwo(new Vector2[]{new Vector2(1, 0), new Vector2(2, 1), new Vector2(3, 2), new Vector2(4, 3)}); // for the 10 21 32 43 diagonal

		CheckForTwo(new Vector2[]{new Vector2(0, 3), new Vector2(1, 2), new Vector2(2, 1), new Vector2(3, 0)}); // for the 03 12 21 30 diagonal
		CheckForTwo(new Vector2[]{new Vector2(1, 4), new Vector2(2, 3), new Vector2(3, 2), new Vector2(4, 1)}); // for the 14 23 32 41 diagonal

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

	Square5x5 CheckForTwo(Vector2[] coords)
	{
		Square5x5 theSquare = null;
		Vector2 coord1 = new Vector2();
		Vector2 coord2 = new Vector2();
		Vector2 coord3 = new Vector2();

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
	Square5x5 GetEmptyMiddleCircleCorners()
	{
		List<Square5x5> aEmptyMiddleCircleCorners = new List<Square5x5>();

		if (GetPlayer (1, 1) == 0) 
		{
			aEmptyMiddleCircleCorners.Add(aGrid[1, 1]);
		}

		if (GetPlayer (1, 3) == 0) 
		{
			aEmptyMiddleCircleCorners.Add(aGrid[1, 3]);
		}

		if (GetPlayer (3, 1) == 0) 
		{
			aEmptyMiddleCircleCorners.Add(aGrid[3, 1]);
		}

		if (GetPlayer (3, 3) == 0) 
		{
			aEmptyMiddleCircleCorners.Add(aGrid[3, 3]);
		}

		if (aEmptyMiddleCircleCorners.Count > 0)
		{
			Debug.Log ("Returning an empty square from the inner circle");
			return aEmptyMiddleCircleCorners[Random.Range(0, aEmptyMiddleCircleCorners.Count)];
		}

		return null;
	}

	Square5x5 GetEmptyMiddleCircleSides()
	{
		List<Square5x5> aEmptyMiddleCircleSides = new List<Square5x5>();

		if (GetPlayer (1, 2) == 0) 
		{
			aEmptyMiddleCircleSides.Add(aGrid[1, 2]);
		}

		if (GetPlayer (2, 1) == 0) 
		{
			aEmptyMiddleCircleSides.Add(aGrid[2, 1]);
		}

		if (GetPlayer (2, 3) == 0) 
		{
			aEmptyMiddleCircleSides.Add(aGrid[2, 3]);
		}

		if (GetPlayer (3, 2) == 0) 
		{
			aEmptyMiddleCircleSides.Add(aGrid[3, 2]);
		}

		if (aEmptyMiddleCircleSides.Count > 0)
		{
			Debug.Log ("Returning an empty square from the inner circle");
			return aEmptyMiddleCircleSides[Random.Range(0, aEmptyMiddleCircleSides.Count)];
		}

		return null;
	}

	Square5x5 GetEmptySideSides()
	{
		List<Square5x5> aEmptySideSides = new List<Square5x5>();

		if (GetPlayer (0, 1) == 0) 
		{
			aEmptySideSides.Add(aGrid[0, 1]);
		}

		if (GetPlayer (0, 3) == 0) 
		{
			aEmptySideSides.Add(aGrid[0, 3]);
		}

		if (GetPlayer (1, 0) == 0) 
		{
			aEmptySideSides.Add(aGrid[1, 0]);
		}

		if (GetPlayer (3, 0) == 0) 
		{
			aEmptySideSides.Add(aGrid[3, 0]);
		}

		if (GetPlayer (1, 4) == 0) 
		{
			aEmptySideSides.Add(aGrid[1, 4]);
		}

		if (GetPlayer (3, 4) == 0) 
		{
			aEmptySideSides.Add(aGrid[3, 4]);
		}

		if (GetPlayer (4, 1) == 0) 
		{
			aEmptySideSides.Add(aGrid[4, 1]);
		}

		if (GetPlayer (4, 3) == 0) 
		{
			aEmptySideSides.Add(aGrid[4, 3]);
		}

		if (aEmptySideSides.Count > 0)
		{
			Debug.Log ("Returning an empty side sides");
			return aEmptySideSides[Random.Range(0, aEmptySideSides.Count)];
		}

		return null;
	}

	Square5x5 GetEmptySideMiddle()
	{
		List<Square5x5> aEmptySideMiddle = new List<Square5x5>();

		if (GetPlayer (0, 2) == 0) 
		{
			aEmptySideMiddle.Add(aGrid[0, 2]);
		}

		if (GetPlayer (2, 4) == 0) 
		{
			aEmptySideMiddle.Add(aGrid[2, 4]);
		}

		if (GetPlayer (2, 0) == 0) 
		{
			aEmptySideMiddle.Add(aGrid[2, 0]);
		}

		if (GetPlayer (4, 2) == 0) 
		{
			aEmptySideMiddle.Add(aGrid[4, 2]);
		}

		if (aEmptySideMiddle.Count > 0)
		{
			Debug.Log ("Returning an empty side middle");
			return aEmptySideMiddle[Random.Range(0, aEmptySideMiddle.Count)];
		}

		return null;
	}

	Square5x5 GetEmptyCorner()
	{
		List<Square5x5> aEmptyCorners = new List<Square5x5>();

		if (GetPlayer (0, 0) == 0) 
		{
			aEmptyCorners.Add(aGrid[0, 0]);
		}

		if (GetPlayer (0, 4) == 0) 
		{
			aEmptyCorners.Add(aGrid[0, 4]);
		}

		if (GetPlayer (4, 0) == 0) 
		{
			aEmptyCorners.Add(aGrid[4, 0]);
		}

		if (GetPlayer (4, 4) == 0) 
		{
			aEmptyCorners.Add(aGrid[4, 4]);
		}

		if (aEmptyCorners.Count > 0)
		{
			Debug.Log ("Returning an empty corner");
			return aEmptyCorners[Random.Range(0, aEmptyCorners.Count)];
		}

		return null;
	}

	Square5x5 GetCentre()
	{
		if (GetPlayer(2, 2) == 0)
		{
			Debug.Log("Vracan centar");
			return aGrid[2, 2];
		}

		return null; 
	}

	Square5x5 GetRandomEmptySquare()
	{
		Square5x5 theSquare;
		List<Square5x5> aEmptySquare = new List<Square5x5>();

		for (int i = 0; i < aSquares.Length; i++) 
		{
			theSquare = aSquares[i];
			if (theSquare.player == 0) 
			{
				aEmptySquare.Add(theSquare);
			}
		}
		theSquare = aEmptySquare[Random.Range (0, aEmptySquare.Count)];
		return theSquare;
	}

	Square5x5 NullifyTheSquare()
	{
		return null;
	}

    void PlacePiece(GameObject piece, Square5x5 other)
    {
		moves++;

        Instantiate(piece, other.gameObject.transform.position, Quaternion.identity);

		other.player = currPlayer;

		if (CheckForWin(other))
		{
			gameOver = true;
			StartCoroutine(ShowWinnerPrompt());
			return;
		}
		else if (moves >= 25) 
		{
			gameOver = true;
			StartCoroutine(ShowTiePrompt());
			return;
		}
        currPlayer++;
        if (currPlayer > 2)
        {
            currPlayer = 1;
        }
        ShowPlayerPrompt();
    }

	int GetPlayer(int r, int c)
	{
		return aGrid[r, c].player;
	}

	bool CheckForWin(Square5x5 other)
	{
		int index = 0;
		int index2 = 0;

		for (index = 2; index < 5; index++)	//checked OK -> checking rows
		{
			if ((GetPlayer (other.r, index - 2) == currPlayer) && (GetPlayer (other.r, index - 1) == currPlayer) && (GetPlayer (other.r, index) == currPlayer)) 
			{
				return true;
			}
		}
		for (index = 2; index < 5; index++) //checked OK -> checking columns
		{
			if ((GetPlayer (index - 2, other.c) == currPlayer) && (GetPlayer (index - 1, other.c) == currPlayer) && (GetPlayer (index, other.c) == currPlayer)) 
			{
				return true;
			}
		}
		//checked OK -> checking 02 - 24 diagonal
		if ((GetPlayer(0, 2) == currPlayer) && (GetPlayer(1, 3) == currPlayer) && (GetPlayer(2, 4) == currPlayer))
		{
			return true;
		}
		//checked OK -> checking 20 - 42 diagonal
		if ((GetPlayer(2, 0) == currPlayer) && (GetPlayer(3, 1) == currPlayer) && (GetPlayer(4, 2) == currPlayer))
		{
			return true;
		}
		//checked OK -> checking 02 - 20 diagonal
		if ((GetPlayer(0, 2) == currPlayer) && (GetPlayer(1, 1) == currPlayer) && (GetPlayer(2, 0) == currPlayer))
		{
			return true;
		}
		//checked OK -> checking 24 - 42 diagonal
		if ((GetPlayer(2, 4) == currPlayer) && (GetPlayer(3, 3) == currPlayer) && (GetPlayer(4, 2) == currPlayer))
		{
			return true;
		}
		//checked OK -> checking 01 - 34 diagonals
		for (index = 2; index < 4; index++) 
		{
			if ((GetPlayer (index - 2, index - 1) == currPlayer) && (GetPlayer (index - 1, index) == currPlayer) && (GetPlayer (index, index + 1) == currPlayer)) 
			{
				return true;
			}
		}
		//checked OK -> checking 00 - 44 diagonals
		for (index = 2; index < 5; index++) 
		{
			if ((GetPlayer (index - 2, index - 2) == currPlayer) && (GetPlayer (index - 1, index - 1) == currPlayer) && (GetPlayer (index, index) == currPlayer)) 
			{
				return true;
			}
		}
		//checked OK - checking 10 - 43 diagonals
		for (index = 2; index < 4; index++) 
		{
			if ((GetPlayer (index - 1, index - 2) == currPlayer) && (GetPlayer (index, index - 1) == currPlayer) && (GetPlayer (index + 1, index) == currPlayer)) 
			{
				return true;
			}
		}
		//checked OK -> checking 03 - 30 diagonals
		index2 = 4;
		for (index = 2; index < 4; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}
		//checked OK -> checking 04 - 40 diagoanls
		index2 = 5;
		for (index = 2; index < 5; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}
		//checked OK checking 41 - 14 diagonals
		index2 = 5;
		for (index = 3; index < 5; index++) 
		{
			index2--;
			if ((GetPlayer (index - 2, index2) == currPlayer) && (GetPlayer (index - 1, index2 - 1) == currPlayer) && (GetPlayer (index, index2 - 2) == currPlayer)) 
			{
				return true;
			}
		}
		return false;
	}

	void ShowScore()
	{
		scoreBoard.text = "Current Score is: " + P1Score + " : " + P2Score;
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

		if (P1Score == 3) {
			yield return new WaitForSeconds (1);
			SceneManager.LoadScene (2);
		} 
		else 
		{
			yield return new WaitForSeconds(3);
			SceneManager.LoadScene(1);
		}
	}

	IEnumerator ShowTiePrompt()
	{
		prompt.text = "Tie! Neither player wins.";

		yield return new WaitForSeconds(3);
		SceneManager.LoadScene(2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
