using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Dungeon
{
	public List<Room> RoomList {get;set;}
	public Room CurrentRoom {get;set;}
	public Room TransitionRoom {get;set;}
	public int RoomWidth {get;set;}
	public int RoomHeight {get;set;}
	
	private int _maxRooms;
	private int _currentAmtOfRooms;
	private int _currentRoomIndex;
	
	//*** NUMBER OF ROOM TEMPLATES
	private int _maxRoomTemplates = 1;

	
	public Dungeon (int maxRooms)
	{
		RoomList = new List<Room>();
		
		_maxRooms = maxRooms;
		_currentAmtOfRooms = 0;
		_currentRoomIndex = 0;
		Generate();
	}
	
	private void Generate()
	{
		// get random room
		int roomNumber = XeldaGame.rand.Next(1, _maxRoomTemplates + 1);
		
		// create initial room point
		Room startRoom = new Room("Rooms/room"+roomNumber);
		startRoom.DebugRoomPosition = new Vector2(0,0);
		RoomList.Add(startRoom);
		_currentAmtOfRooms++;
		
		// set some properties
		// tile size is 24x24.  We need to adjust room width and height so we don't get the extra space around the room that holds
		// the collisions for the passages.
		int tileSize = 32;
		RoomWidth = startRoom.GetRoomWidth() - (2 * tileSize);
		RoomHeight = startRoom.GetRoomHeight() - (2 * tileSize);
		
		while (true)
		{
			CurrentRoom = RoomList[_currentRoomIndex];
			// randomly choose amount of directions to branch depending on which
			// is less: max rooms left to place or connections possible for room
			int maxPossibleConnections = Math.Min(_maxRooms - _currentAmtOfRooms, CurrentRoom.GetPossibleConnectionCount());
			
			// generated max amount of rooms
			if (maxPossibleConnections == 0) break;
			
			int todoConnections = XeldaGame.rand.Next(1, maxPossibleConnections+1);
			// for each needed room, generate and connect the rooms
			for (int i = 0; i < todoConnections; i++)
			{
				Direction dir = CurrentRoom.GetRandomDirectionForConnection();
				roomNumber = XeldaGame.rand.Next(1, _maxRoomTemplates + 1);
				Room room = new Room("Rooms/room"+roomNumber, dir, RoomList.IndexOf(CurrentRoom));
				SetDebugRoomPosition(CurrentRoom, room, dir);
				RoomList.Add(room);
				_currentAmtOfRooms++;
				CurrentRoom.ConnectThisToNewRoom(dir, RoomList.IndexOf(room));
			}
			
			// treat list like queue and go to next node and build off of that.
			_currentRoomIndex++;
		}
		
		// set CurrentRoom back to 0 so player starts at first node.
		CurrentRoom = RoomList[0];
		
		// tell each room to add walls to block passages that have no adjacent room to connect
		foreach(Room room in RoomList) room.AddPassageWalls();
		
		// make sure everything is connected here via debug
//		int a = 0;
//		foreach(Room room in RoomList)
//		{
//			Debug.Log("pos: " + a + " n:" +room.connected_N + " s:" + room.connected_S + " w:" + room.connected_W + " e:" + room.connected_E);
//			a++;
//		}
	}
	
	// debug helping method that sets x,y point of room
	private void SetDebugRoomPosition(Room parentRoom, Room newRoom, Direction dir)
	{
		switch(dir)
		{
		case Direction.N:
			newRoom.DebugRoomPosition = new Vector2(parentRoom.DebugRoomPosition.x, parentRoom.DebugRoomPosition.y + 1);
			break;
		case Direction.S:
			newRoom.DebugRoomPosition = new Vector2(parentRoom.DebugRoomPosition.x, parentRoom.DebugRoomPosition.y - 1);
			break;
		case Direction.W:
			newRoom.DebugRoomPosition = new Vector2(parentRoom.DebugRoomPosition.x - 1, parentRoom.DebugRoomPosition.y);
			break;
		case Direction.E:
			newRoom.DebugRoomPosition = new Vector2(parentRoom.DebugRoomPosition.x + 1, parentRoom.DebugRoomPosition.y);
			break;
		}
	}
	
	public void PassageToConnectedRoom(string direction)
	{
		switch(direction)
		{
		case "NORTH":
			if (CurrentRoom.connected_N != -1) TransitionRoom = RoomList[CurrentRoom.connected_N];
			break;
		case "SOUTH":
			if (CurrentRoom.connected_S != -1) TransitionRoom = RoomList[CurrentRoom.connected_S];
			break;
		case "WEST":
			if (CurrentRoom.connected_W != -1) TransitionRoom = RoomList[CurrentRoom.connected_W];
			break;
		case "EAST":
			if (CurrentRoom.connected_E != -1) TransitionRoom = RoomList[CurrentRoom.connected_E];
			break;
		}
	}
	
	public void ChangeTransitionToCurrentRoom()
	{
		CurrentRoom = TransitionRoom;
		// ret position since transitioning changed these values
		CurrentRoom.x = 0;
		CurrentRoom.y = 0;
	}
}