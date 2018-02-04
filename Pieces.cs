﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour {
	public static Pieces pieces;
	public int r;
	public int c;
	public int index;


	void OnMouseDown()
	{
		if (GameManager.gm.canDestroy == true) 
		{
			Debug.Log ("Gonna destroy this");
			//GameManager.gm.ResetOwnage (GameManager.gm.aGrid [grid[r, c]]);
			GameManager.gm.ResetOwnageAndDialBack(this.index);
			Destroy (gameObject);
		}
	}

	public void Destroy3DModel()
	{
		Destroy (gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		if (pieces == null) 
		{
			//pieces = gameObject.GetComponent<Pieces>();
			pieces = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
