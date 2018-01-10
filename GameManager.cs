using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{

	public static GameManager gm;                                   //to add the GameManager game object
    public GameObject XPiece;                                       //to link the X piece
    public GameObject OPiece;                                       //to link the O piece
    private int currPlayer;                                         //determins the current player
    public GUIText prompt;                                          //to link the top-left prompt, which tells which player is to take a turn
    public Square [] aSquares;                                      //a grid element
    public Square [,] aGrid;                                        //contains the grid elements
    //public Square [] Corners;
    private List<Square> aWinOportunities;                          //list where the AI will list oportunities to win the match
    private List<Square> aBlockOportunities;                        //list where the AI will list oportunities to block the player
    private bool gameOver = false;                                  //on true indicates the current match being over
    private int moves = 0;                                          //counts how many turns have been taken
    private IEnumerator coroutine;
    private float rng = 0;

	// Use this for initialization
	void Start () 
	{
		if (gm == null) 
		{
			gm = gameObject.GetComponent<GameManager>();            //gives "gm" the GameManager component
		}
        currPlayer = 1;                                             //makes the human player go first
        ShowPlayerPrompt();

        aSquares = FindObjectsOfType(typeof(Square)) as Square[];   //searches for all the "Square" objects
           
        aGrid = new Square[3, 3];                                   //makes the "aGrid" variable a 2-dimensional array composed of "Square" objects

        Square theSquare;

        for (int i = 0; i < aSquares.Length; i++)
        {
            theSquare = aSquares[i];
            aGrid[theSquare.r, theSquare.c] = theSquare;
        }
	}

    /*public void ClickSquareTwoPlayerMode(Square other) // this is used if the game is for 2 players
	{
        if (gameOver)
        {  
            return;
        }
        if (currPlayer == 1)
        {
            PlacePiece(XPiece, other);
        }
        else
        {
            PlacePiece(OPiece, other);
        }
	}*/

    public void ClickSquare(Square other)
    {
        if (gameOver)
        {
            return;
        }

        if (currPlayer == 1)
        {
            PlacePiece(XPiece, other);

            if (moves <= 8 && currPlayer != 1 && gameOver == false)
            {
                StartCoroutine(ComputerWaitUntilTakingTurn());
            }
        }
    }

    IEnumerator ComputerWaitUntilTakingTurn()
    {
        yield return new WaitForSeconds(3);
        ComputerTakeATurn();
    }
	
    void PlacePiece(GameObject piece, Square other)
    {
        moves++;                                //increments the moves that were made

        Instantiate(piece, other.gameObject.transform.position, Quaternion.identity);           //places the piece on the grid
        other.player = currPlayer;

        if (CheckForWin(other))
        {
            gameOver = true;
            StartCoroutine(ShowWinnerPrompt());
            return;
        }
        else if (moves >= 9)
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

    void ComputerTakeATurn()        //method by which the AI picks it's "strategy"
    {
        Square theSquare;
        theSquare = NullifyTheSquare();

        rng = Random.value;
        if (rng > 0.5f)
        {
            theSquare = WinOrBlock();
        }

        /*if (theSquare == null)
        {
            theSquare = Block();
        }
        if (theSquare == null)
        {
            theSquare = CreateTrap();
        }
        if (theSquare == null)
        {
            theSquare = PreventTrap();
        }*/
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
            theSquare = GetEmptyCorner();
        }

        rng = Random.value;
        if (theSquare == null && rng > 0.5f)
        {
            theSquare = GetEmptySide();
        }

        if (theSquare == null)
        {
            theSquare = GetRandomEmptySquare();
        }

        PlacePiece(OPiece, theSquare);
    }

    Square NullifyTheSquare()
    {
        return null;
    }

    Square GetRandomEmptySquare()       //method for the AI to choose any random space in the grid
    {
        Square theSquare;
        List<Square> aEmptySquares = new List<Square>();
        //aEmptySquares.Add(aSquares[0]);

        for (int i = 0; i < aSquares.Length; i++)
        {
            theSquare = aSquares[i];
            if (theSquare.player == 0)
            {
                aEmptySquares.Add(theSquare);
            }
        }
        theSquare = aEmptySquares[Random.Range(0, aEmptySquares.Count)];
        return theSquare;
    }

    Square GetCentre()                //this method returns the position of the middle space if it's not occupied so that the AI can place its piece there
    {
        if (GetPlayer(1, 1) == 0)
        {
            return aGrid[1, 1];
        }

        return null;
    }

    Square GetEmptyCorner()            //method for the AI to list all the unocupied corners of the grid, and choosing a random one
    {
        List<Square> aEmptyCorners = new List<Square>();

        if (GetPlayer(0, 0) == 0)
        {
            aEmptyCorners.Add(aGrid[0, 0]);
        }
        if (GetPlayer(0, 2) == 0)
        {
            aEmptyCorners.Add(aGrid[0, 2]);
        }
        if (GetPlayer(2, 0) == 0)
        {
            aEmptyCorners.Add(aGrid[2, 0]);
        }
        if (GetPlayer(2, 2) == 0)
        {
            aEmptyCorners.Add(aGrid[2, 2]);
        }

        if (aEmptyCorners.Count > 0)
        {
            return aEmptyCorners[Random.Range(0, aEmptyCorners.Count)];
        }

        return null;
    }

    Square GetEmptySide()               //method for the AI to list out all the empty side spaces (in the middles, but on the sides) and choose a random one
    {
        List<Square> aEmptySides = new List<Square>();

        if (GetPlayer(0, 1) == 0)
        {
            aEmptySides.Add(aGrid[0, 1]);
        }
        if (GetPlayer(1, 0) == 0)
        {
            aEmptySides.Add(aGrid[1, 0]);
        }
        if (GetPlayer(1, 2) == 0)
        {
            aEmptySides.Add(aGrid[1, 2]);
        }
        if (GetPlayer(2, 1) == 0)
        {
            aEmptySides.Add(aGrid[2, 1]);
        }

        if (aEmptySides.Count > 0)
        {
            return aEmptySides[Random.Range(0, aEmptySides.Count)];
        }

        return null;
    }

    Square WinOrBlock()                         //method to either make the AI block the win of the player, or secure the victory for itself
    {
        aWinOportunities = new List<Square>();
        aBlockOportunities = new List<Square>();

        CheckForTwo(new Vector2 []{new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)});
        CheckForTwo(new Vector2 []{new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2)});
        CheckForTwo(new Vector2 []{new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2)});

        CheckForTwo(new Vector2 []{new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)});
        CheckForTwo(new Vector2 []{new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1)});
        CheckForTwo(new Vector2 []{new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2)});

        CheckForTwo(new Vector2 []{new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2)});
        CheckForTwo(new Vector2 []{new Vector2(0, 2), new Vector2(1, 1), new Vector2(2, 0)});

        if (aWinOportunities.Count > 0)
        {
            return aWinOportunities[Random.Range(0, aWinOportunities.Count)];
        }

        if (aBlockOportunities.Count > 0)
        {
            return aBlockOportunities[Random.Range(0, aBlockOportunities.Count)];
        }

        return null;
    }

    void CheckForTwo(Vector2[] coords)              //method ot make the AI check if there are 2 peices of the same player in a "row"
    {
        int p1Count = 0;
        int p2Count = 0;
        Square theSquare = null;
        Vector2 coord;

        for (int i = 0; i < coords.Length; i++)
        {
            coord = coords[i];

            if (GetPlayer((int)coord.x, (int)coord.y) == 1)
            {
                p1Count++;
            }
            else if (GetPlayer((int)coord.x, (int)coord.y) == 2)
            {
                p2Count++;
            }
            else
            {
                theSquare = aGrid[(int)coord.x, (int)coord.y];
            }
        }

        if (theSquare != null)
        {
            if (p1Count == 2)
            {
                aBlockOportunities.Add(theSquare);
            }
            if (p2Count == 2)
            {
                aWinOportunities.Add(theSquare);
            }
        }
    }

    Square CreateOrPreventTrap()                    //method to make the AI either prevent a trap or make one for the player
    {
        int p1Corners = 0;
        int p2Corners = 0;
        Square[] Corners = new Square[] {aGrid[0, 0], aGrid[0, 2], aGrid[2, 0], aGrid[2, 2]};

        foreach (Square S in Corners)
        {
            if (S.player == 1)
            {
                p1Corners++;
            }
            if (S.player == 2)
            {
                p2Corners++;
            }
        }

        if (p1Corners == 2)
        {
            return GetEmptySide();
        }
        if (p2Corners == 2)
        {
            return GetEmptySide();
        }
        return null;
    }

    bool CheckForWin(Square other)
    {
        if ((GetPlayer(other.r, 0) == currPlayer) && (GetPlayer(other.r, 1) == currPlayer) && (GetPlayer(other.r, 2) == currPlayer))
        {
            return true;
        }
        if ((GetPlayer(0, other.c) == currPlayer) && (GetPlayer(1, other.c) == currPlayer) && (GetPlayer(2, other.c) == currPlayer))
        {
            return true;
        }
        if ((GetPlayer(0, 0) == currPlayer) && (GetPlayer(1, 1) == currPlayer) && (GetPlayer(2, 2) == currPlayer))
        {
            return true;
        }
        if ((GetPlayer(0, 2) == currPlayer) && (GetPlayer(1, 1) == currPlayer) && (GetPlayer(2, 0) == currPlayer))
        {
            return true;
        }

        return false;
    }

    void ShowPlayerPrompt()
    {
        if (currPlayer == 1)
        {  
            prompt.text = "Player 1, place an X";
        }
        else
        {
            prompt.text = "Player 2 is thinking";
        }
    }

    IEnumerator ShowWinnerPrompt()
    {
        if (currPlayer == 1)
        {
            prompt.text = "X gets 3 in a row. Player 1 wins!";
        }
        else
        {
            prompt.text = "O gets 3 in a row. Player 2 wins!";
        }

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    IEnumerator ShowTiePrompt()
    {
        prompt.text = "Tie! Neither player wins.";

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

	// Update is called once per frame
	void Update () 
	{
		
	}
}
