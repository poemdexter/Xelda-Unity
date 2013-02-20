using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Dungeon
{
	public List<Map> mapList {get;set;}
	public Map currentMap {get;set;}
	
	public Dungeon ()
	{
		mapList = new List<Map>();
		mapList.Add(new Map());
		currentMap = mapList[0];
	}
}