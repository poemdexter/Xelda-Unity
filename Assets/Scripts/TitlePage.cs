using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TitlePage : FContainer
{
	// private FSprite _man;
	private FButton _start;
	
	public TitlePage ()
	{
		//_man = new FSprite("man.png");
		//AddChild(_man);
		
		_start = new FButton("start_up.png","start_down.png");
		_start.x = 300;
		_start.y = 250;
		AddChild(_start);
		
		_start.SignalRelease += HandleManButtonPressed;
	}

	private void HandleManButtonPressed (FButton button)
	{
		XeldaGame.instance.GoToGamePage();
	}
}