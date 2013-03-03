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
		
		// set current map to draw
		AddChild(_dungeon.CurrentMap);
		
		if (_manSprite != null) RemoveChild(_manSprite);
		// put man at origin
		_manSprite.x = 0; //-(_dungeon.CurrentMap.GetMapWidth() / 2);
		_manSprite.y = 50; //-(_dungeon.CurrentMap.GetMapHeight() / 2);
		_manSprite.anchorX = 0;
		_manSprite.anchorY = 0;
		AddChild(_manSprite);
		
		//_cameraTarget.x = -200;
		//_cameraTarget.y = -100;
		//AddChild(_cameraTarget);
		
		// *** Stay focused on map
		//Futile.stage.Follow(_cameraTarget,true,false);
		
		// create player
		_player = new Player();
		_player.box.x = _manSprite.x + 4;
		_player.box.y = _manSprite.y + 4;
		_player.box.width = _manSprite.width - 8;
		_player.box.height = _manSprite.height - 8;
		
		// *** debug to find collision boxes
		//showCollisionsWithMen();
	}
	
	// debug method to overlay man with collision boxes
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
		
		foreach(collisionBox box in _dungeon.CurrentMap.passageObjectBoxList)
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
			
			// hit wall
			foreach(collisionBox cbox in _dungeon.CurrentMap.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					break;
				}
			}
			
			// hit wall segment blocking passageway
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
				{
					_collideUp = true;
					break;
				}
			}
			
			// hit passageway
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.TransitionMap, Direction.N);
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
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
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
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.TransitionMap, Direction.S);
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
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
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
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.TransitionMap, Direction.W);
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
			
			foreach(collisionBox cbox in _dungeon.CurrentMap.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
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
					_dungeon.PassageToConnectedMap(cbox.name);
					SwitchMap(_dungeon.TransitionMap, Direction.E);
					break;
				}
			}
		}
	}
	
	private void SwitchMap(Map map, Direction dir)
	{
		// position map offset in direction we need
		switch (dir)
		{
		case Direction.N:
			map.y += _dungeon.MapHeight - 16;
			break;
		case Direction.S:
			map.y -= _dungeon.MapHeight - 16;
			break;
		case Direction.W:
			map.x -= _dungeon.MapWidth - 16;
			break;
		case Direction.E:
			map.x += _dungeon.MapWidth - 16;
			break;
		}
		
		// put up map we want to transition to
		AddChildAtIndex(map, 0);
		_mapTransitionDirection = dir;
	}
	
	private void DoMapTransition(Direction dir)
	{
		float transitionSpeed = 4f;
		float playerTransSpeed = .4f;
		
		resetKeys(); // do this since we never catch keyUp after hitting a passage
		
		switch(dir)
		{
		case Direction.N:
			this.y -= transitionSpeed;
			_manSprite.y += playerTransSpeed;
			if (this.y <= -_dungeon.MapHeight +16)
			{
				ResetMapDrawn();
				_mapTransitionDirection = Direction.None;
			}
			break;
		case Direction.S:
			this.y += transitionSpeed;
			_manSprite.y -= playerTransSpeed;
			if (this.y >= _dungeon.MapHeight -16)
			{
				ResetMapDrawn();
				_mapTransitionDirection = Direction.None;
			}
			break;
		case Direction.W:
			this.x += transitionSpeed;
			_manSprite.x -= playerTransSpeed;
			if (this.x >= _dungeon.MapWidth -16)
			{
				ResetMapDrawn();
				_mapTransitionDirection = Direction.None;
			}
			break;
		case Direction.E:
			this.x -= transitionSpeed;
			_manSprite.x += playerTransSpeed;
			if (this.x <= -_dungeon.MapWidth +16) 
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
		// make transition the current and redraw
		this.RemoveChild(_dungeon.CurrentMap);
		_dungeon.ChangeTransitionToCurrentMap();
		this.RemoveChild(_dungeon.TransitionMap);
		this.AddChildAtIndex(_dungeon.CurrentMap,0);
		
		// readjust player
		if (this.x != 0) _manSprite.x += (this.x > 0) ? _dungeon.MapWidth -16 : -_dungeon.MapWidth +16;
		if (this.y != 0) _manSprite.y += (this.y > 0) ? _dungeon.MapHeight -16 : -_dungeon.MapHeight +16;
		_player.box.x = _manSprite.x + 4;
		_player.box.y = _manSprite.y + 4;
		this.x = 0;
		this.y = 0;
	}
}


