using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GamePage : FContainer
{
	private bool _keyUp = false;
	private bool _keyDown = false;
	private bool _keyLeft = false;
	private bool _keyRight = false;
	private bool _keySpace = false;
	
	private bool _collideUp = false;
	private bool _collideDown = false;
	private bool _collideLeft = false;
	private bool _collideRight = false;
	
	private Direction _roomTransitionDirection = Direction.None;
	
	private Player player;
	
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
		int px = (int)_dungeon.CurrentRoom.playerSpawnBox.box.x;
		int py = (int)_dungeon.CurrentRoom.playerSpawnBox.box.y;
		player = new Player(px, py);
		AddChild(player);
		
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
			HandlePlayerAttacking();
			TestForPlayerCollisionsWithEnvironment();
			HandlePlayerMovement();
			FSM_Manager.HandleMobAI(player, _dungeon.CurrentRoom);
			Combat_Manager.CheckCombatCollisions(player, _dungeon.CurrentRoom);
			Combat_Manager.CheckForDeadAttackBoxes(_dungeon.CurrentRoom);
			Combat_Manager.CheckForDeadMobs(_dungeon.CurrentRoom);
			
			// ** HANDLE PLAYER DEATH SOMETIME
			if (!player.Alive) Debug.Log("Player Death.");
		}
	}
	
	private void HandlePlayerAttacking()
	{
		if (_keySpace && player.attackDelay <= 0) // player hits attack button
		{
			Combat_Manager.PlayerAttack(player, _dungeon.CurrentRoom);
			player.attackDelay = player.attackDelayTime;
		}
		else player.attackDelay--;
	}
	
	void HandleInputs()
	{
		// pushed key
		if (Input.GetKeyDown(KeyCode.W)) _keyUp = true;				// up
		if (Input.GetKeyDown(KeyCode.S)) _keyDown = true;			// down
		if (Input.GetKeyDown(KeyCode.A)) _keyLeft = true;			// left
		if (Input.GetKeyDown(KeyCode.D)) _keyRight = true;			// right
	
		if (Input.GetKeyDown(KeyCode.Space)) _keySpace = true;		// attack
		
		// let go of key
		if (Input.GetKeyUp(KeyCode.W)) _keyUp = false;
		if (Input.GetKeyUp(KeyCode.S)) _keyDown = false;
		if (Input.GetKeyUp(KeyCode.A)) _keyLeft = false;
		if (Input.GetKeyUp(KeyCode.D)) _keyRight = false;
		
		if (Input.GetKeyUp(KeyCode.Space)) _keySpace = false;
	}
	
	void HandlePlayerMovement()
	{
		if (_keyUp && !_collideUp)       player.Move(0, _moveSpeed);
		if (_keyDown && !_collideDown)   player.Move(0, -_moveSpeed);
		if (_keyLeft && !_collideLeft)   player.Move(-_moveSpeed, 0);
		if (_keyRight && !_collideRight) player.Move(_moveSpeed, 0);
	}
	
	private void resetKeys()
	{
		_keyUp = false;
		_keyDown = false;
 		_keyLeft = false;
		_keyRight = false;
		_keySpace = false;
	}
	
	void TestForPlayerCollisionsWithEnvironment()
	{
		_collideUp = false;
		_collideDown = false;
		_collideLeft = false;
		_collideRight = false;
		
		if (_keyUp)
		{
			Rect collisionRect = player.box;
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
			Rect collisionRect = player.box;
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
			Rect collisionRect = player.box;
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
			Rect collisionRect = player.box;
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
			player.Move(0, playerTransSpeed);
			if (this.y <= -_dungeon.RoomHeight)
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		case Direction.S:
			this.y += transitionSpeed;
			player.Move(0, -playerTransSpeed);
			if (this.y >= _dungeon.RoomHeight)
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		case Direction.W:
			this.x += transitionSpeed;
			player.Move(-playerTransSpeed, 0);
			if (this.x >= _dungeon.RoomWidth)
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		case Direction.E:
			this.x -= transitionSpeed;
			player.Move(playerTransSpeed, 0);
			if (this.x <= -_dungeon.RoomWidth) 
			{
				ResetRoomDrawn();
				_roomTransitionDirection = Direction.None;
			}
			break;
		}
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
		player.Move(x,y);
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


