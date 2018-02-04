using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPowerUp : MonoBehaviour {
	public Renderer rend;

	void OnMouseDown()
	{
		Debug.Log ("numberOfTurnsBeforePowerUp: " + GameManager.gm.numberOfTurnsBeforePowerUp);
		if (GameManager.gm.numberOfTurnsBeforePowerUp == 0 && PauseMenu.GameIsPaused == false) 
		{
			GameManager.gm.EnablePowerUp ();	
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
			if(GameManager.gm.canDestroy)
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
