using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPowerUp5x5 : MonoBehaviour 
{
	public Renderer rend;
	void OnMouseDown()
	{
		Debug.Log ("numberOfTurnsBeforePowerUp: " + GameManager5x5.gm.numberOfTurnsBeforePowerUp);
		if (GameManager5x5.gm.numberOfTurnsBeforePowerUp == 0 && PauseMenu.GameIsPaused == false) 
		{
			GameManager5x5.gm.EnablePowerUp ();	
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
			if(GameManager5x5.gm.canDestroy)
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
