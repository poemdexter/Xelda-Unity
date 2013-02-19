using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class XeldaGame : MonoBehaviour {
	
	public static XeldaGame instance;
		
	private FContainer _currentPage;
	
	// Use this for initialization
	void Start () {
		
		// hot hot singleton action
		instance = this;
		
		FutileParams fparams = new FutileParams(true, true, false, false);
		fparams.AddResolutionLevel(600.0f,1f,1f,"");
		
		// *** Uncomment to set origin to bot left
		//fparams.origin = new Vector2(0,0);
		
		Futile.instance.Init (fparams);
		Futile.atlasManager.LoadAtlas("Atlases/Sprites");
		
		GoToTitlePage();
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
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
