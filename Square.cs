using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour 
{
	public int r;
	public int c;
    public int player;

	void OnMouseDown()
	{
        if (player == 0)
        {
            GameManager.gm.ClickSquare(this);
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
