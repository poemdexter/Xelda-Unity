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
	
	public int maxWidth = 8;
	public int maxHeight = 8;
	
	public Minimap minimap;
	
	private readonly int tileSize = 32;
	
	//*** NUMBER OF ROOM TEMPLATES
	private int _maxRoomTemplates = 2;
	
	public Dungeon (int maxRooms)
	{
		RoomList = new List<Room>();
		
		_maxRooms = maxRooms;
		_currentAmtOfRooms = 0;
		_currentRoomIndex = 0;
		Generate();
		minimap = new Minimap(this);
	}
	
	private void Generate()
	{
		// get random room
		int roomNumber = XeldaGame.rand.Next(1, _maxRoomTemplates + 1);
		
		// create initial room point
		Room startRoom = new Room("Rooms/room"+roomNumber);
		startRoom.MinimapRoomCoordinates = new Vector2((float)Math.Round((double)(maxWidth/2)),(float)Math.Round((double)(maxHeight/2))); // start in middle
		startRoom.Visited = true;
		RoomList.Add(startRoom);
		_currentAmtOfRooms++;
		
		// set some properties
		// tile size is 32x32.  We need to adjust room width and height so we don't get the extra space around the room that holds
		// the collisions for the passages.
		RoomWidth = startRoom.GetRoomWidth() - (2 * tileSize);
		RoomHeight = startRoom.GetRoomHeight() - (2 * tileSize);
		
		while (true)
		{
			// out of rooms
			if (_maxRooms - _currentAmtOfRooms <= 0) break;
			if (_currentRoomIndex >= RoomList.Count) break;
			
			CurrentRoom = RoomList[_currentRoomIndex];
			// randomly choose amount of directions to branch depending on which
			// is less: max rooms left to place or connections possible for room
			//int maxPossibleConnections = Math.Min(_maxRooms - _currentAmtOfRooms, CurrentRoom.GetPossibleConnectionCount(RoomList));
			int maxPossibleConnections = Math.Min(_maxRooms - _currentAmtOfRooms, GetPossibleConnectionsCount());
			
			// if we can't connect, go to next room
			if (maxPossibleConnections <= 0)
			{
				_currentRoomIndex++;
				continue;
			}
			
			// generated max amount of rooms
			if (maxPossibleConnections == 0) break;
			
			int todoConnections = XeldaGame.rand.Next(1, maxPossibleConnections+1);
			// for each needed room, generate and connect the rooms
			for (int i = 0; i < todoConnections; i++)
			{
				Direction dir = GetRandomDirectionForConnection(CurrentRoom);
				roomNumber = XeldaGame.rand.Next(1, _maxRoomTemplates + 1);
				Room room = new Room("Rooms/room"+roomNumber, dir, RoomList.IndexOf(CurrentRoom));
				room.Visited = false;
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
	}
	
	public Direction GetRandomDirectionForConnection(Room current)
	{
		int r = XeldaGame.rand.Next(GetPossibleConnectionsCount());
		return GetPossibleConnectionDirections()[r];
	}
	
	public List<Direction> GetPossibleConnectionDirections()
	{
		List<Direction> dList = new List<Direction>();
		Vector2 minimapCoords = CurrentRoom.MinimapRoomCoordinates;
		if (CurrentRoom.connected_N == -1 && minimapCoords.y + 1 < maxHeight && CantFindAdjacentRoom(minimapCoords.x, minimapCoords.y + 1)) dList.Add(Direction.N);
		if (CurrentRoom.connected_S == -1 && minimapCoords.y - 1 >= 0 && CantFindAdjacentRoom(minimapCoords.x, minimapCoords.y - 1)) dList.Add(Direction.S);
		if (CurrentRoom.connected_W == -1 && minimapCoords.x - 1 >= 0 && CantFindAdjacentRoom(minimapCoords.x - 1, minimapCoords.y)) dList.Add(Direction.W);
		if (CurrentRoom.connected_E == -1 && minimapCoords.x + 1 < maxWidth && CantFindAdjacentRoom(minimapCoords.x + 1, minimapCoords.y)) dList.Add(Direction.E);
		return dList;
	}
	
	private bool CantFindAdjacentRoom(float x, float y)
	{
		return RoomList.FindIndex(r => r.MinimapRoomCoordinates == new Vector2(x,y)) == -1;
	}
	
	private int GetPossibleConnectionsCount()
	{
		return GetPossibleConnectionDirections().Count;
	}
	
	// debug helping method that sets x,y point of room
	private void SetDebugRoomPosition(Room parentRoom, Room newRoom, Direction dir)
	{
		switch(dir)
		{
		case Direction.N:
			newRoom.MinimapRoomCoordinates = new Vector2(parentRoom.MinimapRoomCoordinates.x, parentRoom.MinimapRoomCoordinates.y + 1);
			break;
		case Direction.S:
			newRoom.MinimapRoomCoordinates = new Vector2(parentRoom.MinimapRoomCoordinates.x, parentRoom.MinimapRoomCoordinates.y - 1);
			break;
		case Direction.W:
			newRoom.MinimapRoomCoordinates = new Vector2(parentRoom.MinimapRoomCoordinates.x - 1, parentRoom.MinimapRoomCoordinates.y);
			break;
		case Direction.E:
			newRoom.MinimapRoomCoordinates = new Vector2(parentRoom.MinimapRoomCoordinates.x + 1, parentRoom.MinimapRoomCoordinates.y);
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