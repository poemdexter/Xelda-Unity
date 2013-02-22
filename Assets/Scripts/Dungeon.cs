using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Dungeon
{
	public List<Map> MapList {get;set;}
	public Map CurrentMap {get;set;}
	
	private int _maxRooms;
	private int _currentAmtOfRooms;
	private int _currentRoomIndex;
	
	public Dungeon (int maxRooms)
	{
		MapList = new List<Map>();
		
		_maxRooms = maxRooms;
		_currentAmtOfRooms = 0;
		_currentRoomIndex = 0;
		Generate();
	}
	
	private void Generate()
	{
		System.Random rand = new System.Random(System.DateTime.Now.Millisecond);
		
		// get random map
		int mapNumber = rand.Next(1,3);
		
		// create initial map point
		Map startMap = new Map("Maps/xelda_map"+mapNumber);
		startMap.DebugMapPosition = new Vector2(0,0);
		MapList.Add(startMap);
		_currentAmtOfRooms++;
		
		while (true)
		{
			CurrentMap = MapList[_currentRoomIndex];
			// randomly choose amount of directions to branch depending on which
			// is less: max rooms left to place or connections possible for map
			int maxPossibleConnections = Math.Min(_maxRooms - _currentAmtOfRooms, CurrentMap.GetPossibleConnectionCount());
			
			// generated max amount of rooms
			if (maxPossibleConnections == 0) break;
			
			int todoConnections = rand.Next(1, maxPossibleConnections+1);
			// for each needed room, generate and connect the maps
			for (int i = 0; i < todoConnections; i++)
			{
				Direction dir = CurrentMap.GetRandomDirectionForConnection();
				mapNumber = rand.Next(1,3);
				Map map = new Map("Maps/xelda_map"+mapNumber, dir, MapList.IndexOf(CurrentMap));
				SetDebugMapPosition(CurrentMap, map, dir);
				MapList.Add(map);
				_currentAmtOfRooms++;
				CurrentMap.ConnectThisToNewMap(dir, MapList.IndexOf(map));
			}
			
			// treat list like queue and go to next node and build off of that.
			_currentRoomIndex++;
		}
		
		// set CurrentMap back to 0 so player starts at first node.
		CurrentMap = MapList[0];
		
		// make sure everything is connected here via debug
		int a = 0;
		foreach(Map map in MapList)
		{
			Debug.Log("pos: " + a + " n:" +map.connected_N + " s:" + map.connected_S + " w:" + map.connected_W + " e:" + map.connected_E);
			a++;
		}
	}
	
	private void SetDebugMapPosition(Map parentMap, Map newMap, Direction dir)
	{
		switch(dir)
		{
		case Direction.N:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x, parentMap.DebugMapPosition.y + 1);
			break;
		case Direction.S:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x, parentMap.DebugMapPosition.y - 1);
			break;
		case Direction.W:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x - 1, parentMap.DebugMapPosition.y);
			break;
		case Direction.E:
			newMap.DebugMapPosition = new Vector2(parentMap.DebugMapPosition.x + 1, parentMap.DebugMapPosition.y);
			break;
		}
	}
	
	public void TransitionToConnectedMap(int direction)
	{
		
	}
}