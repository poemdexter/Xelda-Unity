using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TitlePage : FContainer
{
	// private FSprite _man;
	private FButton _man;
	
	public TitlePage ()
	{
		//_man = new FSprite("man.png");
		//AddChild(_man);
		
		_man = new FButton("man.png");
		AddChild(_man);
		
		_man.SignalRelease += HandleManButtonPressed;
	}

	private void HandleManButtonPressed (FButton button)
	{
		XeldaGame.instance.GoToGamePage();
	}
}