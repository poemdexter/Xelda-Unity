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
	
	private bool _fireUp = false;
	private bool _fireDown = false;
	private bool _fireLeft = false;
	private bool _fireRight = false;
	
	private bool _collideUp = false;
	private bool _collideDown = false;
	private bool _collideLeft = false;
	private bool _collideRight = false;
	
	private Direction _roomTransitionDirection = Direction.None;
	public Player player;
	private float _moveSpeed = 2f;
	
	// controls transition speeds for player during move from one map to another
	private float player_NS_TransSpeed = .5f;
	private float player_EW_TransSpeed = .3f;
	
	public Dungeon _dungeon;
	private FContainer Dungeon_Container;
	
	private readonly int DUNGEON_CONTAINER_OFFSET = -100;
	
	public GamePage ()
	{
		// create dungeon
		_dungeon = new Dungeon(20);
		
		// add the dungeon/game portion of the screen
		Dungeon_Container = new FContainer();
		AddChild (Dungeon_Container);
		
		// add current room
		Dungeon_Container.AddChild(_dungeon.CurrentRoom);
		
		// create player
		int px = (int)_dungeon.CurrentRoom.playerSpawnBox.box.x;
		int py = (int)_dungeon.CurrentRoom.playerSpawnBox.box.y;
		player = new Player(px, py);
		Dungeon_Container.AddChild(player);
		
		// reposition dungeon container to make room for UI
		Dungeon_Container.y = DUNGEON_CONTAINER_OFFSET;
		
		// add the UI portion of the screen
		AddChild (UI_Manager.getGameUIContainer(this));
		
		// *** debug to find collision boxes
		//showCollisionsWithMen();
		
		this.scale = 1;
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
			Combat_Manager.HandleProjectileMovement(_dungeon.CurrentRoom);
			Combat_Manager.CheckCombatCollisions(player, _dungeon.CurrentRoom);
			Loot_Manager.CheckForPlayerLootCollisions(player, _dungeon.CurrentRoom);
			
			// ** HANDLE PLAYER DEATH SOMETIME
			if (!player.Alive) Debug.Log("Player Death.");
		}
	}
	
	private void HandlePlayerAttacking()
	{
		if (IsFireButtonPressed() && player.attackDelay <= 0) // player hits attack button
		{
			Combat_Manager.PlayerAttack(player, _dungeon.CurrentRoom);
			player.attackDelay = player.attackDelayTime;
		}
		else player.attackDelay--;
	}
	
	private bool IsFireButtonPressed()
	{
		return _fireUp || _fireDown || _fireLeft || _fireRight;
	}
	
	void HandleInputs()
	{
		// pushed movement key
		if (Input.GetKeyDown(KeyCode.W)) // up
		{
			_keyUp = true;
			player.AnimationFacing = Direction.N;
		}
		if (Input.GetKeyDown(KeyCode.S)) // down
		{
			_keyDown = true;
			player.AnimationFacing = Direction.S;
		}
		if (Input.GetKeyDown(KeyCode.A)) // left
		{
			_keyLeft = true;
			player.AnimationFacing = Direction.W;
		}
		if (Input.GetKeyDown(KeyCode.D)) // right
		{
			_keyRight = true;
			player.AnimationFacing = Direction.E;
		}
		
		// pushed projectile key
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			_fireUp = true;
			player.Facing = Direction.N;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			_fireDown = true;
			player.Facing = Direction.S;
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_fireLeft = true;
			player.Facing = Direction.W;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			_fireRight = true;
			player.Facing = Direction.E;
		}
		
		// let go of move key
		if (Input.GetKeyUp(KeyCode.W)) _keyUp = false;
		if (Input.GetKeyUp(KeyCode.S)) _keyDown = false;
		if (Input.GetKeyUp(KeyCode.A)) _keyLeft = false;
		if (Input.GetKeyUp(KeyCode.D)) _keyRight = false;
		
		// let go of projectile key
		if (Input.GetKeyUp(KeyCode.UpArrow))    _fireUp = false;
		if (Input.GetKeyUp(KeyCode.DownArrow))  _fireDown = false;
		if (Input.GetKeyUp(KeyCode.LeftArrow))  _fireLeft = false;
		if (Input.GetKeyUp(KeyCode.RightArrow)) _fireRight = false;
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
		
		_fireUp = false;
		_fireDown = false;
		_fireRight = false;
		_fireLeft = false;
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
		Dungeon_Container.AddChildAtIndex(room, 0);
		_roomTransitionDirection = dir;
	}
	
	private void DoRoomTransition(Direction dir)
	{
		float transitionSpeed = 4f;
		
		resetKeys(); // do this since we never catch keyUp after hitting a passage
		
		switch(dir)
		{
		case Direction.N:
			Dungeon_Container.y -= transitionSpeed;
			player.Move(0, player_NS_TransSpeed);
			if (Dungeon_Container.y <= -_dungeon.RoomHeight + DUNGEON_CONTAINER_OFFSET) FinishedTransitioning(_roomTransitionDirection);
			break;
		case Direction.S:
			Dungeon_Container.y += transitionSpeed;
			player.Move(0, -player_NS_TransSpeed);
			if (Dungeon_Container.y >= _dungeon.RoomHeight + DUNGEON_CONTAINER_OFFSET) FinishedTransitioning(_roomTransitionDirection);
			break;
		case Direction.W:
			Dungeon_Container.x += transitionSpeed;
			player.Move(-player_EW_TransSpeed, 0);
			if (Dungeon_Container.x >= _dungeon.RoomWidth) FinishedTransitioning(_roomTransitionDirection);
			break;
		case Direction.E:
			Dungeon_Container.x -= transitionSpeed;
			player.Move(player_EW_TransSpeed, 0);
			if (Dungeon_Container.x <= -_dungeon.RoomWidth) FinishedTransitioning(_roomTransitionDirection);
			break;
		}
	}
	
	private void FinishedTransitioning(Direction dir)
	{
		ResetRoomDrawn();
		_dungeon.minimap.ChangePlayerIconPosition(dir);
		_roomTransitionDirection = Direction.None;
	}
	
	// removes the old room sprite and the temp new room sprite
	// resets camera back to origin and moves player as well
	private void ResetRoomDrawn()
	{
		// make transition the current and redraw
		Dungeon_Container.RemoveChild(_dungeon.CurrentRoom);
		_dungeon.ChangeTransitionToCurrentRoom();
		Dungeon_Container.RemoveChild(_dungeon.TransitionRoom);
		Dungeon_Container.AddChildAtIndex(_dungeon.CurrentRoom,0);
		
		// redraw minimap
		_dungeon.minimap.UpdateMinimap(_dungeon);
		
		// readjust player
		int x = 0;
		int y = 0;
		if (Dungeon_Container.x != 0) x = (Dungeon_Container.x > 0) ? _dungeon.RoomWidth : -_dungeon.RoomWidth;
		if (Dungeon_Container.y != DUNGEON_CONTAINER_OFFSET) y = (Dungeon_Container.y > 0) ? _dungeon.RoomHeight : -_dungeon.RoomHeight;
		player.Move(x,y);
		Dungeon_Container.x = 0;
		Dungeon_Container.y = 0 + DUNGEON_CONTAINER_OFFSET;
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


