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
	
	public Dungeon (int maxRooms)
	{
		MapList = new List<Map>();
		
		_maxRooms = maxRooms;
		_currentAmtOfRooms = 0;
		Generate();
	}
	
	private void Generate()
	{
		System.Random rand = new System.Random(System.DateTime.Now.Millisecond);
		int mapNumber = rand.Next(1,3);
		// create initial map point
		MapList.Add(new Map("Maps/xelda_map"+mapNumber));
		CurrentMap = MapList[0];
		_currentAmtOfRooms++;
		
		// randomly choose amount of directions to branch depending on which
		// is less: max rooms left to place or connections possible for map
		int todoConnections = Math.Min(_maxRooms - _currentAmtOfRooms, CurrentMap.GetPossibleConnectionCount());
		
		// generated max amount of rooms
		if (todoConnections == 0) return;
		
		// for each needed room, generate and connect the maps
		for (int i = 0; i < todoConnections; i++)
		{
			Direction dir = CurrentMap.GetRandomDirectionForConnection();
			mapNumber = rand.Next(1,3);
			Map map = new Map("Maps/xelda_map"+mapNumber, dir, MapList.IndexOf(CurrentMap));
			MapList.Add(map);
			CurrentMap.ConnectThisToNewMap(dir, _currentAmtOfRooms);
		}
		
		// TODO: make sure everything is connected here via debug
		int a = 0;
		foreach(Map map in MapList)
		{
			Debug.Log("pos: " + a);
			Debug.Log("n" +map.connected_N + " s" + map.connected_S + " w" + map.connected_W + " e" + map.connected_E);
			a++;
		}
		Debug.Log("done");
	}
	
	public void TransitionToConnectedMap(int direction)
	{
		
	}
}