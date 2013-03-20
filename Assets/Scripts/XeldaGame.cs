using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class XeldaGame : MonoBehaviour {
	
	public static XeldaGame instance;
	private FContainer _currentPage;
	public static System.Random rand;
	
	// Use this for initialization
	void Start () {
		
		// hot hot singleton action
		instance = this;
		
		FutileParams fparams = new FutileParams(true, true, false, false);
		fparams.AddResolutionLevel(800.0f,1f,1f,"");
		
		// *** Uncomment to set origin to bot left
		//fparams.origin = new Vector2(0,0);
		
		Futile.instance.Init (fparams);
		Futile.atlasManager.LoadAtlas("Atlases/Sprites");
		Futile.atlasManager.LoadFont("gamefont", "gamefont.png", "Atlases/gamefont");
		
		rand = new System.Random(System.DateTime.Now.Millisecond);
		
		// *** Goes to initial page ***
		GoToTitlePage();
		//GoToDebugRoomPage();
	}
	
	public void GoToTitlePage()
	{
		if (_currentPage != null) _currentPage.RemoveFromContainer();
		
		_currentPage = new TitlePage();
		Futile.stage.AddChild(_currentPage);
	}
	
	public void GoToGamePage()
	{
		if (_currentPage != null) _currentPage.RemoveFromContainer();
		
		_currentPage = new GamePage();
		Futile.stage.AddChild(_currentPage);
	}
	
	public void GoToDebugRoomPage()
	{
		if (_currentPage != null) _currentPage.RemoveFromContainer();
		
		_currentPage = new DebugRoomPage();
		Futile.stage.AddChild(_currentPage);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
