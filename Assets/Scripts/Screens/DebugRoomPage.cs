using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DebugRoomPage : FContainer
{
	private FSprite _manSprite;
	private FSprite _floorSprite;
	
	//private FNode _cameraTarget = new FNode();
	
	private Dungeon _dungeon;
	
	public DebugRoomPage()
	{
		_dungeon = new Dungeon(5);
		
		int roomSep = 250;
		float roomScale = 1f;
		
		_floorSprite = new FSprite(_dungeon.CurrentRoom.roomName + ".png");
		_floorSprite.scale = roomScale;
		AddChild(_floorSprite);
		
		// lets draw whole room not just current node and connections.
		// todo: draw connections
		foreach (Room currentRoom in _dungeon.RoomList)
		{
			foreach (Direction dir in _dungeon.RoomList[_dungeon.RoomList.IndexOf(currentRoom)].GetConnectedDirections())
			{
				switch(dir)
				{
				case Direction.N:
					FSprite tempSprite1 = new FSprite(_dungeon.RoomList[currentRoom.connected_N].roomName + ".png");
					tempSprite1.x += _dungeon.RoomList[currentRoom.connected_N].MinimapRoomCoordinates.x * roomSep;
					tempSprite1.y += _dungeon.RoomList[currentRoom.connected_N].MinimapRoomCoordinates.y * roomSep;
					tempSprite1.scale = roomScale;
					AddChild (tempSprite1);
					break;
				case Direction.S:
					FSprite tempSprite2 = new FSprite(_dungeon.RoomList[currentRoom.connected_S].roomName + ".png");
					tempSprite2.x += _dungeon.RoomList[currentRoom.connected_S].MinimapRoomCoordinates.x * roomSep;
					tempSprite2.y += _dungeon.RoomList[currentRoom.connected_S].MinimapRoomCoordinates.y * roomSep;
					tempSprite2.scale = roomScale;
					AddChild (tempSprite2);
					break;
				case Direction.W:
					FSprite tempSprite3 = new FSprite(_dungeon.RoomList[currentRoom.connected_W].roomName + ".png");
					tempSprite3.x += _dungeon.RoomList[currentRoom.connected_W].MinimapRoomCoordinates.x * roomSep;
					tempSprite3.y += _dungeon.RoomList[currentRoom.connected_W].MinimapRoomCoordinates.y * roomSep;
					tempSprite3.scale = roomScale;
					AddChild (tempSprite3);
					break;
				case Direction.E:
					FSprite tempSprite4 = new FSprite(_dungeon.RoomList[currentRoom.connected_E].roomName + ".png");
					tempSprite4.x += _dungeon.RoomList[currentRoom.connected_E].MinimapRoomCoordinates.x * roomSep;
					tempSprite4.y += _dungeon.RoomList[currentRoom.connected_E].MinimapRoomCoordinates.y * roomSep;
					tempSprite4.scale = roomScale;
					AddChild (tempSprite4);
					break;
				}
			}
		}
		
		// *** debug to find collision boxes
		//showCollisionsWithMen();
	}
	
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
		
	}
}


