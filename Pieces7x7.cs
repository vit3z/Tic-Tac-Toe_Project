using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces7x7 : MonoBehaviour {

	public static Pieces7x7 pieces;
	public int r;
	public int c;
	public int index;

	void OnMouseDown()
	{
		if (GameManager7x7.gm.canDestroy == true) 
		{
			Debug.Log ("Gonna destroy this");
			//GameManager.gm.ResetOwnage (GameManager.gm.aGrid [grid[r, c]]);
			GameManager7x7.gm.ResetOwnageAndDialBack(this.index);
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
