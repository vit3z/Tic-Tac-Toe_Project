using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square5x5 : MonoBehaviour 
{
    public int r;
    public int c;
    public int player;
	public static Square5x5 sq;
	public int index;

    void OnMouseDown()
    {
		if (player == 0 && GameManager5x5.gm.canDestroy == false && PauseMenu.GameIsPaused == false)
		{
			GameManager5x5.gm.ClickSquare(this);       //if the human player has clicked the tile, his piece is placed
		}
    }
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
