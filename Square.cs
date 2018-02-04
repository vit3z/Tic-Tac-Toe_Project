using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour 
{
	public int r;                   //row component of the Square object
    public int c;                   //column component of the Square object
    public int player;              //determin which player placed the piece on the tile
	public static Square sq;
	public int index;

	void OnMouseDown()
	{
		if (player == 0 && GameManager.gm.canDestroy == false && PauseMenu.GameIsPaused == false)
        {
            GameManager.gm.ClickSquare(this);       //if the human player has clicked the tile, his piece is placed
        }
	}


	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
