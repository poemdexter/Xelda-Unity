using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DebugMapPage : FContainer
{
	private FSprite _manSprite;
	private FSprite _floorSprite;
	
	//private FNode _cameraTarget = new FNode();
	
	private Dungeon _dungeon;
	
	public DebugMapPage()
	{
		_dungeon = new Dungeon(5);
		
		int mapSep = 250;
		float mapScale = 1f;
		
		_floorSprite = new FSprite(_dungeon.CurrentMap.mapName + ".png");
		_floorSprite.scale = mapScale;
		AddChild(_floorSprite);
		
		// lets draw whole map not just current node and connections.
		// todo: draw connections
		foreach (Map currentMap in _dungeon.MapList)
		{
			foreach (Direction dir in _dungeon.MapList[_dungeon.MapList.IndexOf(currentMap)].GetConnectedDirections())
			{
				switch(dir)
				{
				case Direction.N:
					FSprite tempSprite1 = new FSprite(_dungeon.MapList[currentMap.connected_N].mapName + ".png");
					tempSprite1.x += _dungeon.MapList[currentMap.connected_N].DebugMapPosition.x * mapSep;
					tempSprite1.y += _dungeon.MapList[currentMap.connected_N].DebugMapPosition.y * mapSep;
					tempSprite1.scale = mapScale;
					AddChild (tempSprite1);
					break;
				case Direction.S:
					FSprite tempSprite2 = new FSprite(_dungeon.MapList[currentMap.connected_S].mapName + ".png");
					tempSprite2.x += _dungeon.MapList[currentMap.connected_S].DebugMapPosition.x * mapSep;
					tempSprite2.y += _dungeon.MapList[currentMap.connected_S].DebugMapPosition.y * mapSep;
					tempSprite2.scale = mapScale;
					AddChild (tempSprite2);
					break;
				case Direction.W:
					FSprite tempSprite3 = new FSprite(_dungeon.MapList[currentMap.connected_W].mapName + ".png");
					tempSprite3.x += _dungeon.MapList[currentMap.connected_W].DebugMapPosition.x * mapSep;
					tempSprite3.y += _dungeon.MapList[currentMap.connected_W].DebugMapPosition.y * mapSep;
					tempSprite3.scale = mapScale;
					AddChild (tempSprite3);
					break;
				case Direction.E:
					FSprite tempSprite4 = new FSprite(_dungeon.MapList[currentMap.connected_E].mapName + ".png");
					tempSprite4.x += _dungeon.MapList[currentMap.connected_E].DebugMapPosition.x * mapSep;
					tempSprite4.y += _dungeon.MapList[currentMap.connected_E].DebugMapPosition.y * mapSep;
					tempSprite4.scale = mapScale;
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
		foreach(collisionBox box in _dungeon.CurrentMap.collisionBoxList)
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


