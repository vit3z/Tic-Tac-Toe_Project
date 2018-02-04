using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPowerUp7x7 : MonoBehaviour {
	public Renderer rend;

	void OnMouseDown()
	{
		Debug.Log ("numberOfTurnsBeforePowerUp: " + GameManager7x7.gm.numberOfTurnsBeforePowerUp);
		if (GameManager7x7.gm.numberOfTurnsBeforePowerUp == 0 && PauseMenu.GameIsPaused == false) 
		{
			GameManager7x7.gm.EnablePowerUp ();	
		}
	}
	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			if(GameManager7x7.gm.canDestroy)
			{
				rend.material.color = Color.red;
			}
			else
			{
				rend.material.color = Color.white;
			}
		}
		
	}
}
