using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces5x5 : MonoBehaviour {

	public static Pieces5x5 pieces;
	public int r;
	public int c;
	public int index;


	void OnMouseDown()
	{
		if (GameManager5x5.gm.canDestroy == true) 
		{
			Debug.Log ("Gonna destroy this");
			//GameManager.gm.ResetOwnage (GameManager.gm.aGrid [grid[r, c]]);
			GameManager5x5.gm.ResetOwnageAndDialBack(this.index);
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
