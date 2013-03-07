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
	private bool _keyUp = false;
	private bool _keyDown = false;
	private bool _keyLeft = false;
	private bool _keyRight = false;
	
	private bool _collideUp = false;
	private bool _collideDown = false;
	private bool _collideLeft = false;
	private bool _collideRight = false;
	
	private Direction _roomTransitionDirection = Direction.None;
	
	private Mob _player;
	
	private float _moveSpeed = 2f;
	
	//private FNode _cameraTarget = new FNode();
	
	private Dungeon _dungeon;
	
	public GamePage ()
	{
		// create dungeon
		_dungeon = new Dungeon(2);
		
		// set current room to draw
		AddChild(_dungeon.CurrentRoom);
		
		// create player
		_player = new Mob("man", -10, 50);
		AddChild(_player);
		
		// *** debug to find collision boxes
		//showCollisionsWithMen();
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
		if (_roomTransitionDirection != Direction.None) DoRoomTransition(_roomTransitionDirection);
		else 
		{
			HandleInputs();
			TestForCollisions();
			HandleMovement();
			FSM_Manager.HandleMobAI(_player, _dungeon.CurrentRoom);
		}
	}
	
	void HandleInputs()
	{
		// pushed key
		if (Input.GetKeyDown(KeyCode.W)) _keyUp = true;
		if (Input.GetKeyDown(KeyCode.S)) _keyDown = true;
		if (Input.GetKeyDown(KeyCode.A)) _keyLeft = true;
		if (Input.GetKeyDown(KeyCode.D)) _keyRight = true;
		
		// let go of key
		if (Input.GetKeyUp(KeyCode.W)) _keyUp = false;
		if (Input.GetKeyUp(KeyCode.S)) _keyDown = false;
		if (Input.GetKeyUp(KeyCode.A)) _keyLeft = false;
		if (Input.GetKeyUp(KeyCode.D)) _keyRight = false;
	}
	
	void HandleMovement()
	{
		if (_keyUp && !_collideUp)       _player.Move(0, _moveSpeed);
		if (_keyDown && !_collideDown)   _player.Move(0, -_moveSpeed);
		if (_keyLeft && !_collideLeft)   _player.Move(-_moveSpeed, 0);
		if (_keyRight && !_collideRight) _player.Move(_moveSpeed, 0);
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
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					break;
				}
			}
			
			// hit wall segment blocking passageway
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
				{
					_collideUp = true;
					break;
				}
			}
			
			// hit passageway
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideUp = true;
					_dungeon.PassageToConnectedRoom(cbox.name);
					SwitchRoom(_dungeon.TransitionRoom, Direction.N);
					break;
				}
			}
		}
		if (_keyDown)
		{
			Rect collisionRect = _player.box;
			collisionRect.y = collisionRect.y - _moveSpeed;
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideDown = true;
					break;
				}
			}
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
				{
					_collideDown = true;
					break;
				}
			}
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideDown = true;
					_dungeon.PassageToConnectedRoom(cbox.name);
					SwitchRoom(_dungeon.TransitionRoom, Direction.S);
					break;
				}
			}
		}
		if (_keyLeft)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x - _moveSpeed;
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideLeft = true;
					break;
				}
			}
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
				{
					_collideLeft = true;
					break;
				}
			}
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideLeft = true;
					_dungeon.PassageToConnectedRoom(cbox.name);
					SwitchRoom(_dungeon.TransitionRoom, Direction.W);
					break;
				}
			}
		}
		if (_keyRight)
		{
			Rect collisionRect = _player.box;
			collisionRect.x = collisionRect.x + _moveSpeed;
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.collisionBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideRight = true;
					break;
				}
			}
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageObjectBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box) && cbox.active)
				{
					_collideRight = true;
					break;
				}
			}
			
			foreach(CollisionBox cbox in _dungeon.CurrentRoom.passageBoxList)
			{
				if (collisionRect.CheckIntersect(cbox.box))
				{
					_collideRight = true;
					_dungeon.PassageToConnectedRoom(cbox.name);
					SwitchRoom(_dungeon.TransitionRoom, Direction.E);
					break;
				}
			}
		}
	}
	
	private void SwitchRoom(Room room, Direction dir)
	{
		// position room offset in direction we need
		switch (dir)
		{
		case Direction.N:
			room.y += _dungeon.RoomHeight;
			break;
		case Direction.S:
			room.y -= _dungeon.RoomHeight;
			break;
		case Direction.W:
			room.x -= _dungeon.RoomWidth;
			break;
		case Direction.E:
			room.x += _dungeon.RoomWidth;
			break;
		}
		
		// put up room we want to transition to
		AddChildAtIndex(room, 0);
		_roomTransitionDirection = dir;
	}
	
	private void DoRoomTransition(Direction dir)
	{
		float transitionSpeed = 4f;
		float playerTransSpeed = .4f;
		
		resetKeys(); // do this since we never catch keyUp after hitting a passage
		
		switch(dir)
		{
		case Direction.N:
			this.y -= transitionSpeed;
			_player.Move(0, playerTransSpeed);
			if (this.y <= -_dungeon.RoomHeight)
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		case Direction.S:
			this.y += transitionSpeed;
			_player.Move(0, -playerTransSpeed);
			if (this.y >= _dungeon.RoomHeight)
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		case Direction.W:
			this.x += transitionSpeed;
			_player.Move(-playerTransSpeed, 0);
			if (this.x >= _dungeon.RoomWidth)
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		case Direction.E:
			this.x -= transitionSpeed;
			_player.Move(playerTransSpeed, 0);
			if (this.x <= -_dungeon.RoomWidth) 
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
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
	
	// removes the old room sprite and the temp new room sprite
	// resets camera back to origin and moves player as well
	private void ResetRoomDrawn()
	{
		// make transition the current and redraw
		this.RemoveChild(_dungeon.CurrentRoom);
		_dungeon.ChangeTransitionToCurrentRoom();
		this.RemoveChild(_dungeon.TransitionRoom);
		this.AddChildAtIndex(_dungeon.CurrentRoom,0);
		
		// readjust player
		int x = 0;
		int y = 0;
		if (this.x != 0) x = (this.x > 0) ? _dungeon.RoomWidth : -_dungeon.RoomWidth;
		if (this.y != 0) y = (this.y > 0) ? _dungeon.RoomHeight : -_dungeon.RoomHeight;
		_player.Move(x,y);
		this.x = 0;
		this.y = 0;
	}
	
	// debug method to overlay man with collision boxes
	private void showCollisionsWithMen()
	{
		foreach(CollisionBox box in _dungeon.CurrentRoom.collisionBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
		}
		
		foreach(CollisionBox box in _dungeon.CurrentRoom.passageBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
		}
		
		foreach(CollisionBox box in _dungeon.CurrentRoom.passageObjectBoxList)
		{
			FSprite cb = new FSprite("man.png");
			cb.x = box.box.x;
			cb.y = box.box.y;
			cb.anchorX = 0;
			cb.anchorY = 0;
			AddChild(cb);
		}
	}
	
}


