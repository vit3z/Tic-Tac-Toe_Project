using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square7x7 : MonoBehaviour 
{
	public int r;
	public int c;
	public int player;

	void OnMouseDown()
	{
		if (player == 0) 
		{
			GameManager7x7.gm.ClickSquare(this);	
		}

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
