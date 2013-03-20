using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TitlePage : FContainer
{
	// private FSprite _man;
	private FButton _start;
	private FLabel _title;
	
	public TitlePage ()
	{
		_title = new FLabel("gamefont", "Xelda Prototype");
		_title.y = 200;
		_title.scale = 2f;
		AddChild(_title);
		
		_start = new FButton("start_up.png","start_down.png");
		_start.AddLabel("gamefont","Start", Color.white);
		AddChild(_start);
		
		_start.SignalRelease += HandleManButtonPressed;
	}

	private void HandleManButtonPressed (FButton button)
	{
		XeldaGame.instance.GoToGamePage();
	}
}