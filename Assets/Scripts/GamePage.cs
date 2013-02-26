using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct Player
{
	public Rect box;
	public string name;
}

public class GamePage : FContainer
{
	private FSprite _manSprite;
	private FSprite _floorSprite;
	private FSprite transitionMapSprite;
	
	private bool _keyUp = false;
	private bool _keyDown = false;
	private bool _keyLeft = false;
	private bool _keyRight = false;
	
	private bool _collideUp = false;
	private bool _collideDown = false;
	private bool _collideLeft = false;
	private bool _collideRight = false;
	
	private Direction _mapTransitionDirection = Direction.None;
	
	private Player _player;
	
	private float _moveSpeed = 2f;
	
	//private FNode _cameraTarget = new FNode();
	
	private Dungeon _dungeon;
	
	public GamePage ()
	{
		_manSprite = new FSprite("man.png");
		
		_dungeon = new Dungeon(2);
		
		// remove old one if exists
		if (_floorSprite != null) RemoveChild(_floorSprite);
		
		// set new map to draw
		_floorSprite = new FSprite(_dungeon.CurrentMap.mapName + ".png");
		AddChild(_floorSprite);
		
		if (_manSprite != null) RemoveChild(_manSprite);
		_manSprite.x = 50; //-(_dungeon.CurrentMap.GetMapWidth() / 2);
		_manSprite.y = 50; //-(_dungeon.CurrentMap.GetMapHeight() / 2);
		_manSprite.anchorX = 0;
		_manSprite.anchorY = 0;
		AddChild(_manSprite);
		
		//_cameraTarget.x = -200;
		//_cameraTarget.y = -100;
		//AddChild(_cameraTarget);
		
		// *** Stay focused on map
		//Futile.stage.Follow(_cameraTarget,true,false);
		
		_player = new Player();
		_player.box.x = _manSprite.x + 4;
		_player.box.y = _manSprite.y + 4;
		_player.box.width = _manSprite.width - 8;
		_player.box.height = _manSprite.height - 8;
		
		// *** debug to find collision boxes
		//showCollisionsWithMen();
	}
	
	private void showCollisionsWithMen()
	{
		foreach(collisionBox box in _dungeon.CurrentMap.collisionBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
		}
		
		foreach(collisionBox box in _dungeon.CurrentMap.passageBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
		}
	}
	
	override public void HandleAddedToStage()
	{
		Futile.instance.SignalUpdate += HandleUpdate;
		base.HandleAddedToStage();
	}
	
	override public void HandleRemovedFromStage()
	{
		Futile.instance.SignalUpdate -= HandleUpdate;
		base.HandleRemovedFromStage();
	}
	
	void HandleUpdate()
	{
		if (_mapTransitionDirection != Direction.None) 
		{
			DoMapTransition(_mapTransitionDirection);
		}
		else 
		{
		HandleInputs();
		TestForCollisions();
		HandleMovement();
		}
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
		
		_player.box.x = _manSprite.x + 4;
		_player.box.y = _manSprite.y + 4;
		
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
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					break;
				}
			}
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					Debug.Log("PASSAGED!");
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.CurrentMap.mapName, Direction.N);
					break;
				}
			}
		}
		if (_keyDown)
		{
			Rect collisionRect = _player.box;
			collisionRect.y = collisionRect.y - _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideDown = true;
					break;
				}
			}
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideDown = true;
					Debug.Log("PASSAGED!");
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.CurrentMap.mapName, Direction.S);
					break;
				}
			}
		}
		if (_keyLeft)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x - _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideLeft = true;
					break;
				}
			}
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideLeft = true;
					Debug.Log("PASSAGED!");
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.CurrentMap.mapName, Direction.W);
					break;
				}
			}
		}
		if (_keyRight)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x + _moveSpeed;
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideRight = true;
					break;
				}
			}
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideRight = true;
					Debug.Log("PASSAGED!");
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.CurrentMap.mapName, Direction.E);
					break;
				}
			}
		}
	}
	
	private void SwitchMap(string mapName, Direction dir)
	{
		transitionMapSprite = new FSprite(mapName + ".png");
		
		// position map offset in direction we need
		switch (dir)
		{
		case Direction.N:
			transitionMapSprite.y += _dungeon.MapHeight;
			break;
		case Direction.S:
			transitionMapSprite.y -= _dungeon.MapHeight;
			break;
		case Direction.W:
			transitionMapSprite.x -= _dungeon.MapWidth;
			break;
		case Direction.E:
			transitionMapSprite.x += _dungeon.MapWidth;
			break;
		}
		
		// put up map we want to transition to
		AddChildAtIndex(transitionMapSprite, 0);
		_mapTransitionDirection = dir;
	}
	
	private void DoMapTransition(Direction dir)
	{
		float transitionSpeed = 2f;
		float playerTransSpeed = .4f;
		
		resetKeys(); // do this since we never catch keyUp after hitting a passage
		
		switch(dir)
		{
		case Direction.N:
			this.y -= transitionSpeed;
			_manSprite.y += playerTransSpeed;
			if (this.y <= -_dungeon.MapHeight) _mapTransitionDirection = Direction.None;
			break;
		case Direction.S:
			this.y -= transitionSpeed;
			_manSprite.y += playerTransSpeed;
			if (this.y >= _dungeon.MapHeight) _mapTransitionDirection = Direction.None;
			break;
		case Direction.W:
			this.x += transitionSpeed;
			_manSprite.x -= playerTransSpeed;
			if (this.x >= _dungeon.MapWidth) _mapTransitionDirection = Direction.None;
			break;
		case Direction.E:
			this.x -= transitionSpeed;
			_manSprite.x += playerTransSpeed;
			if (this.x <= -_dungeon.MapWidth) 
			{
				ResetMapDrawn();
				_mapTransitionDirection = Direction.None;
			}
			break;
		}
	}
	
	private void resetKeys()
	{
		_keyUp = false;
		_keyDown = false;
 		_keyLeft = false;
		_keyRight = false;
	}
	
	// removes the old map sprite and the temp new map sprite
	// resets camera back to origin and moves player as well
	private void ResetMapDrawn()
	{
		this.RemoveChild(transitionMapSprite);
		this.RemoveChild(_floorSprite);
			
		_floorSprite = new FSprite(_dungeon.CurrentMap.mapName + ".png");
		AddChildAtIndex(_floorSprite,0);
		if (this.x != 0) _manSprite.x += (this.x > 0) ? _dungeon.MapWidth : -_dungeon.MapWidth;
		if (this.y != 0) _manSprite.y += (this.y > 0) ? _dungeon.MapHeight : -_dungeon.MapHeight;
		_player.box.x = _manSprite.x + 4;
		_player.box.y = _manSprite.y + 4;
		this.x = 0;
	}
}


