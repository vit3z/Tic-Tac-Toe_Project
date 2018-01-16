using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square5x5 : MonoBehaviour 
{
    public int r;
    public int c;
    public int player;

    void OnMouseDown()
    {
		GameManager5x5.gm.ClickSquare(this);
    }
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
