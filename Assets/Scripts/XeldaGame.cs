using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct collisionBox
{
	public Rect box;
	public string name;
	public string tip;
}

public struct Player
{
	public Rect box;
	public string name;
}

public class XeldaGame : MonoBehaviour {
	
	private FSprite _manSprite;
	private FContainer _holder;
	private FContainer _camera;
	
	private int _tileSize;
	private int _mapWidth;
	private int _mapHeight;
	
	private bool _keyUp = false;
	private bool _keyDown = false;
	private bool _keyLeft = false;
	private bool _keyRight = false;
	
	private bool _collideUp = false;
	private bool _collideDown = false;
	private bool _collideLeft = false;
	private bool _collideRight = false;
	
	private Player _player;
	private collisionBox _cbox;
	private List<collisionBox> collisionBoxList = new List<collisionBox>();
	
	private float _moveSpeed = 2f;
	
	private FNode _cameraTarget = new FNode();
		
	// Use this for initialization
	void Start () {
		FutileParams fparams = new FutileParams(true, true, false, false);
		fparams.AddResolutionLevel(600.0f,1f,1f,"");
		
		// *** Uncomment to set origin to bot left
		//fparams.origin = new Vector2(0,0);
		
		Futile.instance.Init (fparams);
		
		Futile.atlasManager.LoadAtlas("Atlases/Sprites");
		
		Futile.stage.AddChild(_holder = new FContainer());
		
		
		TextAsset dataAsset = (TextAsset) Resources.Load ("Maps/xelda_map", typeof(TextAsset));
		if(!dataAsset) 
			Debug.Log("missing map txt file.");
		
		Dictionary<string,object> hash = dataAsset.text.dictionaryFromJson();
		
		// Map Metadata
		_mapWidth = int.Parse(hash["width"].ToString());
		_mapHeight = int.Parse(hash["height"].ToString());
		_tileSize = int.Parse(hash["tilewidth"].ToString());
		
		//Debug.Log(_mapWidth +"||"+ _mapHeight +"||"+ _tileSize);
		
		List<object> tilesetsList = (List<object>)hash["tilesets"];
		Dictionary<string,object> tileset = (Dictionary<string,object>)tilesetsList[0];
		
		string elementPath = tileset["image"].ToString();
		string [] pathSplit = elementPath.Split(new Char [] {'/'});
		string _tilesetElementName = pathSplit[pathSplit.Length-1];
		
		List<object> layersList = (List<object>)hash["layers"];
		
		for (int i=0; i < layersList.Count; i++)
		{
			Dictionary<string,object> layerHash = (Dictionary<string,object>)layersList[i];
			int layerWidth = int.Parse (layerHash["width"].ToString());
			int layerHeight = int.Parse (layerHash["height"].ToString());
			int xOffset = int.Parse (layerHash["x"].ToString());
			int yOffset = int.Parse (layerHash["y"].ToString());
			
			if (layerHash["name"].ToString().Equals("Objects"))
			{
				// Load object data if it exists...
				List<object> objectList = (List<object>)layerHash["objects"];
				
				for (int j=0; j < objectList.Count; j++)
				{
					Dictionary<string,object> objHash = (Dictionary<string,object>)objectList[j];
					
					if (objHash["type"].ToString().ToUpper().Equals("COLLISION"))
					{
						_cbox = new collisionBox();
						_cbox.name = objHash["name"].ToString();
						_cbox.box.x = int.Parse(objHash["x"].ToString());
						_cbox.box.y = int.Parse(objHash["y"].ToString());
						_cbox.box.width = int.Parse(objHash["width"].ToString());
						_cbox.box.height = int.Parse(objHash["height"].ToString());
						_cbox.box.y = GetMapHeight() - _cbox.box.y - _cbox.box.height;
						//Debug.Log("h: " + _mapHeight);
						Debug.Log("got collision box");
						collisionBoxList.Add(_cbox);
					}
				}
			}
		}
		//Debug.Log(_tilesetElementName);
		
		FSprite floorSprite = new FSprite("xelda_map.png");
		floorSprite.anchorX = 0;
		floorSprite.anchorY = 0;
	
		_holder.AddChild(floorSprite);
		
		_manSprite = new FSprite("man.png");
		_manSprite.anchorX = 0;
		_manSprite.anchorY = 0;
		
		_player = new Player();
		_player.box.x = 0;
		_player.box.y = 0;
		_player.box.width = 24;
		_player.box.height = 24;
		
		_holder.AddChild(_manSprite);
		_holder.AddChild(_cameraTarget);
		// *** Comment out to not follow sprite
		Futile.stage.Follow(_cameraTarget,true,false);
	}
	
	int GetMapHeight()
	{
		return _mapHeight * _tileSize;
	}
	
	// Update is called once per frame
	void Update () {
		HandleInputs();
		TestForCollisions();
		HandleMovement();
		
		//Debug.Log(_player.box.x + " " + _player.box.y + " " + _cbox.box.x + " " + _cbox.box.y);
	}
	
	void HandleInputs()
	{
		
		if (Input.GetKeyUp(KeyCode.W))
		{
			_keyUp = false;
		}
		if (Input.GetKeyUp(KeyCode.S))
		{
			_keyDown = false;
		}
		if (Input.GetKeyUp(KeyCode.A))
		{
			_keyLeft = false;
		}
		if (Input.GetKeyUp(KeyCode.D))
		{
			_keyRight = false;
		}
	
		if (Input.GetKeyDown(KeyCode.W))
		{
			_keyUp = true;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			_keyDown = true;
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			_keyLeft = true;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			_keyRight = true;
		}	
	}
	
	void HandleMovement()
	{
		if (_keyUp && !_collideUp)
		{
			_manSprite.y += _moveSpeed;
		}
		if (_keyDown && !_collideDown)
		{
			_manSprite.y -= _moveSpeed;
		}
		if (_keyLeft && !_collideLeft)
		{
			_manSprite.x -= _moveSpeed;
		}
		if (_keyRight && !_collideRight)
		{
			_manSprite.x += _moveSpeed;
		}
		
		_player.box.x = _manSprite.x;
		_player.box.y = _manSprite.y;
		
		_cameraTarget.x += (_manSprite.x - _cameraTarget.x) / 5.0f;
		_cameraTarget.y += (_manSprite.y - _cameraTarget.y) / 5.0f;
		
		//Debug.Log(_cameraTarget.x + " ct " + _cameraTarget.y);
	}
	
	void TestForCollisions()
	{
		_collideUp = false;
		_collideDown = false;
		_collideLeft = false;
		_collideRight = false;
		
		if (_keyUp)
		{
			Rect collisionRect = _player.box;
			collisionRect.y = collisionRect.y + _moveSpeed;
			
			foreach(collisionBox cbox in collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					break;
				}
			}
		}
		if (_keyDown)
		{
			Rect collisionRect = _player.box;
			collisionRect.y = collisionRect.y - _moveSpeed;
			
			foreach(collisionBox cbox in collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideDown = true;
					break;
				}
			}
		}
		if (_keyLeft)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x - _moveSpeed;
			
			foreach(collisionBox cbox in collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideLeft = true;
					break;
				}
			}
		}
		if (_keyRight)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x + _moveSpeed;
			
			foreach(collisionBox cbox in collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideRight = true;
					break;
				}
			}
		}
	}
}
