using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{

	public static GameManager gm;
    public GameObject XPiece;
    public GameObject OPiece;
    private int currPlayer;
    public GUIText prompt;
    public Square [] aSquares;
    public Square[,] aGrid;
    private bool gameOver = false;
    private int moves = 0;

	// Use this for initialization
	void Start () 
	{
		if (gm == null) 
		{
			gm = gameObject.GetComponent<GameManager>();
		}
        currPlayer = 1;
        ShowPlayerPrompt();

        aSquares = FindObjectsOfType(typeof(Square)) as Square[];

        aGrid = new Square[3, 3];

        Square theSquare;

        for (int i = 0; i < aSquares.Length; i++)
        {
            theSquare = aSquares[i];
            aGrid[theSquare.r, theSquare.c] = theSquare;
        }
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

    /*public void ClickSquareTwoPlayerMode(Square other)
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
           // yield break;
            return;
        }

        PlacePiece(XPiece, other);

        if (!gameOver)
        {
           // StartCoroutine(ComputerWaitUntilTakingTurn());
           // yield return new WaitForSeconds(2);
            Invoke("ComputerTakeATurn", 3);
        }
    }

    /*IEnumerator ComputerWaitUntilTakingTurn()
    {
        yield return new WaitForSeconds(3);
    }*/
	
    void PlacePiece(GameObject piece, Square other)
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

    void ComputerTakeATurn()
    {
        Square theSquare;
        List<Square> aEmptySquares = new List<Square>();

        for (int i = 0; i < aSquares.Length; i++)
        {
            theSquare = aSquares[i];
            if (theSquare.player == 0)
            {
                aEmptySquares.Add(theSquare);
            }
        }
        theSquare = aEmptySquares[Random.Range(0, aEmptySquares.Count)];
        PlacePiece(OPiece, theSquare);
    }

	// Update is called once per frame
	void Update () 
	{
		
	}
}
